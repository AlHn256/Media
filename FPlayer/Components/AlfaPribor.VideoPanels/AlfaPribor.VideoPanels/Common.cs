using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoPanels
{
    
    /// <summary>Доступные варианты интерфейса (число видеоокон)</summary>
    public enum VideoStyle
    {
        /// <summary>Видеоокна отсутствуют</summary>
        None = 0,
        /// <summary>Одно видеокно</summary>
        One = 1,
        /// <summary>Два видеоокна</summary>
        Double = 2,
        /// <summary>Три видеоокна в двух строках</summary>
        TripleTwoRows = 3,
        /// <summary>Четыре видеоокна</summary>
        Quadro = 4,
        /// <summary>Пять видеоокон</summary>
        Pentad = 5,
        /// <summary>Шесть видеоокон</summary>
        Hexagon = 6,
        /// <summary>Три видеоокна одна строка (повернутые)</summary>
        TripleOneRow = 7,
        /// <summary>Два видеоокна с поворотом</summary>
        DoubleRotation = 8,
        /// <summary>Шесть видеоокон повернутых картинка в картинке</summary>
        HexagonRotation = 9,
        /// <summary>Пять видеоокон повернутых картинка в картинке, средней малой картинки нет</summary>
        PentadRotation = 10
    }
    
    /// <summary>Названия кнопок</summary>
    public enum ToolbarButton
    {
        /// <summary>Печать кадра</summary>
        PrintImage = 1,
        /// <summary>Сохранение кадра</summary>
        SaveImage = 2,
        /// <summary>Мастер канал</summary>
        Master = 3,
        /// <summary>Увеличение/уменьшение окна окна</summary>
        Maximize = 4,
        /// <summary>
        /// Произвольный тип
        /// </summary>
        Custom = 5
    }
    
    /// <summary>Режим</summary>
    public enum ScreenMode
    {
        /// <summary>Полиэкран</summary>
        PolyScreen = 1,
        /// <summary>Одно видеоокна</summary>
        OneScreen = 2
    }
    
    /// <summary>Названия картинок кнопок</summary>
    enum ToolbarImagesNames
    {
        /// <summary>Печать кадра</summary>
        PrintImage = 1,
        /// <summary>Сохранение кадра</summary>
        SaveImage = 2,
        /// <summary>Мастер канал</summary>
        Master = 3,
        /// <summary>Увеличение окна</summary>
        Maximize = 4
    }
    
    /// <summary>Единицы измерения</summary>
    public enum MestureUnit
    {
        /// <summary>Проценты</summary>
        Persantage = 0,
        /// <summary>Пиксели</summary>
        Pixel = 1
    }
       
}
