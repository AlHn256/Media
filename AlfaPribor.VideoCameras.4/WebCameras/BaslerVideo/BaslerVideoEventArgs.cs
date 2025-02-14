using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoCameras4
{

    /// <summary>Аргументы события "Изменился статус клиента Balser видео"</summary>
    public class BaslerVideoStatusEventArgs : EventArgs
    {

        BaslerVideoStatus _Status;

        /// <summary>Конструктор</summary>
        /// <param name="status">Статус</param>
        public BaslerVideoStatusEventArgs(BaslerVideoStatus status) : base() { _Status = status; }

        /// <summary>Статус</summary>
        public BaslerVideoStatus Status { get { return _Status; } }

    }

    /// <summary>Аргументы события "Кадр видео от камеры Balser"</summary>
    public class BaslerVideoFrameEventArgs : EventArgs
    {
        
        byte[] _Frame;

        /// <summary>Конструктор</summary>
        /// <param name="frame">Данные кадра</param>
        public BaslerVideoFrameEventArgs(byte[] frame) : base() { _Frame = frame; }

        /// <summary>Данные кадра</summary>
        public byte[] Frame { get { return _Frame; } }

    }
}
