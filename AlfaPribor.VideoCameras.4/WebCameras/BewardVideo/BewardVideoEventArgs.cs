using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoCameras4
{

    /// <summary>Аргументы события "Изменился статус клиента Beward видео"</summary>
    public class BewardVideoStatusEventArgs : EventArgs
    {

        BewardVideoStatus _Status;

        /// <summary>Конструктор</summary>
        /// <param name="status">Статус</param>
        public BewardVideoStatusEventArgs(BewardVideoStatus status) : base() { _Status = status; }

        /// <summary>Статус</summary>
        public BewardVideoStatus Status { get { return _Status; } }

    }

    /// <summary>Аргументы события "Кадр видео от камеры Balser"</summary>
    public class BewardVideoFrameEventArgs : EventArgs
    {
        
        byte[] _Frame;

        /// <summary>Конструктор</summary>
        /// <param name="frame">Данные кадра</param>
        public BewardVideoFrameEventArgs(byte[] frame) : base() { _Frame = frame; }

        /// <summary>Данные кадра</summary>
        public byte[] Frame { get { return _Frame; } }

    }
}
