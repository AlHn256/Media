using System;
using System.Collections.Generic;
using System.Text;

namespace AlfaPribor.VideoCameras4
{
    /// <summary>Базовый интерфейс "Видеокамера"</summary>
    public interface IVideoCamera
    {
        /// <summary>Идентификатор камеры (номер видеопотока)</summary>
        int Id { get; }

        /// <summary>Активация</summary>
        bool Active { get; set; }

        /// <summary>Текущий статус</summary>
        VideoCameraStatus Status { get; }

        /// <summary>Настройки</summary>
        VideoCameraSettings Settings { get; set; }

        /// <summary>Параметры совместимости</summary>
        VideoCameraCompat Compat { get; }

        // Получение и установка видеопараметров
        /// <summary>Яркость</summary>
        int Brightness { get; set; }
        /// <summary>Контраст</summary>
        int Contrast { get; set; }
        /// <summary>Оттенок</summary>
        int Hue { get; set; }
        /// <summary>Насыщенность</summary>
        int Saturation { get; set; }
        /// <summary>Гамма</summary>
        int Gamma { get; set; }
        /// <summary>Резкость</summary>
        int Sharpness { get; set; }
        /// <summary>Выдержка (время затвора)</summary>
        int Shutter { get; set; }
        /// <summary>Компрессия</summary>
        int Compression { get; set; }
    }
}
