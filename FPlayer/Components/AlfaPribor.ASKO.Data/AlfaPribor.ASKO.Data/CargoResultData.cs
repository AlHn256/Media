using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AlfaPribor.ASKO.Data 
{

    /// <summary>Представление данных о грузе</summary>
    public class CargoResultData 
    {
                
        /// <summary>Смещение относительно центра по оси X</summary>
        public int ShiftX { get; set; }
        /// <summary>Центр по Y по длине</summary>
        public int ShiftY { get; set; }
        /// <summary>Центр по Z по длине</summary>
        public int ShiftZ { get; set; }

        /// <summary>Объём груза в мм3</summary>
        public int CargoVolume { get; set; }

        /// <summary>Средняя ширина груза</summary>
        public double CargoWidth { get; set; }
        /// <summary>Средняя высота груза</summary>
        public double CargoHeight { get; set; }
        /// <summary>Итоговая длина груза</summary>
        public double CargoLength { get; set; }

        /// <summary>Максимальная ширина груза</summary>
        public double CargoWidthMax { get; set; }
        /// <summary>Максимальная высота груза</summary>
        public double CargoHeightMax { get; set; }

        /// <summary>Наличие груза</summary>
        public bool CargoExist { get; set; }
        /// <summary>Очистка вагона</summary>
        public bool CargoClear { get; set; }
        /// <summary>Тип груза</summary>
        public int CargoType{ get; set; }

        public int FloorLevel;

        /// <summary>Центр по X (по длине)</summary>
        public int CenterX { get; set; }
        /// <summary>Центр по Y (по ширине)</summary>
        public int CenterY { get; set; }
        /// <summary>Центр по Z (по высоте)</summary>
        public int CenterZ { get; set; }
        
        /// <summary>Средняя длина груза</summary>
        public double XAvg2D { get; set; }
        /// <summary>Средняя высота груза</summary>
        public double YAvg2D { get; set; }

        /// <summary>Центр по X по объёму</summary>
        public double XVolume3D { get; set; }
        /// <summary>Центр по Y по объёму</summary>
        public double YVolume3D { get; set; }
        /// <summary>Центр по Z по объёму</summary>
        public double ZVolume3D { get; set; }

        /// <summary>Смещение относительно центра по объёму по X</summary>
        public double XVolumeDiff3D { get; set; }
        /// <summary>Смещение относительно центра по объёму по Y</summary>
        public double YVolumeDiff3D { get; set; }
        /// <summary>Смещение относительно центра по объёму по Z</summary>
        public double ZVolumeDiff3D { get; set; }

        /// <summary>Несоотвествие по грузу</summary>
        public bool CargoNGBT { get; set; }

        /// <summary>Несоотвествие по грузу</summary>
        public bool CargoMaxHeightNGBT { get; set; }

        public int FrameCount { get; set; }

        #region Cargo Specific

        #region Debug Data

        /// <summary>Смещение относительно центра по оси X</summary>
        [CustomCaptionAttribute("Cмещение по оси Х по модулю*")]
        public int ShiftXMod { get; set; }
        /// <summary>Центр по Y по длине</summary>
        [CustomCaptionAttribute("Смещение по оси Y по модулю*")]
        public int ShiftYMod { get; set; }
        /// <summary>Центр по Z по длине</summary>
        [CustomCaptionAttribute("Смещение по оси Z по модулю*")]
        public int ShiftZMod { get; set; }

        /// <summary>Длина платформы</summary>
        [CustomCaptionAttribute("Длина Платформы")]
        public double PlatformLen { get; set; }

        /// <summary>Длина платформы</summary>
        [CustomCaptionAttribute("Исходная длина платформы")]
        public double SourcePlatformLen { get; set; }

        #endregion

        /// <summary>Центр по Z по длине</summary>
        [CustomCaptionAttribute("Абсолютная координата платформы*")]
        public int XMin2D { get; set; }

        /// <summary>Скорость</summary>
        [CustomCaptionAttribute("Скорость движения платформы*")]
        public double Speed { get; set; }

        /// <summary>Средняя точка по сканам</summary>
        [CustomCaptionAttribute("Средняя точка X*")]
        public double NormalizedAvgPointX { get; set; }

        /// <summary>Средняя точка по сканам</summary>
        [CustomCaptionAttribute("Средняя точка Y*")]
        public double NormalizedAvgPointY { get; set; }


        //Additiition Matriz Data For pyramydeAnalize
        /// <summary>Число кадров на вагон</summary>
        [CustomCaptionAttribute("Определитель матрицы")]
        public double Determinant { get; set; }
        /// <summary>Число кадров на вагон</summary>
        [CustomCaptionAttribute("Наличие смещения")]
        //[CustomCheckAttribute(true, 0, true)]
        public bool HasChanges { get; set; }
        /// <summary>Число кадров на вагон</summary>
        [CustomCaptionAttribute("Медиана")]
        public double Median { get; set; }
        [CustomCaptionAttribute("Среднее")]
        public double AVG { get; set; }
        /// <summary>Число кадров на вагон</summary>
        [CustomCaptionAttribute("Строковое представление матрицы")]
        public string Matrix { get; set; }

        public double ShiftCargoMainCritery { get; set; }

        #endregion

        public CargoResultData() { }

    }

    /// <summary>Представление данных о грузе и сканах</summary>
    public class CargoResultDataEx
    {
        /// <summary>Данные о грузе</summary>
        public CargoResultData Cargo { get; set; }
        public WagonCargoDataShort WagonInfo { get; set; }
        /// <summary>Число вершин груза</summary>
        public int VertexCount { get; set; }

        public CargoResultDataEx() {
            WagonInfo = new WagonCargoDataShort();
        }
    }

    /// <summary>Объект данных груза, точек модели и точек груза</summary>
    public class CargoAndPointData
    {
        public CargoResultData CargoInfo { get; set; }
        //Просто для сравнения грузов и более точного позиционирования модели нужны временные метки 
        public List<Tuple<long, PointF[]>> Vertex { get; set; }
        public Dictionary<int, PointF[]> OversizeVertex { get; set; }
        public int ScanCount { get; set; }
        public bool AsyncGetData { get; set; }
        public long StartCargoTime { get; set; }
        public long EndCargoTime { get; set; }

        public CargoAndPointData()
        {
            CargoInfo = new CargoResultData();
            Vertex = new List<Tuple<long, PointF[]>>();
            OversizeVertex = new Dictionary<int,PointF[]>();
        }
    }

    


    /// <summary>Атрибуты для отображения имени свойств на форме подробнее о расчёте.</summary>
    public class CustomCaptionAttribute : Attribute
    {
        public string CaptionName { get; set; }
        public CustomCaptionAttribute() : base() { }
        public CustomCaptionAttribute(string name) : this() { CaptionName = name; }
    }

}
