using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoCameras4
{

    /// <summary>Аргументы события "Изменился статус клиента HikVision видео"</summary>
    public class EvidenceVideoStatusEventArgs : EventArgs
    {

        EvidenceVideoStatus status;

        /// <summary>Конструктор</summary>
        /// <param name="status">Статус</param>
        public EvidenceVideoStatusEventArgs(EvidenceVideoStatus status) : base() { this.status = status; }

        /// <summary>Статус</summary>
        public EvidenceVideoStatus Status { get { return status; } }

    }

    /// <summary>Аргументы события "Кадр видео от камеры HikVision"</summary>
    public class EvidenceVideoFrameEventArgs : EventArgs
    {
        
        byte[] frame;

        /// <summary>Конструктор</summary>
        /// <param name="frame">Данные кадра</param>
        public EvidenceVideoFrameEventArgs(byte[] frame) : base() { this.frame = frame; }

        /// <summary>Данные кадра</summary>
        public byte[] Frame { get { return frame; } }

    }
}
