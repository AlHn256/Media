using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoCameras4
{
    /// <summary>Аргументы события "Изменился статус клиента Axis видео"</summary>
    public class AxisVideoStatusEventArgs : EventArgs
    {
        AxisVideoStatus _Status;

        /// <summary>Конструктор</summary>
        /// <param name="status">Статус</param>
        public AxisVideoStatusEventArgs(AxisVideoStatus status) : base()
        {
            _Status = status;
        }

        /// <summary>Статус</summary>
        public AxisVideoStatus Status
        {
            get { return _Status; }
        }
    }

    /// <summary>Аргументы события "Кадр видео от камеры Axis"</summary>
    public class AxisVideoFrameEventArgs : EventArgs
    {
        private byte[] _Frame;

        /// <summary>Конструктор</summary>
        /// <param name="frame">Данные кадра</param>
        public AxisVideoFrameEventArgs(byte[] frame) : base()
        {
            _Frame = frame;
        }

        /// <summary>Данные кадра</summary>
        public byte[] Frame
        {
            get { return _Frame; }
        }
    }
}
