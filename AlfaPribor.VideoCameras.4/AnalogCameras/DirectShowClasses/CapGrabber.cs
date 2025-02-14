using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace AnalogNamespace
{

    /// <summary>Класс захвата изображений</summary>
    internal class CapGrabber:ISampleGrabberCB,INotifyPropertyChanged
    {

        public CapGrabber()
        {
            
        }

        public IntPtr Map { get; set; }
        
        public int Width
        {
            get { return m_Width; }
            set
            {
                m_Width = value;
                OnPropertyChanged("Width");
            }
        }

        public int Height
        {
            get { return m_Height; }
            set
            {
                m_Height = value;
                OnPropertyChanged("Height");
            }
        }

        int m_Height = default(int);

        int m_Width = default(int);

        #region ISampleGrabberCB Members

        public int SampleCB(double sampleTime, IntPtr sample)
        {
            return 0;
        }

        byte[] buff = null;

        public int BufferCB(double sampleTime, IntPtr buffer, int bufferLen)
        {
            if (buff == null) return 0;
            CopyMemory(buff, buffer, bufferLen);
            OnNewFrameArrived(buff);
            return 0;
        }

        #endregion

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(byte[] Destination, IntPtr Source, int Length);

        public delegate void DelegateEventNewFrame(byte[] frame);
        public event DelegateEventNewFrame NewFrameArrived;

        void OnNewFrameArrived(byte[] frame)
        {
            if (NewFrameArrived != null)
                NewFrameArrived(frame);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                buff = new byte[m_Width * m_Height * 3];
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}
