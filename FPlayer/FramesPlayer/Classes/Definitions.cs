using System;
using System.Collections.Generic;
using System.Text;

namespace FramesPlayer
{

    /// <summary>Тип хранения кадра</summary>
    public enum ImageType
    {
        /// <summary>Неизвестно</summary>
        Unknown = 0,
        /// <summary>Jpeg-сжатие</summary>
        Jpeg = 1,
        /// <summary>Тепловизионная raw8</summary>
        Raw8 = 2
    }

    /// <summary>Тип видео канала</summary>
    public enum ChannelType
    {
        /// <summary>Неизвестен</summary>
        None = 0,
        /// <summary>Телекамера</summary>
        Telecamera = 1,
        /// <summary>Тепловизор</summary>
        Thermovision = 2
    };

    /// <summary>Тип телекамеры</summary>
    public enum CameraType
    {
        /// <summary>Аналоговая</summary>
        Analog = 0,
        /// <summary>Телекамера Axis</summary>
        Axis = 1,
        /// <summary>Тепловизор Panasonic</summary>
        Panasonic = 2
    };

    /// <summary>Формат сохранения изображения</summary>
    public enum ImageFormat
    {
        /// <summary>Битмэп</summary>
        Bitmap = 0,
        /// <summary>Jpeg</summary>
        Jpeg = 1
    };

    /// <summary>Тип палитры тепловизора</summary>
    public enum Palettes
    {
        /// <summary>Градации серого</summary>
        Gray = 0,
        /// <summary>"Радуга"</summary>
        Rainbow = 1,
        /// <summary>"Металл"</summary>
        RedHot = 2,
    }

    /// <summary>Типы графических элементов окна тепловизора</summary>
    public enum GraphicElement
    {
        /// <summary>Границы анализа</summary>
        DrawBorders = 1,
        /// <summary>Границы калибровочного типа</summary>
        DrawCalibre = 2,
        /// <summary>Уровень горизонта</summary>
        DrawHorizon = 3,
        /// <summary>Отображать надписи</summary>
        DrawCaption = 4,
        /// <summary>Функция распределения температур</summary>
        DrawFunction = 5,
        /// <summary>Уровень загрузки</summary>
        DrawLevel = 6,
        /// <summary>Шкала температур</summary>
        DrawScale = 7,
        /// <summary>Уровень загрузки по документам</summary>
        DrawLevelDoc = 8,
        /// <summary>Температура в указанной точке</summary>
        DrawTemperature = 9,
        /// <summary>Скорость отрисовки кадров</summary>
        DrawFPS = 10,
        /// <summary>Скорость получения кадров</summary>
        DrawIPS = 11
    }

    /// <summary>Тип кодека MJPEG</summary>
    public enum CodecType
    {
        /// <summary>Video Compression Manager кодек (Morgan, PICVideo и т.д.)</summary>
        VCM = 0,
        /// <summary>Кодек на основе Windows Presentation Foundation</summary>
        WPF = 1,
        /// <summary>Кодек Intel Performance Primitives</summary>
        IPP = 2
    }

}
