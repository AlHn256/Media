using System;
using System.Collections.Generic;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Кэш видеоиндексов</summary>
    public  class VideoIndexesCache
    {

        #region Fields

        /// <summary>Кэш коллекции видеоиндексов для каждого видеопотока</summary>
        Dictionary<int, List<SingleStreamFrameIndex>> _Cache;

        /// <summary>Количество предполагаемых по умолчанию потоков</summary>
        public static int DefaultStreamsCount = 4;

        /// <summary>Количество предполагаемых по умолчанию кадров для каждого видеопотока
        /// (25 кадрос/сек * 1800 сек = 45000 кадров)</summary>
        public static int DefaultStreamIndexesCount = 45000;

        #endregion

        #region Methods

        /// <summary>Конструктор класса</summary>
        public VideoIndexesCache() : this(DefaultStreamsCount) { }

        /// <summary>Конструктор класса</summary>
        /// <param name="streams_count">Предполагаемое число видеопотоков</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Недопустимое число видеопотоков</exception>
        public VideoIndexesCache(int streams_count)
        {
            if (streams_count < 0) { throw new ArgumentOutOfRangeException(); }
            _Cache = new Dictionary<int, List<SingleStreamFrameIndex>>(streams_count);
        }

        /// <summary>Прочитать в кэш индексы из заданного потока 
        /// (чтение начинается с текущей позиции читающего курсора потока)</summary>
        /// <param name="stream">Поток видеонидексов</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на поток, содержащий индексы</exception>
        /// <exception cref="System.ObjectDisposedException">Поток был закрыт</exception>
        /// <exception cref="AlfaPribor.VideoStorage.VideoStorageException">Не удалось прочитать видеоиндексы</exception>
        public void Read(IVideoIndexesStream stream)
        {
            if (stream == null) { throw new ArgumentNullException(); }
            try
            {
                stream.Position = 0;
                SingleStreamFrameIndex index;
                while (stream.ReadIndex(out index) != 0) { Add(index); }
            }
            catch (ObjectDisposedException) { throw; }
            catch (Exception E) { throw new VideoStorageException(E.Message, E); }
        }

        /// <summary>Записать в поток индексы из кэша
        /// (запись начинается с текущей позиции записывающего курсора потока)</summary>
        /// <param name="stream">Поток видеонидексов</param>
        /// <exception cref="System.ObjectDisposedException">Целевой поток был закрыт</exception>
        /// <exception cref="System.NotSupportedException">Поток не поддерживает запись</exception>
        /// <exception cref="System.IO.IOException">Ошибка ввода/вывода</exception>
        public void Write(IVideoIndexesStream stream)
        {
            // Считаем общее количество индексов видеокадров
            int count = 0;
            foreach (List<SingleStreamFrameIndex> list in _Cache.Values) { count += list.Count; }

            // Формируем единый список индексов, отсортированный по метке времени видеокадра каждого индекса по возрастанию
            List<SingleStreamFrameIndex> sorted_indexes = new List<SingleStreamFrameIndex>(count);
            IndexComparer ic = new IndexComparer();
            foreach (KeyValuePair<int, List<SingleStreamFrameIndex>> item in _Cache)
            {
                if (item.Value == null) { continue; }
                foreach (SingleStreamFrameIndex index in item.Value)
                {
                    if (index != null)
                    {
                        //Поиск такого же элемента в сортированном списке
                        int pos = sorted_indexes.BinarySearch(index, ic);
                        if (pos < 0)
                        {
                            pos = ~pos;
                            if (pos >= sorted_indexes.Count) { sorted_indexes.Add(index); }
                            else { sorted_indexes.Insert(pos, index); }
                        }
                        else { }
                    }
                }
            }
            // Сохраняем отсортированные индексы в поток
            foreach (SingleStreamFrameIndex index in sorted_indexes) { stream.WriteIndex(index); }
        }

        /// <summary>Добавляет индекс в кэш</summary>
        /// <param name="index">Ссылка на видеоиндекс, добавляемый в кэш</param>
        /// <param name="sorted">Признак сортировки списка, в который добавляется видеоиндекс</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на видеоиндекс</exception>
        /// <returns>Возвращает список индексов для потока, к которому относится index</returns>
        public List<SingleStreamFrameIndex> Add(SingleStreamFrameIndex index, bool sorted)
        {
            if (index == null) { throw new ArgumentNullException(); }

            List<SingleStreamFrameIndex> IndexesList = null;

            // Если список индексов для потока, к которому принадлежит index, еще не существует - создаем его
            if (_Cache.ContainsKey(index.StreamId)) IndexesList = _Cache[index.StreamId];
            else
            {
                IndexesList = new List<SingleStreamFrameIndex>(DefaultStreamIndexesCount);
                _Cache[index.StreamId] = IndexesList;
            }

            // Добавляем индекс в кэш
            if (!sorted) IndexesList.Add(index);
            else
            {
                // Считаем список отсортированным
                int pos = IndexesList.BinarySearch(index);
                if (pos < 0)
                {
                    pos = ~pos;
                    if (pos >= IndexesList.Count) IndexesList.Add(index);
                    else IndexesList.Insert(pos, index);
                }
            }

            return IndexesList;
        }

        /// <summary>Добавляет индекс в кэш</summary>
        /// <param name="index">Ссылка на видеоиндекс, добавляемый в кэш</param>
        /// <exception cref="System.ArgumentNullException">Не задана ссылка на видеоиндекс</exception>
        /// <returns>Возвращает список индексов для потока, к которому относится index</returns>
        public List<SingleStreamFrameIndex> Add(SingleStreamFrameIndex index)
        {
            return Add(index, false);
        }

        /// <summary>Сортировать списки индексов для каждого видеопотока в кэше</summary>
        public void Sort()
        {
            foreach (KeyValuePair<int, List<SingleStreamFrameIndex>> item in _Cache)
            {
                if (item.Value == null) continue;
                item.Value.Sort();
            }
        }

        /// <summary>Очистить кэш</summary>
        public void Clear()
        {
            _Cache.Clear();
        }

        #endregion

        #region Properties

        /// <summary>Коллекция видеоиндексов, ключем для которой является идентификатор видеопотока</summary>
        /// <exception cref="System.ArgumentNullException">Присваиваемое значение равно null</exception>
        public Dictionary<int, List<SingleStreamFrameIndex>> Indexes
        {
            get { return _Cache; }
            set 
            {
                if (value == null) { throw new ArgumentNullException(); }
                _Cache = value;
            }
        }

        #endregion

        public class IndexComparer : IComparer<SingleStreamFrameIndex>
        {
            public int Compare(SingleStreamFrameIndex x, SingleStreamFrameIndex y)
            {
                if (x == null)
                {
                    if (y == null) return 0;   // Если x == null и y == null, то они равны
                    else return -1; // Если x == null и y != null, то у больше
                }
                else
                {
                    if (y == null) return 1;
                    else
                    {
                        //x и y != null - нормальное сравнение

                        //Один поток
                        if (x.StreamId == y.StreamId)
                        {
                            if (x.TimeStamp == y.TimeStamp) return 0;//Равны
                            else 
                            {
                                if (x.TimeStamp > y.TimeStamp) return 1;
                                else return -1;
                            }
                        }
                        //Разные потоки
                        else 
                        {
                            if (x.TimeStamp > y.TimeStamp) return 1;
                            else
                            {
                                if (x.TimeStamp == y.TimeStamp)
                                {
                                    if (x.StreamId > y.StreamId) return 1;
                                    return 1;
                                }
                                else return -1;
                            }
                        }
                    }
                }
            }
        }
    }
}
