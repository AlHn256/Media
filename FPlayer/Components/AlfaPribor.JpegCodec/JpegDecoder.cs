using System;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.IO;

namespace AlfaPribor.JpegCodec
{

    /// <summary>Класс декодирования JPEG кодеком VfW</summary>
    public class JpegDecoder
    {

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

        //hex flags for ICDecompress

        /// <summary>don't draw just buffer (hurry up!)</summary>
        const uint ICDECOMPRESS_HURRYUP = 0x80000000;
        /// <summary>don't draw just update screen</summary>
        const uint ICDECOMPRESS_UPDATE = 0x40000000;
        /// <summary>this frame is before real start</summary>
        const uint ICDECOMPRESS_PREROLL = 0x20000000;
        /// <summary>repeat last frame</summary>
        const uint ICDECOMPRESS_NULLFRAME = 0x10000000;
        /// <summary>this frame is not a key frame</summary>
        const uint ICDECOMPRESS_NOTKEYFRAME = 0x08000000;    

        static readonly int DRV_USER = 0x4000;
        static readonly int ICM_USER = (DRV_USER + 0x0000);
        static readonly int ICM_DECOMPRESS_BEGIN = (ICM_USER + 12);  // start a series of decompress calls
        static readonly int ICM_DECOMPRESS = (ICM_USER + 13);        // decompress a frame
        static readonly int ICM_DECOMPRESS_END = (ICM_USER + 14);

        //Открыть кодек
        [DllImport("MSVFW32.dll"), PreserveSig]
        public static extern uint ICOpen(int fccType, int fccHandler, ICMODE wMode);

        [DllImport("MSVFW32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ICDecompress(uint hic, uint dwFlags, ref BITMAPINFOHEADER lpbiFormat, byte[] lpData,
                                              ref BITMAPINFOHEADER lpbi, byte[] lpBits);

        //Закрыть кодек
        [DllImport("MSVFW32.dll")]
        public static extern int ICClose(uint hic);

        //HIC кодека
        uint hic;

        //Заголовки входного и выходного битмэпа
        BITMAPINFOHEADER bmih_out = new BITMAPINFOHEADER();
        BITMAPINFOHEADER bmih_in = new BITMAPINFOHEADER();

        const int mjpg = 0x67706A6D;
        const int type = 0x63646976;

        #endregion

        #region Events

        /// <summary>Делегат события распаковки</summary>
        /// <param name="frame">Данные распакованного кадра</param>
        public delegate void DelegateEventDecode(byte[] frame);
        /// <summary>Событие распаковки</summary>
        public event DelegateEventDecode EventDecode;

        #endregion

        /// <summary>Конструктор</summary>
        public JpegDecoder()
        {
        }

        /// <summary>Открыть Jpeg-декомпрессор Windows Compression Manager</summary>
        /// <param name="width">Ширина изображения</param>
        /// <param name="height">Высота изображения</param>
        public bool OpenVCMDecoder()
        {
            hic = 0;
            hic = ICOpen(type, mjpg, ICMODE.ICMODE_DECOMPRESS);
            if (hic == 0) return false;
            return true;
        }

        /// <summary>Закрыть Jpeg-декомпрессор Windows Compression Manager</summary>
        public int CloseVCMDecoder()
        {
            return ICClose(hic);
        }

        /// <summary>Распаковка кадра в RGB24</summary>
        public bool DecodeFrame(byte[] in_data, int width, int height, byte[] out_data)
        {
            if (hic == 0) return false;
            try
            {
                uint flag = ICDECOMPRESS_NOTKEYFRAME;// 0x08000000;
                //Входной заголовок
                bmih_in.biSize = 40;
                bmih_in.biWidth = width;
                bmih_in.biHeight = height;
                bmih_in.biBitCount = 24;
                bmih_in.biCompression = mjpg;
                bmih_in.biPlanes = 1;
                bmih_in.biSizeImage = in_data.Length;
                //Выходной заголовок
                bmih_out.biSize = 40;
                bmih_out.biWidth = width;
                bmih_out.biHeight = height;
                bmih_out.biBitCount = 24;
                bmih_out.biCompression = 0;
                bmih_out.biPlanes = 1;
                bmih_out.biSizeImage = 0;
                int i = ICDecompress(hic, flag, ref bmih_in, in_data, ref bmih_out, out_data);
                if (i == 0) if (EventDecode != null) EventDecode(out_data);
            }
            catch { return false; };
            return true;
        }

        /// <summary>Распаковка кадра в RGB24</summary>
        public bool DecodeFrame32bit(byte[] in_data, int width, int height, byte[] out_data)
        {
            if (hic == 0) return false;
            try
            {
                uint flag = ICDECOMPRESS_NOTKEYFRAME;// 0x08000000;
                //Входной заголовок
                bmih_in.biSize = 40;
                bmih_in.biWidth = width;
                bmih_in.biHeight = height;
                bmih_in.biBitCount = 32;
                bmih_in.biCompression = mjpg;
                bmih_in.biPlanes = 1;
                bmih_in.biSizeImage = in_data.Length;
                //Выходной заголовок
                bmih_out.biSize = 40;
                bmih_out.biWidth = width;
                bmih_out.biHeight = height;
                bmih_out.biBitCount = 32;
                bmih_out.biCompression = 0;
                bmih_out.biPlanes = 1;
                bmih_out.biSizeImage = 0;
                int i = ICDecompress(hic, flag, ref bmih_in, in_data, ref bmih_out, out_data);
                if (i == 0) if (EventDecode != null) EventDecode(out_data);
            }
            catch { return false; };
            return true;
        }

        ///<summary>Распаковка jpeg при помощи Windows Presentation Core 
        ///из состава .NET Framework 3.0</summary>
        /// <param name="in_data">Входные данные</param>
        /// <param name="out_data">Выходные данные RGB24</param>
        public void DecodeWPF(byte[] in_data, int width, int height, byte[] out_data)
        {
            MemoryStream ms = new MemoryStream(in_data);
            try
            {
                JpegBitmapDecoder decode = new JpegBitmapDecoder(ms, BitmapCreateOptions.PreservePixelFormat,
                                                                 BitmapCacheOption.Default);
                decode.Frames[0].CopyPixels(out_data, width * 3, 0);
                if (EventDecode != null) EventDecode(out_data);
            }
            catch { };
        }
    }
}