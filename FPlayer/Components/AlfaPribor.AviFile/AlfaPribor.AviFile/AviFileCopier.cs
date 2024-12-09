using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;
using AlfaPribor.AviFile;

namespace AlfaPribor.AviFile
{
    /// <summary>Выполняет копирование информации между avi-файлами в соответствии с заданными правилами</summary>
    public class AviFileCopier : IDisposable
    {
        #region Fields

        /// <summary>Размер буфера для хранения данных одного сэмпла</summary>
        private int _SampleBufferSize;

        /// <summary>Указатель на буфер в неуправляемой памяти</summary>
        private IntPtr _Buffer;

        /// <summary>Признак освобождения ресурсов объекта</summary>
        private bool _Disposed = false;

        #endregion

        #region Methods

        /// <summary>Конструктор объектов класса</summary>
        /// <param name="buffer_size">Размер буфера для хранения данных</param>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти</exception>
        public AviFileCopier(int buffer_size)
        {
            SampleBufferSize = buffer_size;
        }

        /// <summary>Конструктор объектов класса.
        /// Устанавливает свойства объекта в значения по умолчанию
        /// </summary>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти</exception>
        public AviFileCopier()
        {
            SampleBufferSize = 1024 * 1024 * 2; // 2 Мб
        }

        /// <summary>Деструктор объекта</summary>
        ~AviFileCopier()
        {
            Dispose(false);
        }

        /// <summary>Копирует данные между avi-файлами в соответствии с заданными правилами</summary>
        /// <param name="rules">Правила копирования</param>
        public void Copy(IList<CopyRule> rules)
        {
            if (rules == null)
            {
                throw new ArgumentNullException();
            }
            foreach (var rule in rules)
            {
                int sampleIndex = rule.StartSample;
                int lastWritedSample = 0;
                try
                {
                    // Проверяем правило
                    CheckRule(rule);
                    // Запускаем декомпрессию исходного потока
                    if (rule.ReadMode == CopyMode.Decode)
                    {
                        Avi.BITMAPINFOHEADER biHeader = rule.SourceFile.GetFrameInfo(rule.SourceStreamIndex, 0);
                        rule.SourceFile.BeginDecompress(rule.SourceStreamIndex, biHeader);
                    }
                    int endSampleIndex = sampleIndex + rule.SamplesCount;
                    while (sampleIndex < endSampleIndex)
                    {
                        int bytesRead = 0;
                        int samplesRead = 1;
                        Avi.BITMAPINFOHEADER biHeader;
                        byte[] data;
                        if (rule.ReadMode == CopyMode.Decode)
                        {
                            data = rule.SourceFile.ReadDecompress(rule.SourceStreamIndex, sampleIndex, out biHeader);
                            if (data.Length > _SampleBufferSize)
                            {
                                throw new BufferTooSmallException();
                            }
                            
                        }
                        else
                        {
                            biHeader = rule.SourceFile.GetFrameInfo(rule.SourceStreamIndex, sampleIndex);
                            data = rule.SourceFile.Read(rule.SourceStreamIndex, sampleIndex, ref samplesRead);
                        }
                        object dataInfo = (object)biHeader;
                        RaiseReadSampleEvent(rule, sampleIndex - rule.StartSample, ref dataInfo, ref data);
                        biHeader = (Avi.BITMAPINFOHEADER)dataInfo;

                        Marshal.Copy(data, 0, _Buffer, data.Length);
                        bytesRead += data.Length;

                        int bytesWrite = 0;
                        int samplesWrite = samplesRead;
                        if (rule.WriteMode == CopyMode.Encode)
                        {
                            bytesWrite = rule.DestFile.WriteCompress(
                                rule.DestStreamIndex, lastWritedSample, ref samplesWrite, _Buffer, bytesRead
                            );
                        }
                        else
                        {
                            bytesWrite = rule.DestFile.Write(rule.DestStreamIndex, lastWritedSample, ref samplesWrite, _Buffer, bytesRead);
                        }

                        RaiseCopySampleEvent(rule, sampleIndex - rule.StartSample);

                        lastWritedSample += samplesWrite;
                        sampleIndex += samplesRead;
                    }
                    // Завершаем декомпрессию исходного потока
                    if (rule.ReadMode == CopyMode.Decode)
                    {
                        rule.SourceFile.EndDecompress(rule.SourceStreamIndex);
                    }
                }
                catch (Exception E)
                {
                    try
                    {
                        RaiseCopyExceptionEvent(rule, E);
                    }
                    catch
                    {
                        // Отмена дальнейшиго копирования данных
                        return;
                    }
                }
            }
        }

