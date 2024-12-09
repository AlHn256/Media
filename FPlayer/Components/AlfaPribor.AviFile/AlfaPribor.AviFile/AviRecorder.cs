using System;
using System.Collections.Generic;
using System.Text;

using AlfaPribor.AviFile;

namespace AlfaPribor.AviRecorder
{

    /// <summary>Класс записи видеороликов в АВИ</summary>
    public class AviRecorder
    {

        AviFile.AviFile wfile;
        int total_frames;

        /// <summary>Конструктор класса</summary>
        public AviRecorder()
        {

        }

        /// <summary>Создание файла</summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="width">Ширина кадра</param>
        /// <param name="height">Высота кадра</param>
        /// <param name="fccHandler">Сигнатура сжатия</param>
        /// <param name="compress">Сжимать данные при записи кадров</param>
        /// <returns>Результат операции</returns>
        public bool CreateFile(string path, int width, int height, uint fccHandler, bool compress)
        {
            if (wfile != null) return false;
            else
            {
                wfile = new AviFile.AviFile();
                wfile.Open(path, Avi.OF_CREATE);

                Avi.AVISTREAMINFO stream = new Avi.AVISTREAMINFO();
                stream.dwQuality = 10000;
                stream.fccHandler = fccHandler;
                stream.fccType = Avi.StreamtypeVIDEO;
                stream.rcFrame.left = 0;
                stream.rcFrame.top = 0;
                stream.rcFrame.right = (uint)width;
                stream.rcFrame.bottom = (uint)height;
                stream.dwRate = 1000000;
                stream.dwScale = 40000;
                stream.dwStart = 0;

                Avi.BITMAPINFOHEADER bmih = new Avi.BITMAPINFOHEADER();
                bmih.biSize = 40;
                bmih.biWidth = width;
                bmih.biHeight = height;
                bmih.biBitCount = 24;
                if (compress) bmih.biCompression = 0;
                else bmih.biCompression = fccHandler;
                bmih.biSizeImage = (uint)(width * height * 3);
                bmih.biPlanes = 1;

                if (compress)
                {
                    //Параметры сжатия AVI
                    Avi.AVICOMPRESSOPTIONS opts = new Avi.AVICOMPRESSOPTIONS();
                    opts.fccType = Avi.StreamtypeVIDEO;
                    opts.fccHandler = fccHandler;
                    opts.dwQuality = 10000;
                    //Создать сжатый видеопоток
                    int res = wfile.CreateCompressedStream(stream, bmih, opts);
                    if (res != 0) return false;
                }
                else
                {
                    int res = wfile.CreateStream(stream, bmih);
                    if (res != 0) return false;
                }

                total_frames = 0;
            }

            return true;
        }

        /// <summary>Запись кадра</summary>
        /// <param name="buff">Буфер кадра</param>
        /// <param name="compress">true: сжимать входной массив установленным при создании кодеком
        /// false: помещать входной массив в поток файл без изменений</param>
        /// <returns></returns>
        public bool WriteFrame(byte[] buff, bool compress)
        {
            int samples = 1;
            try
            {
                //Сжимать записываемые данные
                if (compress)
                {
                    if (wfile.WriteCompress(0, total_frames, ref samples, buff, 0, buff.Length) > 0)
                    {
                        total_frames++;
                        return true;
                    }
                    else return false;
                }
                //Не сжимать записываемые данные
                //Запис несжатого AVI или помещение уже сжатых данных
                else 
                {
                    if (wfile.Write(0, total_frames, ref samples, buff, 0, buff.Length) > 0)
                    {
                        total_frames++;
                        return true;
                    }
                    else return false;
                }
            }
            catch { return false; }
        }

        /// <summary>Закрыть файл</summary>
        /// <returns>Результат операции</returns>
        public bool CloseFile()
        {
            if (wfile != null)
            {
                try { wfile.Close(); }
                catch { return false; }
                wfile = null;
                return true;
            }
            else return false;
        }

    }

}
