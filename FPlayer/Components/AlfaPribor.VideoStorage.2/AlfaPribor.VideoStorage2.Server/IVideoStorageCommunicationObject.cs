using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace AlfaPribor.VideoStorage.Server
{
    /// <summary>Интерфейс класса коммуникационных объектов, обменивающихся данными по сети</summary>
    public interface IVideoStorageCommunicationObject
    {
        #region Methods

        /// <summary>Открывает коммуникационный объект для обмена данными по сети</summary>
        void Open();

        /// <summary>Открывает коммуникационный объект для обмена данными по сети в течении заданного интервала времени</summary>
        /// <param name="timeout">Временной интервал, в течении которого предпринимается попытка открытия коммуникационнгого объекта</param>
        /// <exception cref="System.TimeoutException">Истекло время ожидания открытия коммуникационного оъекта</exception>
        void Open(TimeSpan timeout);

        /// <summary>Закрывает коммуникационный объект</summary>
        void Close();

        /// <summary>Закрывает коммуникационный объект в течении заданного интервала времени</summary>
        /// <param name="timeout">Временной интервал, в течении которого предпринимается попытка закрытия коммуникационнгого объекта</param>
        /// <exception cref="System.TimeoutException">Истекло время ожидания открытия коммуникационного оъекта</exception>
        void Close(TimeSpan timeout);

        /// <summary>Начинает асинхронную операцию открытия коммуникационного объекта</summary>
        /// <param name="callback">Метод обратного вызова, который будет вызван по окончании операции открытия</param>
        /// <param name="state">Объект с дополнительными сведениями об операции открытия коммуникационного объекта</param>
        /// <returns>Результат выполнения операции</returns>
        IAsyncResult BeginOpen(AsyncCallback callback, object state);

        /// <summary>Начинает асинхронную операцию открытия коммуникационного объекта в течении заданного интервала времени</summary>
        /// <param name="timeout">Время ожидания открытия коммуникационного объекта</param>
        /// <param name="callback">Метод обратного вызова, который будет вызван по окончании операции открытия</param>
        /// <param name="state">Объект с дополнительными сведениями об операции открытия коммуникационного объекта</param>
        /// <exception cref="System.TimeoutException">Истекло время ожидания открытия коммуникационного оъекта</exception>
        /// <returns>Результат выполнения операции</returns>
        IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state);

        /// <summary>Начинает асинхронную операцию закрытия коммуникационного объекта</summary>
        /// <param name="callback">Метод обратного вызова, который будет вызван по окончании операции закрытия</param>
        /// <param name="state">Объект с дополнительными сведениями об операции закрытия коммуникационного объекта</param>
        /// <returns>Результат выполнения операции</returns>
        IAsyncResult BeginClose(AsyncCallback callback, object state);

        /// <summary>Начинает асинхронную операцию закрытия коммуникационного объекта в течение заданного интервала времени</summary>
        /// <param name="timeout">Время ожидания закрытия коммуникационного объекта</param>
        /// <param name="callback">Метод обратного вызова, который будет вызван по окончании операции закрытия</param>
        /// <param name="state">Объект с дополнительными сведениями об операции закрытия коммуникационного объекта</param>
        /// <exception cref="System.TimeoutException">Истекло время ожидания закрытия коммуникационного оъекта</exception>
        /// <returns>Результат выполнения операции</returns>
        IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state);

        /// <summary>Завершает асинхронную операцию открытия коммуникационного объекта</summary>
        /// <param name="result">Результат операции открытия</param>
        void EndOpen(IAsyncResult result);

        /// <summary>Завершает асинхронную операцию закрытия коммуникационного объекта</summary>
        /// <param name="result">Результат операции закрытия</param>
        void EndClose(IAsyncResult result);

        #endregion

        #region Properties

        /// <summary>Сетевой адрес коммуникационного объекта</summary>
        string Address { get; set; }

        /// <summary>Номер сетевого порта</summary>
        int Port { get; set; }

        /// <summary>Коммуникационное состояние объекта</summary>
        CommunicationState State { get; }

        #endregion

        #region Events

        /// <summary>Событие "Коммуникационный объект открыт"</summary>
        event EventHandler Opened;

        /// <summary>Событие "Коммуникационный объект закрыт"</summary>
        event EventHandler Closed;

        /// <summary>Событие "Коммуникационный объект неисправен"</summary>
        event EventHandler Faulted;

        #endregion
    }
}
