using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoCameras4
{
    /// <summary>Аргументы события "Изменился статус клиента Panasonic видео"</summary>
    public class PanasonicVideoStatusEventArgs : EventArgs
    {
        PanasonicVideoStatus _Status;

        /// <summary>Конструктор</summary>
        /// <param name="status">Статус</param>
        public PanasonicVideoStatusEventArgs(PanasonicVideoStatus status) : base() { _Status = status; }

        /// <summary>Статус</summary>
        public PanasonicVideoStatus Status { get { return _Status; } }
    }

    /// <summary>Аргументы события "Кадр видео от камеры Axis"</summary>
    public class PanasonicVideoFrameEventArgs : EventArgs
    {
        byte[] _Frame;

        /// <summary>Конструктор</summary>
        /// <param name="frame">Данные кадра</param>
        public PanasonicVideoFrameEventArgs(byte[] frame) : base() { _Frame = frame; }

        /// <summary>Данные кадра</summary>
        public byte[] Frame { get { return _Frame; } }

    }
}