        /// <summary>Генерирует событие "Чтение сэмпла данных"</summary>
        /// <param name="rule">Правило копирования</param>
        /// <param name="number">Порядковый номер прочитанного сэмпла данных</param>
        /// <param name="data_info">Информация о прочитанных данных</param>
        /// <param name="data">Данные</param>
        /// <exception cref="AlfaPribor.AviFile.AviFileException">Ошибка в обработчике события</exception>
        private void RaiseReadSampleEvent(CopyRule rule, int number, ref object data_info, ref byte[] data)
        {
            if (ReadSample != null)
            {
                try
                {
                    ReadSampleEventArgs args = new ReadSampleEventArgs(rule, number, data_info, data);
                    ReadSample(this, args);
                    data_info = args.Info;
                    data = args.Data;
                }
                catch (Exception e)
                {
                    throw new AviFileException("ReadSample event handler raise exception!", e);
                }
            }
        }

        /// <summary>Генерирует событие "Копирование экземпляра данных"</summary>
        /// <param name="rule">Правило копирования</param>
        /// <param name="number">Порядковый номер прочитанного сэмпла данных</param>
        protected void RaiseCopySampleEvent(CopyRule rule, int number)
        {
            if (CopySample != null)
            {
                try
                {
                    CopySample(this, new CopySampleEventArgs(rule, number));
                }
                catch { }
            }
        }

        /// <summary>Проверяет правило копирования</summary>
        /// <param name="rule">Правило копирования</param>
        /// <exception cref="System.ArgumentNullException">Не задан источник или приемник данных</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Индекс потока в источнике или приемнике указан неверно</exception>
        /// <exception cref="System.IO.IOException">Файл-источник или файл-приемник не открыты</exception>
        private void CheckRule(CopyRule rule)
        {
            if (rule.SourceFile == null)
            {
                throw new ArgumentNullException("SourceFile file not set!");
            }
            if (!rule.SourceFile.IsOpen)
            {
                throw new IOException("SourceFile file not open!");
            }
            if (rule.SourceStreamIndex < 0 || rule.SourceStreamIndex > rule.SourceFile.StreamsCount - 1)
            {
                throw new ArgumentOutOfRangeException("Stream index in source file is out of range!");
            }
            if (rule.ReadMode == CopyMode.Encode)
            {
                throw new ArgumentException("Source stream read mode is invalid!");
            }
            if (rule.DestFile == null)
            {
                throw new ArgumentNullException("Destination file not set!");
            }
            if (!rule.DestFile.IsOpen)
            {
                throw new IOException("Destination file not open!");
            }
            if (rule.DestStreamIndex < 0 || rule.DestStreamIndex > rule.DestFile.StreamsCount - 1)
            {
                throw new ArgumentOutOfRangeException("Stream index in destination file is out of range!");
            }
            if (rule.WriteMode == CopyMode.Decode)
            {
                throw new ArgumentException("Destination stream write mode is invalid!");
            }
        }

        /// <summary>Генерирует событие "Исключение при копировании данных"</summary>
        /// <param name="rule">Правило копирования данных</param>
        /// <param name="except">ВОзникшее исключение</param>
        /// <exception cref="AlfaPribor.AviFile.AbortException">Пользователь прервал процесс копирования</exception>
        protected void RaiseCopyExceptionEvent(CopyRule rule, Exception except)
        {
            if (CopyException != null)
            {
                try
                {
                    CopyExceptionEventArgs args = new CopyExceptionEventArgs(rule, except);
                    CopyException(this, args);
                    if (args.Cancel)
                    {
                        throw new AbortException();
                    }
                }
                catch (AbortException)
                {
                    throw;
                }
                catch { }
            }
        }

        #endregion

        #region Properties

        /// <summary>Размер буфера для хранения данных</summary>
        /// <exception cref="System.OutOfMemoryException">Недостаточно свободной памяти</exception>
        public int SampleBufferSize
        {
            get { return _SampleBufferSize; }
            set
            {
                if (_SampleBufferSize == value)
                {
                    return;
                }
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException();
                }
                if (_Buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(_Buffer);
                }
                try
                {
                    _Buffer = Marshal.AllocHGlobal(value);
                }
                catch
                {
                    _Buffer = IntPtr.Zero;
                    throw;
                }
                _SampleBufferSize = value;
            }
        }

