using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoCameras4
{

    /// <summary>Аргументы события "Изменился статус клиента HikVision видео"</summary>
    public class HikVisionVideoStatusEventArgs : EventArgs
    {

        HikVisionVideoStatus status;

        /// <summary>Конструктор</summary>
        /// <param name="status">Статус</param>
        public HikVisionVideoStatusEventArgs(HikVisionVideoStatus status) : base() { this.status = status; }

        /// <summary>Статус</summary>
        public HikVisionVideoStatus Status { get { return status; } }

    }

    /// <summary>Аргументы события "Кадр видео от камеры HikVision"</summary>
    public class HikVisionVideoFrameEventArgs : EventArgs
    {
        
        byte[] frame;

        /// <summary>Конструктор</summary>
        /// <param name="frame">Данные кадра</param>
        public HikVisionVideoFrameEventArgs(byte[] frame) : base() { this.frame = frame; }

        /// <summary>Данные кадра</summary>
        public byte[] Frame { get { return frame; } }

    }
}
