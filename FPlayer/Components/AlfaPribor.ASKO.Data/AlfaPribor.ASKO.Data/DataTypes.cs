using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.ASKO.Data
{

    /// <summary>Тип загрузки вагона</summary>
    public enum WagonFillState
    {
        /// <summary>Неопознан</summary>
        Unknown = 0,
        /// <summary>Вагон пустой, наличие груза это негабарит</summary>
        Empty = 1,
        /// <summary>Вагон полный, негабарит, в случае если груз есть</summary>
        Fill = 2
    };

    /// <summary> Действия над удалёнными моделями </summary>
    public enum AvailibleModelActions
    {
        ReturnRemoteModel = 0,
        ReturnComparedModel = 1,
        ReturnComparedModelDataWithoutModel = 2
    };

    /// <summary>Информация о том, кто был инициатором запроса получения результата сравнения вагона и удалённой модели</summary>
    public enum RemoteModelReturnType {
        UserRequestSingleWagon = 0,
        UserRequestCalcTrain = 1,
        AutoRequestSingleWagon = 2,
        AutoRequestCalcTain = 3
    };

    /*
     string result = isLoco ? string.Empty : "Неизвестно";
            switch (validValue) {
                case 0:
                    result = "Обнаружен";
                    break;
                case 1:
                    result = "Иной тип";
                    break;
                case 2:
                    result = "Точность";
                    break;
                case 3:
                    result = "Не обнаружен";
                    break;
            }
         
         */
/// <summary>Результаты поиска вагона на удалённом сервере  </summary>
    public enum RemoteWagonValidResult {
        InvNumbIsEmpty=-2,
        Undefine=-1,
        FindAndCalc=0,
        IncompareType=1,
        IncorrectQuality=2,
        NotFound=3,
        ModelError=4
    };

    //public enum DevState : int
    //{
    //    /// <summary>Не известно</summary>
    //    unknown = -1,
    //    /// <summary>Нет связи</summary>
    //    offline = 0,
    //    /// <summary>Есть связь</summary>
    //    online = 1,
    //    /// <summary>Нет видеосигнала (только для телекамер)</summary>
    //    novideo = 2,
    //    /// <summary>Выключено / не активно</summary>
    //    none = 3,
    //    /// <summary>Изменение настроек</summary>
    //    changeSettings = 4
    //};


}