        #endregion

        #region Events

        /// <summary>Событие "Исключение при копировании данных"</summary>
        public event EventHandler<CopyExceptionEventArgs> CopyException;

        /// <summary>Событие "Копирование экземпляра данных"</summary>
        public event EventHandler<CopySampleEventArgs> CopySample;

        /// <summary>Событие "Чтение экземпляра данных"</summary>
        public event EventHandler<ReadSampleEventArgs> ReadSample;

        #endregion

        #region IDisposable members

        /// <summary>Освобождает используемые объектом ресурсы</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Освобождает ресурсы объекта</summary>
        /// <param name="disposing">Определяет, какие ресурсы освобождать.
        /// Если установлено значение TRUE - освобождаются все ресурсы (управляемые/неуправляемые),
        /// если FALSE - только неуправляемые ресурсы
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_Disposed)
            {
                return;
            }
            if (_Buffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_Buffer);
                _Buffer = IntPtr.Zero;
            }
            _Disposed = true;
        }

        #endregion
    }

    /// <summary>Описывает правило копирования</summary>
    public struct CopyRule
    {
        /// <summary>Индекс потока в файле-источнике</summary>
        public int SourceStreamIndex;

        /// <summary>Индекс потока в файле-приемнике</summary>
        public int DestStreamIndex;

        /// <summary>Индекс первого сэмпла, с которого начинается копирование</summary>
        public int StartSample;

        /// <summary>Количество копируемых сэмплов</summary>
        public int SamplesCount;

        /// <summary>Копируемый файл</summary>
        public AviFile SourceFile;

        /// <summary>Целевой файл</summary>
        public AviFile DestFile;

        /// <summary>Режим чтения данных из файла-источника</summary>
        public CopyMode ReadMode;

        /// <summary>Режим записи данных в файл-приемник</summary>
        public CopyMode WriteMode;

        /// <summary>Конструктор объекта</summary>
        /// <param name="source">Файл-источник</param>
        /// <param name="dest">Файл-приемник</param>
        /// <param name="src_stream_index">Индекс потока в файле-источнике</param>
        /// <param name="dst_stream_index">Индекс потока в файле-приемнике</param>
        /// <param name="start_sample">Индекс первого сэмпла, с которого начинается копирование</param>
        /// <param name="samples_count">Количество копируемых сэмплов</param>
        public CopyRule(AviFile source, AviFile dest, int src_stream_index, int dst_stream_index, int start_sample, int samples_count)
        {
            SourceFile = source;
            DestFile = dest;
            SourceStreamIndex = src_stream_index;
            DestStreamIndex = dst_stream_index;
            StartSample = start_sample;
            SamplesCount = samples_count;
            ReadMode = CopyMode.Original;
            WriteMode = CopyMode.Original;
        }

        /// <summary>Конструктор объекта</summary>
        /// <param name="source">Файл-источник</param>
        /// <param name="dest">Файл-приемник</param>
        /// <param name="src_stream_index">Индекс потока в файле-источнике</param>
        /// <param name="dst_stream_index">Индекс потока в файле-приемнике</param>
        /// <param name="start_sample">Индекс первого сэмпла, с которого начинается копирование</param>
        /// <param name="samples_count">Количество копируемых сэмплов</param>
        /// <param name="read_mode">Режим чтения данных из файла-источника</param>
        /// <param name="write_mode">Режим записи данных в файл-приемник</param>
        public CopyRule(AviFile source, AviFile dest, int src_stream_index, int dst_stream_index, int start_sample, int samples_count,
            CopyMode read_mode, CopyMode write_mode)
        {
            SourceFile = source;
            DestFile = dest;
            SourceStreamIndex = src_stream_index;
            DestStreamIndex = dst_stream_index;
            StartSample = start_sample;
            SamplesCount = samples_count;
            ReadMode = read_mode;
            WriteMode = write_mode;
        }
    }

    /// <summary>Режим копирования данных</summary>
    public enum CopyMode
    {
        /// <summary>Оставить данные без изменений</summary>
        Original,

        /// <summary>Кодировать данные</summary>
        Encode,

        /// <summary>Декодировать данные</summary>
        Decode,
    }
}
