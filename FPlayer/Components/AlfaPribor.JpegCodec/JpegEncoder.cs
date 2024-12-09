using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace AlfaPribor.JpegCodec
{

    public class JpegEncoder
    {

        #region Import

        //Открыть кодек
        [DllImport("MSVFW32.dll"), PreserveSig]
        private static extern int ICOpen(int fccType, int fccHandler, ICMODE wMode);

        [DllImport("MSVFW32.dll")]
        private static extern int ICCompress(int hic, int dwFlags, ref BITMAPINFOHEADER lpbiFormat, byte[] lpData,
                                            ref BITMAPINFOHEADER lpbi, byte[] lpBits,
                                            ref int lpckid, ref int lpdwFlags, int lFrame, int dwFrameSize, int dwQuality,
                                            byte[] lpbiPrev, byte[] lpPrev);
        //Закрыть кодек
        [DllImport("MSVFW32.dll")]
        private static extern int ICClose(int hic);

        //Конфигурация кодека
        [DllImport("MSVFW32.dll")]
        private static extern int ICSendMessage(int hic, uint wMsg, int dw1, int dw2);

        //private static extern int ICConfigure(int hic, IntPtr hwnd);

        #endregion

        #region Declaration

        public enum ICMODE
        {
            ICMODE_COMPRESS = 1, ICMODE_DECOMPRESS = 2, ICMODE_FASTDECOMPRESS = 3,
            ICMODE_QUERY = 4, ICMODE_FASTCOMPRESS = 5, ICMODE_DRAW = 8
        };

        public struct BITMAPINFOHEADER
        {
            public uint biSize;
            public int biWidth;
            public int biHeight;
            public Int16 biPlanes;
            public Int16 biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        }

        public struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmiHeader;
            public RGBQUAD[] bmiColors;
        };

        public struct RGBQUAD
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        };

        //Заголовок Bitmap
        BITMAPINFOHEADER bmih_out = new BITMAPINFOHEADER();
        BITMAPINFOHEADER bmih_in = new BITMAPINFOHEADER();

        /// <summary>HIC кодека</summary>
        protected int hic;

        static int mjpg = 0x67706A6D; //0x47504A4D;//0x67706A6D;
        static int type = 0x63646976;// 1667524982;//0x63646976

        #endregion

        #region Events

        /// <summary>Делегат события сжатия кадра</summary>
        /// <param name="frame">Данные сжатого кадра</param>
        public delegate void DelegateEventEncode(byte[] frame);
        /// <summary>Событие компрессии</summary>
        public event DelegateEventEncode EventEncode;

        #endregion

        public JpegEncoder()
        {
        }

        /// <summary>Открыть Jpeg-компрессор Windows Compression Manager</summary>
        /// <param name="width">Ширина изображения</param>
        /// <param name="height">Высота изображения</param>
        public bool OpenVCMEncoder()
        {
            hic = 0;
            hic = ICOpen(type, mjpg, ICMODE.ICMODE_COMPRESS);
            if (hic == 0) return false;
            return true;
        }

        /// <summary>Закрыть Jpeg-компрессор Windows Compression Manager</summary>
        public void CloseVCMDecoder()
        {
            ICClose(hic);
        }

        /// <summary>Сжатие кадра VCM кодеком</summary>
        /// <param name="in_data">Входные данные кадра в формате RGB24</param>
        /// <param name="out_data">Выходные данные jpeg</param>
        /// <param name="sizeimage">Размер сжатых данных</param>
        /// <returns>Результат операции</returns>
        public bool EncodeFrame(byte[] in_data, int width, int height, ref byte[] out_data)
        {
            int res = 0;
            if (hic == 0) return false;
            try
            {
                int dwCkID = 0;
                int dwCompFlags = 0;
                int flag = 0x00000001;
                //Входной заголовок
                bmih_in.biSize = 40;
                bmih_in.biWidth = width;
                bmih_in.biHeight = height;
                bmih_in.biBitCount = 24;
                bmih_in.biCompression = 0;
                bmih_in.biPlanes = 1;
                bmih_in.biSizeImage = in_data.Length;
                //Выходной заголовок
                bmih_out.biSize = 40;
                bmih_out.biWidth = width;
                bmih_out.biHeight = height;
                bmih_out.biBitCount = 24;
                bmih_out.biCompression = mjpg;
                bmih_out.biPlanes = 1;
                res = ICCompress(hic, flag, ref bmih_out, out_data, ref bmih_in, in_data, ref dwCkID, ref dwCompFlags,
                                 0, 0, 0, null, null);
                Array.Resize(ref out_data, bmih_out.biSizeImage);
                if (EventEncode != null) EventEncode(out_data);
            }
            catch { return false; };
            return true;
        }

        ///<summary>Сжатие в jpeg при помощи Windows Presentation Core 
        ///из состава .NET Framework 3.0</summary>
        /// <param name="in_data">Входные данные</param>
        /// <param name="out_data">Выходные данные RGB24</param>
        public void EncodeWPF(byte[] in_data, int width, int height, ref byte[] out_data)
        {

            int w = width;
            int h = height;

            BITMAPINFOHEADER bmih = new BITMAPINFOHEADER();
            bmih.biSize = 40;
            bmih.biWidth = w;
            bmih.biHeight = h;
            bmih.biPlanes = 1;
            bmih.biBitCount = 24;
            bmih.biCompression = 0;
            bmih.biSizeImage = w * h * 3;
            bmih.biXPelsPerMeter = 100;
            bmih.biYPelsPerMeter = 100;
            bmih.biClrUsed = 0;
            bmih.biClrImportant = 0;
            byte[] bmi_header = new byte[40];
            bmi_header = StructureToByteArray(bmih);

            byte[] fileheader = new byte[14];
            fileheader[0] = 0x42;
            fileheader[1] = 0x4D;
            fileheader[2] = 0x46;
            fileheader[3] = 0x1C;
            fileheader[4] = 0x03;
            fileheader[5] = 0x0;
            fileheader[6] = 0x0;
            fileheader[7] = 0x0;
            fileheader[8] = 0x0;
            fileheader[8] = 0x0;
            fileheader[9] = 0x0;
            fileheader[10] = 0x36;
            fileheader[11] = 0x0;
            fileheader[12] = 0x0;
            fileheader[13] = 0x0;

            int size = w * h * 3;

            MemoryStream ms = new System.IO.MemoryStream(14 + 40 + size);
            ms.Write(fileheader, 0, 14);
            ms.Write(bmi_header, 0, 40);
            ms.Write(in_data, 0, size);

            //MemoryStream ms = new MemoryStream(in_data);
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            try
            {
                encoder.Frames.Add(BitmapFrame.Create(ms));
                MemoryStream out_ms = new MemoryStream();
                encoder.Save(out_ms);
                out_data = out_ms.GetBuffer();
                if (EventEncode != null) EventEncode(out_data);
            }
            catch { };
        }

        /// <summary>Вызов окна настройки кодека</summary>
        /// <param name="handle">Handle родительского окна</param>
        public static void ConfigEncoder(IntPtr handle)
        {
            int hic = ICOpen(type, mjpg, ICMODE.ICMODE_COMPRESS);
            uint d_user = 0x4000;
            if (hic != 0) ICSendMessage(hic, d_user + 0x1000 + 10, 0, 0);
        }

        static byte[] StructureToByteArray(object obj)
        {
            int len = System.Runtime.InteropServices.Marshal.SizeOf(obj);
            byte[] arr = new byte[len];
            IntPtr ptr = System.Runtime.InteropServices.Marshal.AllocHGlobal(len);
            System.Runtime.InteropServices.Marshal.StructureToPtr(obj, ptr, true);
            System.Runtime.InteropServices.Marshal.Copy(ptr, arr, 0, len);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(ptr);
            return arr;
        }


    }

}
