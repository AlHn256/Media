using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.ASKO.Data
{
    public class CargoRemoteCompareResultData
    {
        /// <summary>Идентификатор вагона</summary>
        public int WagonID { get; set; }
        /// <summary>Идентификатор удалённого сервера</summary>
        public int RemoteServerId { get; set; }
        /// <summary>Идентификатор поезда на удалённом сервере</summary>
        public int RemoteTrainId { get; set; }
        /// <summary>Номер поезда на удалённом сервере</summary>
        public string RemoteTrainNumber { get; set; }
        /// <summary>Индекс поезда на удалённом сервере</summary>
        public string RemoteTrainIndex { get; set; }
        /// <summary>Серийный номер вагона на удалённом сервере</summary>
        public int RemoteWagonSn { get; set; }
        /// <summary>Идентификатор вагона на удалённом сервере</summary>
        public int RemoteWagonId { get; set; }
        /// <summary>Дата прохождения вагона на удалённом сервере</summary>
        public DateTime RemoteWagonDate { get; set; }
        /// <summary>Смещение груза по оси Х</summary>
        public int DiffShiftX { get; set; }
        /// <summary>Смещение груза по оси Y</summary>
        public int DiffShiftY { get; set; }
        /// <summary>Смещение груза по оси Z</summary>
        public int DiffShiftZ { get; set; }
        /// <summary>Изменение объёма груза</summary>
        public int DiffVolume { get; set; }
        /// <summary>Изменение ширины груза</summary>
        public int DiffWidth { get; set; }
        /// <summary>Смещение высоты груза</summary>
        public int DiffHeght { get; set; }
        /// <summary>Изменение длины груза</summary>
        public int DiffLength { get; set; }
        /// <summary>Изменение максимаьлной ширины груза</summary>
        public int DiffWidthMax { get; set; }
        /// <summary>Изменнение максимальной высоты груза</summary>
        public int DiffHeightMax { get; set; }
        /// <summary>Изменение флага наличия груза</summary>
        public bool DiffCargoExists { get; set; }
        /// <summary>Изменение флага очистки вагона</summary>
        public bool DiffCargoClear { get; set; }
        /// <summary>Флаг использования приведённого сравнения</summary>
        public bool IsRelationModel { get; set; }
        /// <summary>Изменение скорости движения вагона</summary>
        public float DiffSpeed { get; set; }
        /// <summary>Флаг валидности, полученных данных сравнения груза( По сути результат поиска и расчёта 0 - наёден и рассчитан, осальное либо расчёт не проводился, либо ошибки при определении соответствия типов)</summary>
        //Правильное название RemoteSearchState
        public int RemoteValid { get; set; }
        /// <summary>Дата выполнения сравнения груза, если не совпадает с RemoteWagonDate, значит сравнение ваполнялось повторно</summary>
        public DateTime CompareDate { get; set; }
        /// <summary>Идентификатор оператора, инициировавшего сравнение</summary>
        public int OperatorID { get; set; }

        public CargoRemoteCompareResultData() {
            RemoteTrainNumber = string.Empty;
            RemoteTrainIndex = string.Empty;
            RemoteValid = (int)RemoteWagonValidResult.Undefine;

        }

        public CargoRemoteCompareResultData(CargoResultData diffData):this()
        {
            DiffShiftX = diffData.ShiftX;
            DiffShiftY = diffData.ShiftY;
            DiffShiftZ = diffData.ShiftZ;
            DiffVolume = diffData.CargoVolume;
            DiffWidth = (int)diffData.CargoWidth;
            DiffHeght = (int)diffData.CargoHeight;
            DiffLength = (int)diffData.CargoLength;

            DiffWidthMax = (int)diffData.CargoWidthMax;
            DiffHeightMax = (int)diffData.CargoHeightMax;
            DiffSpeed = (float)diffData.Speed;
            DiffCargoExists = diffData.CargoClear;
            DiffCargoExists = diffData.CargoExist;
        }

        public CargoRemoteCompareResultData(WagonData wagonData):this()
        {
            DiffShiftX = wagonData.DiffShiftX;
            DiffShiftY = wagonData.DiffShiftY;
            DiffShiftZ = wagonData.DiffShiftZ;
            DiffVolume = wagonData.DiffVolume;
            DiffWidth = (int)wagonData.DiffWidth;
            DiffHeght = (int)wagonData.DiffHeght;
            DiffLength = (int)wagonData.DiffLength;

            DiffWidthMax = (int)wagonData.DiffWidthMax;
            DiffHeightMax = (int)wagonData.DiffHeightMax;
            DiffSpeed = (float)wagonData.DiffSpeed;
            DiffCargoExists = wagonData.DiffCargoClear;
            DiffCargoExists = wagonData.DiffCargoExists;
        }

    }
}
