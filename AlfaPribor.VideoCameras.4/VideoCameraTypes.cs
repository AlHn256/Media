using System;
using System.Collections.Generic;
using System.Text;

namespace AlfaPribor.VideoCameras4
{

    /// <summary>Тип видеокамеры</summary>
    public enum VideoCameraType
    {
        /// <summary>Неизвестно</summary>
        Unknown = 0,
        /// <summary>Аналоговая</summary>
        Analog,
        /// <summary>IP камера</summary>
        IP,
        /// <summary>Тепловизор</summary>
        Termo           
    }

    /// <summary>Бит на пиксель (в основном для тепловизоров, для обычных камер 24 bpp)</summary>
    public enum VideoCameraBpp 
    { 
        /// <summary>8 бит/пиксель</summary>
        bpp8 = 8, 
        /// <summary>16 бит/пиксель</summary>
        bpp16 = 16,
        /// <summary>24 бита/пиксель</summary>
        bpp24 = 24 
    }

    /// <summary>Формат кадра, передаваемый телекамерой</summary>
    public enum VideoCameraFormatFrame
    {
        /// <summary>Битовая карта</summary>
        Bitmap = 1,
        /// <summary>JPEG (JFIF)</summary>
        Jpeg = 2
    }

    /// <summary>Параметры совместимости (что можно настраивать в камере)</summary>
    public struct VideoCameraCompat
    {
        /// <summary>Яркость</summary>
        public bool Brightness;
        /// <summary>Контраст</summary>
        public bool Contrast;
        /// <summary>Оттенок</summary>
        public bool Hue;
        /// <summary>Насыщенность</summary>
        public bool Saturation;
        /// <summary>Гамма</summary>
        public bool Gamma;
        /// <summary>Резкость</summary>
        public bool Sharpness;
        /// <summary>Время экспозиции</summary>
        public bool Shutter;
        /// <summary>Степень сжатия</summary>
        public bool Compression;
        /// <summary>Список возможных разрешений (например: 1280x960;640x480;320x240)</summary>
        public string Resolutions;
        /// <summary>Темп передачи</summary>
        public bool FPS;
        /// <summary>Формат кадра</summary>
        public VideoCameraFormatFrame format;  
    }

    /// <summary>Статус видеокамеры (наличие сигнала или связи)</summary>
    public enum VideoCameraStatus 
    {
        /// <summary>Неизвестно</summary>
        Unknown = -1,
        /// <summary>Нет связи (или видеосигнала?)</summary>
        Offline = 0,
        /// <summary>Есть связь (или видеосигнал?)</summary>
        Online = 1     
    }

    /// <summary>Видеопараметры</summary>
    public struct VideoParams
    {
        /// <summary>Яркость</summary>
        public int Brightness;
        /// <summary>Контраст</summary>
        public int Contrast;
        /// <summary>Оттенок</summary>
        public int Hue;
        /// <summary>Насыщенность</summary>
        public int Saturation;
        /// <summary>Гамма</summary>
        public int Gamma;
        /// <summary>Четкость</summary>
        public int Sharpness;
        /// <summary>Выдержка</summary>
        public int Shutter;
        /// <summary>Сжатие</summary>
        public int Compression;
        /// <summary>Скорость трансляции кадров в секунду</summary>
        public int FPS;
    }

}
