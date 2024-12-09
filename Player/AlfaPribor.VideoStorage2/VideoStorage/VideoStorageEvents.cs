using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Делегат "Запрос на удаление записи из кольцевого буфера"</summary>
    /// <param name="sender">Отправитель</param>
    /// <param name="e">Аргумент события</param>
    public delegate void EvCircularBufferDeleting(object sender, CircularBufferCancelEventArgs e);

    /// <summary>Делегат "Удалена запись из кольцевого буфера"</summary>
    /// <param name="sender">Отправитель</param>
    /// <param name="e">Аргумент события</param>
    public delegate void EvCircularBufferDeleted(object sender, CircularBufferEventArgs e);

    /// <summary>Делегат "Закончена синхронизация хранилища"</summary>
    /// <param name="sender">Отправитель</param>
    /// <param name="e">Аргумент события</param>
    public delegate void EvSyncComplete(object sender, SyncCompleteEventArgs e);

    /// <summary>Делегат "Открыта видеозапись хранилища"</summary>
    /// <param name="sender">Отправитель</param>
    /// <param name="e">Аргумент события</param>
    public delegate void EvRecordOpen(object sender, VideoRecordEventArgs e);

    /// <summary>Делегат "Закрытие видеозаписи хранилища"</summary>
    /// <param name="sender">Отправитель</param>
    /// <param name="e">Аргумент события</param>
    public delegate void EvRecordClose(object sender, VideoRecordEventArgs e);

    /// <summary>Делегат "Удаление видеозаписи из хранилища"</summary>
    /// <param name="sender">Отправитель</param>
    /// <param name="e">Аргумент события</param>
    public delegate void EvRecordDelete(object sender, VideoRecordEventArgs e);

    /// <summary>Аргумент события "Удаление видеофрагмента из кольцевого буфера"</summary>
    public class CircularBufferCancelEventArgs : CancelEventArgs
    {
        /// <summary>Идентификатор поезда / видеозаписи</summary>
        private string _Id;

        /// <summary>Конструктор</summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        public CircularBufferCancelEventArgs(string id)
        {
            _Id = id;
        }

        /// <summary>Идентификатор поезда / видеозаписи</summary>
        public string Id
        {
            get { return _Id; }
        }
    }

    /// <summary>Аргумент события "Удаление видеофрагмента из кольцевого буфера"</summary>
    public class CircularBufferEventArgs : EventArgs
    {
        /// <summary>Идентификатор поезда / видеозаписи</summary>
        private string _Id;

        /// <summary>Конструктор</summary>
        /// <param name="id">Идентификатор поезда / видеозаписи</param>
        public CircularBufferEventArgs(string id)
        {
            _Id = id;
        }

        /// <summary>Идентификатор поезда / видеозаписи</summary>
        public string Id
        {
            get { return _Id; }
        }
    }
    
    /// <summary>Аргумент события "Окончена синхронизация хранилища"</summary>
    public class SyncCompleteEventArgs : EventArgs
    {
        /// <summary>Количество удаленных видеофрагментов</summary>
        private int _ErasedCount;

        /// <summary>Конструктор</summary>
        /// <param name="erased_count">Количество удаленных видеофрагментов</param>
        public SyncCompleteEventArgs(int erased_count)
        {
            _ErasedCount = erased_count;
        }

        /// <summary>Количество удаленных видеофрагментов</summary>
        public int ErasedCount
        {
            get { return _ErasedCount; }
        }
    }

    /// <summary>Аргумент событий, извещающих о действиях с видеозаписями</summary>
    public class VideoRecordEventArgs : EventArgs
    {
        /// <summary>Видеозапись, над которой производится действие</summary>
        private object _Record;

        /// <summary>Конструктор класса</summary>
        /// <param name="rec">Ссылка на видеозапись, над которой производится действие</param>
        public VideoRecordEventArgs(object rec)
        {
            _Record = rec;
        }

        /// <summary>Видеозапись, над которой производится действие</summary>
        public object Record
        {
            get { return _Record; }
        }
    }
}
