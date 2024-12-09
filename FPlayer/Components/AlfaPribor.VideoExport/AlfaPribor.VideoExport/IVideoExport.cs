using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AlfaPribor.VideoExport
{
    /// <summary>Интерфейс объектов, осуществляющих формирование видеокадров и запись их в видеопоток</summary>
    public interface IVideoExport
    {
        #region Methods

        /// <summary>Подготавливает изображение кадра</summary>
        void MakeFrame();

        /// <summary>Помещает изображение кадра в контейнер для хранения кадров</summary>
        void SaveFrame();

        /// <summary>Помещает изображение кадра в контейнер для хранения кадров</summary>
        /// <param name="frame">Изображение кадра</param>
        void SaveFrame(Image frame);

        #endregion

        #region Properties

        /// <summary>Описывает границы кадра результирующего изображения</summary>
        RectangleF FrameRect
        {
            get;
            set;
        }

        /// <summary>Список графических объектов, размещаемых в кадре</summary>
        IList<BaseGraphicRegion> Regions
        {
            get;
            set;
        }

        /// <summary>Графическое изображение кадра</summary>
        /// <remarks>Формируется после вызова метода PrapareFrame</remarks>
        Image Frame
        {
            get;
        }

        /// <summary>Скорость воспроизведения потока (кадров/сек)</summary>
        double FPS
        {
            get;
            set;
        }

        #endregion
    }
}
