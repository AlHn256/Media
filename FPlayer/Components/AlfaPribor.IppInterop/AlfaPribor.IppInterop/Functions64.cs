using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace AlfaPribor.IppInterop
{
    public class IppFunctions64
    {

        /// <summary>Функция зеркалирования изображения 8 бит/канал, 1 канал цветности</summary>
        /// <param name="pSrc">Исходный массив изображения</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="pDst">Выходной массив изображения</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="roiSize">Область обработки</param>
        /// <param name="flip">Ось зеркалирования</param>
        /// <returns>результат выполнения операции (0 - успешно)</returns>
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiMirror_8u_C1R")]
        public static extern IppStatus ippiMirror_8u_C1R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiSize roiSize, IppiAxis flip);

        /// <summary>Функция зеркалирования изображения 8 бит/канал, 3 канала</summary>
        /// <param name="pSrc">Исходный массив изображения</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="pDst">Выходной массив изображения</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="roiSize">Область обработки</param>
        /// <param name="flip">Ось зеркалирования</param>
        /// <returns>результат выполнения операции (0 - успешно)</returns>
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiMirror_8u_C3R")]
        public static extern IppStatus ippiMirror_8u_C3R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiSize roiSize, IppiAxis flip);

        /// <summary>Функция зеркалирования изображения 8 бит/канал, 4 канала</summary>
        /// <param name="pSrc">Исходный массив изображения</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="pDst">Выходной массив изображения</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="roiSize">Область обработки</param>
        /// <param name="flip">Ось зеркалирования</param>
        /// <returns>результат выполнения операции (0 - успешно)</returns>
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiMirror_8u_C4R")]
        public static extern IppStatus ippiMirror_8u_C4R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiSize roiSize, IppiAxis flip);

        /// <summary>Копирование изображения 8 бит/канал, 3 канала цветности, в 
        /// изображение 8 бит/канал, 4 канала цветности, с прозрачным каналом</summary>
        /// <param name="pSrc">Исходный массив изображения</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="pDst">Выходной массив изображения</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="roiSize">Область обработки</param>
        /// <returns>результат выполнения операции (0 - успешно)</returns>
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiCopy_8u_C3AC4R")]
        public static extern IppStatus ippiCopy_8u_C3AC4R(byte[] pSrc, int srcStep, IntPtr pDst, int dstStep, IppiSize roiSize);

        /// <summary>(Перегрузка) Копирование изображения 8 бит/канал, 3 канала цветности, в 
        /// изображение 8 бит/канал, 4 канала цветности, с прозрачным каналом</summary>
        /// <param name="pSrc">Исходный массив изображения</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="pDst">Выходной массив изображения</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="roiSize">Область обработки</param>
        /// <returns>результат выполнения операции (0 - успешно)</returns>
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiCopy_8u_C3AC4R")]
        public static extern IppStatus ippiCopy_8u_C3AC4R(IntPtr pSrc, int srcStep, IntPtr pDst, int dstStep, IppiSize roiSize);

        /// <summary>Копирование изображения 8 бит/канал, 4 канала цветности</summary>
        /// <param name="pSrc">Исходный массив изображения</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="pDst">Указатель на массив выходного изображения</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="roiSize">Область обработки</param>
        /// <returns>результат выполнения операции (0 - успешно)</returns>
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiCopy_8u_C4R")]
        public static extern IppStatus ippiCopy_8u_C4R(byte[] pSrc, int srcStep, IntPtr pDst, int dstStep, IppiSize roiSize);

        /// <summary>(Перегрузка) Копирование изображения 8 бит/канал, 4 канала цветности</summary>
        /// <param name="pSrc">Указатель на массив исходного изображения</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="pDst">Указатель на массив выходного изображения</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="roiSize">Область обработки</param>
        /// <returns>результат выполнения операции (0 - успешно)</returns>
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiCopy_8u_C4R")]
        public static extern IppStatus ippiCopy_8u_C4R(IntPtr pSrc, int srcStep, IntPtr pDst, int dstStep, IppiSize roiSize);

        [DllImport("ipp2016x64.dll", EntryPoint = "ippiCopy_8u_C4R")]
        public static extern IppStatus ippiCopy_8u_C4R(IntPtr pSrc, int srcStep, byte[] pDst, int dstStep, IppiSize roiSize);

        /// <summary>Создание изображения 8 бит, 1 канал в оттенках серого из 8 бит, 3 канала, формата RGB</summary>
        /// <param name="pSrc">Указатель на массив исходного изображения</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="pDst">Указатель на массив выходного изображения</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="roiSize">Область обработки</param>
        /// <returns>результат выполнения операции (0 - успешно)</returns>
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiRGBToGray_8u_C3C1R")]
        public static extern IppStatus ippiRGBToGray_8u_C3C1R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiSize roiSize);

        /// <summary>Изменение размера изображения 8 бит, 1 канал</summary>
        /// <param name="pSrc">Исходный массив изображения</param>
        /// <param name="srcSize">Размер входного изображения</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="srcRoi">Область обработки входного изображения</param>
        /// <param name="pDst">Выходной массив изображения</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="dstRoiSize">Область обработки выходного изображения</param>
        /// <param name="xFactor">Масштабирование по ширине</param>
        /// <param name="yFactor">Масштабирование по высоте</param>
        /// <param name="interpolation">Тип интерполяции</param>
        /// <returns>Результат выполнения операции (0 - успешно)</returns>
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResize_8u_C1R")]/*Сделать*/
        public static extern IppStatus ippiResize_8u_C1R(byte[] pSrc, IppiSize srcSize, int srcStep, IppiRect srcRoi,
                                                         byte[] pDst, int dstStep, IppiSize dstRoiSize,
                                                         double xFactor, double yFactor, IppInterpolation interpolation);

        /// <summary>Изменение размера изображения 8 бит, 3 канала</summary>
        /// <param name="pSrc">Исходный массив изображения</param>
        /// <param name="srcSize">Размер входного изображения</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="srcRoi">Область обработки входного изображения</param>
        /// <param name="pDst">Выходной массив изображения</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="dstRoiSize">Область обработки выходного изображения</param>
        /// <param name="xFactor">Масштабирование по ширине</param>
        /// <param name="yFactor">Масштабирование по высоте</param>
        /// <param name="interpolation">Тип интерполяции</param>
        /// <returns>Результат выполнения операции (0 - успешно)</returns>
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResize_8u_C3R")]/*Сделать*/
        public static extern IppStatus ippiResize_8u_C3R(byte[] pSrc, IppiSize srcSize, int srcStep, IppiRect srcRoi,
                                                         byte[] pDst, int dstStep, IppiSize dstRoiSize,
                                                         double xFactor, double yFactor, IppInterpolation interpolation);

        /// <summary>Изменение размера изображения 8 бит, 4 канала</summary>
        /// <param name="pSrc">Исходный массив изображения</param>
        /// <param name="srcSize">Размер входного изображения</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="srcRoi">Область обработки входного изображения</param>
        /// <param name="pDst">Выходной массив изображения</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="dstRoiSize">Область обработки выходного изображения</param>
        /// <param name="xFactor">Масштабирование по ширине</param>
        /// <param name="yFactor">Масштабирование по высоте</param>
        /// <param name="interpolation">Тип интерполяции</param>
        /// <returns>Результат выполнения операции (0 - успешно)</returns>
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResize_8u_C4R")]/*Сделать*/
        public static extern IppStatus ippiResize_8u_C4R(byte[] pSrc, IppiSize srcSize, int srcStep, IppiRect srcRoi,
                                                         byte[] pDst, int dstStep, IppiSize dstRoiSize,
                                                         double xFactor, double yFactor, IppInterpolation interpolation);

        /// <summary>Изменение размера изображения 8 бит, 4 канала</summary>
        /// <param name="pSrc">Исходный массив изображения</param>
        /// <param name="srcSize">Размер входного изображения</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="srcRoi">Область обработки входного изображения</param>
        /// <param name="pDst">Выходной массив изображения</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="dstRoiSize">Область обработки выходного изображения</param>
        /// <param name="xFactor">Масштабирование по ширине</param>
        /// <param name="yFactor">Масштабирование по высоте</param>
        /// <param name="interpolation">Тип интерполяции</param>
        /// <returns>Результат выполнения операции (0 - успешно)</returns>
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResize_8u_C4R")]/*Сделать*/
        public static extern IppStatus ippiResize_8u_C4R(byte[] pSrc, IppiSize srcSize, int srcStep, IppiRect srcRoi,
                                                         IntPtr pDst, int dstStep, IppiSize dstRoiSize,
                                                         double xFactor, double yFactor, IppInterpolation interpolation);

        #region Resize Cubic(Бикубическая интерполяция(Основная))

        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeCubic_8u_C1R")]
        public static extern IppStatus ippiResizeCubic_8u_C1R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiPoint dstOffset, IppiSize dstSize, IppiBorderType border, IntPtr pBorderValue, IntPtr pSpec, byte[] pBuffer);

        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeCubic_8u_C1R")]
        public static extern IppStatus ippiResizeCubic_8u_C1R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiPoint dstOffset, IppiSize dstSize, IppiBorderType border, IntPtr pBorderValue, IntPtr pSpec, IntPtr pBuffer);

        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeCubic_8u_C1R")]
        public static extern IppStatus ippiResizeCubic_8u_C1R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiPoint dstOffset, IppiSize dstSize, IppiBorderType border, IntPtr pBorderValue, float[] pSpec, byte[] pBuffer);

        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeCubic_8u_C3R")]
        public static extern IppStatus ippiResizeCubic_8u_C3R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiPoint dstOffset, IppiSize dstSize, IppiBorderType border, IntPtr pBorderValue, IntPtr pSpec, byte[] pBuffer);
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeCubic_8u_C3R")]
        public static extern IppStatus ippiResizeCubic_8u_C3R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiPoint dstOffset, IppiSize dstSize, IppiBorderType border, IntPtr pBorderValue, IntPtr pSpec, IntPtr pBuffer);

        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeCubic_8u_C4R")]
        public static extern IppStatus ippiResizeCubic_8u_C4R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiPoint dstOffset, IppiSize dstSize, IppiBorderType border, IntPtr pBorderValue, IntPtr pSpec, byte[] pBuffer);
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeCubic_8u_C4R")]
        public static extern IppStatus ippiResizeCubic_8u_C4R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiPoint dstOffset, IppiSize dstSize, IppiBorderType border, IntPtr pBorderValue, IntPtr pSpec, IntPtr pBuffer);

        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeCubic_8u_C4R")]
        public static extern IppStatus ippiResizeCubic_8u_C4R(byte[] pSrc, int srcStep, IntPtr pDst, int dstStep, IppiPoint dstOffset, IppiSize dstSize, IppiBorderType border, IntPtr pBorderValue, IntPtr pSpec, byte[] pBuffer);
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeCubic_8u_C4R")]
        public static extern IppStatus ippiResizeCubic_8u_C4R(byte[] pSrc, int srcStep, IntPtr pDst, int dstStep, IppiPoint dstOffset, IppiSize dstSize, IppiBorderType border, IntPtr pBorderValue, IntPtr pSpec, IntPtr pBuffer);

        #endregion

        #region Resize Linear(Линейная интерполяция)

        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeLinear_8u_C1R")]
        public static extern IppStatus ippiResizeLinear_8u_C1R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiPoint dstOffset, IppiSize dstSize, IppiBorderType border, IntPtr pBorderValue, IntPtr pSpec, byte[] pBuffer);
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeLinear_8u_C1R")]
        public static extern IppStatus ippiResizeLinear_8u_C1R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiPoint dstOffset, IppiSize dstSize, IppiBorderType border, IntPtr pBorderValue, IntPtr pSpec, IntPtr pBuffer);


        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeLinear_8u_C3R")]
        public static extern IppStatus ippiResizeLinear_8u_C3R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiPoint dstOffset, IppiSize dstSize, IppiBorderType border, IntPtr pBorderValue, IntPtr pSpec, byte[] pBuffer);
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeLinear_8u_C3R")]
        public static extern IppStatus ippiResizeLinear_8u_C3R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiPoint dstOffset, IppiSize dstSize, IppiBorderType border, IntPtr pBorderValue, IntPtr pSpec, IntPtr pBuffer);


        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeLinear_8u_C4R")]
        public static extern IppStatus ippiResizeLinear_8u_C4R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiPoint dstOffset, IppiSize dstSize, IppiBorderType border, IntPtr pBorderValue, IntPtr pSpec, byte[] pBuffer);
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeLinear_8u_C4R")]
        public static extern IppStatus ippiResizeLinear_8u_C4R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiPoint dstOffset, IppiSize dstSize, IppiBorderType border, IntPtr pBorderValue, IntPtr pSpec, IntPtr pBuffer);


        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeLinear_8u_C4R")]
        public static extern IppStatus ippiResizeLinear_8u_C4R(byte[] pSrc, int srcStep, IntPtr pDst, int dstStep, IppiPoint dstOffset, IppiSize dstSize, IppiBorderType border, IntPtr pBorderValue, IntPtr pSpec, byte[] pBuffer);
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeLinear_8u_C4R")]
        public static extern IppStatus ippiResizeLinear_8u_C4R(byte[] pSrc, int srcStep, IntPtr pDst, int dstStep, IppiPoint dstOffset, IppiSize dstSize, IppiBorderType border, IntPtr pBorderValue, IntPtr pSpec, IntPtr pBuffer);

        #endregion

        #region Resize Support

        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeGetSize_8u")]
        public static extern IppStatus ippiResizeGetSize_8u(IppiSize srcSize, IppiSize dstSize, IppiInterpolationType interpolation, int antialiasing, out int pSpecSize, out int pInitBufSize);

        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeGetBufferSize_8u")]
        public static extern IppStatus  ippiResizeGetBufferSize_8u(IntPtr pSpec, IppiSize dstSize, int numChannels, out int pBufSize);

        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeLinearInit_8u")]
        public static extern IppStatus ippiResizeLinearInit_8u(IppiSize srcSize, IppiSize dstSize, IntPtr pSpec);
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeLinearInit_8u")]
        public static extern IppStatus ippiResizeLinearInit_8u(IppiSize srcSize, IppiSize dstSize, float[] pSpec);

        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeCubicInit_8u")]
        public static extern IppStatus ippiResizeCubicInit_8u(IppiSize srcSize, IppiSize dstSize, float valueB, float valueC, IntPtr pSpec, IntPtr pInitBuf);
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeCubicInit_8u")]
        public static extern IppStatus ippiResizeCubicInit_8u(IppiSize srcSize, IppiSize dstSize, float valueB, float valueC, float[] pSpec, byte[] pInitBuf);

        [DllImport("ipp2016x64.dll", EntryPoint = "ippiResizeCubicInit_8u")]
        public static extern IntPtr ippsMalloc_8u(int initSize);

        #endregion

        /// <summary>Поворот изображения 8 бит 1 канал</summary>
        /// <param name="pSrc">Исходный массив изображения</param>
        /// <param name="srcSize">Размер входного изображения</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="srcRoi">Область обработки входного изображения</param>
        /// <param name="pDst">Выходной массив изображения</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="dstRoi">Область обработки выходного изображения</param>
        /// <param name="angle">Угол поворота</param>
        /// <param name="xCenter">Координата X центра поворота</param>
        /// <param name="yCenter">Координата Y центра поворота</param>
        /// <param name="interpolation">Тип интерполяци</param>
        /// <returns>Результат выполнения операции (0 - успешно)</returns>
        //[DllImport("ipp2016x64.dll", EntryPoint = "ippiRotateCenter_8u_C1R")]/*Сделать*/
        [DllImport("ipp2010.dll", EntryPoint = "ippiRotateCenter_8u_C1R")]/*Сделать*/
        public static extern IppStatus ippiRotateCenter_8u_C1R(byte[] pSrc,
                                           IppiSize srcSize, int srcStep, IppiRect srcRoi,
                                           byte[] pDst, int dstStep, IppiRect dstRoi, double angle,
                                           double xCenter, double yCenter, int interpolation);

        /// <summary>Вычисление среднего значения изображения 8 бит 1 канал</summary>
        /// <param name="pSrc">Входное изображение</param>
        /// <param name="srcStep">Шаг строки в байтах</param>
        /// <param name="srcSize">Регион обработки</param>
        /// <param name="mean">Среднее значение точек</param>
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiMean_8u_C1R")]
        public static extern IppStatus ippiMean_8u_C1R(byte[] pSrc, int srcStep, IppiSize srcSize, ref double mean);

        /// <summary>(Перегрузка) Вычисление среднего значения изображения 8 бит 1 канал</summary>
        /// <param name="pSrc">Входное изображение</param>
        /// <param name="srcStep">Шаг строки в байтах</param>
        /// <param name="srcSize">Регион обработки</param>
        /// <param name="mean">Среднее значение точек</param>
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiMean_8u_C1R")]
        public static extern IppStatus ippiMean_8u_C1R(IntPtr pSrc, int srcStep, IppiSize srcSize, ref double mean);

        /// <summary>Преобразование изображения 8 бит/пиксель в 24 бит пиксель с
        /// заполнением цветного изображения пикселями серого</summary>
        /// <param name="pSrc">Входное изображение</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="pDst">Выходное цветное изображение</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="roiSize">Регион обработки</param>
        /// <returns>Результат операции</returns>
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiDup_8u_C1C3R")]
        public static extern IppStatus ippiDup_8u_C1C3R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiSize roiSize);

        /// <summary>Конвертация уветного изображения в серое с коэффициентами преобразования</summary>
        /// <param name="pSrc">Входное цветное изображение</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="pDst">Выходное черно белое изображение</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="roiSize">Регион обработки</param>
        /// <param name="coeffs">Коэффицианты преобразования R G B</param>
        /// <returns>Результат операции</returns>
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiColorToGray_8u_C3C1R")]
        public static extern IppStatus ippiColorToGray_8u_C3C1R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiSize roiSize, float[] coeffs);

        /// <summary>Трансформация 8u_C1R</summary>
        /// <param name="pSrc">Массив источник</param>
        /// <param name="srcSize">Размер источника</param>
        /// <param name="srcStep">Шаг строки</param>
        /// <param name="srcROI">Область интереса</param>
        /// <param name="pxMap">Массив искажений по x</param>
        /// <param name="xMapStep">Шаг массива искажений по x</param>
        /// <param name="pyMap">Массив искажений по x</param>
        /// <param name="yMapStep">Шаг массива искажений по y</param>
        /// <param name="pDst">Массив приемник</param>
        /// <param name="dstStep">Шаг строк массива приемника</param>
        /// <param name="dstRoiSize">Область интереса приемного массива</param>
        /// <param name="interpolation">Тип интерполяции</param>
        /// <returns></returns>
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiRemap_8u_C1R")]
        public static extern IppStatus ippiRemap_8u_C1R(byte[] pSrc, IppiSize srcSize, int srcStep, IppiRect srcROI,
                                                        float[,] pxMap, int xMapStep, float[,] pyMap, int yMapStep, byte[] pDst,
                                                        int dstStep, IppiSize dstRoiSize, IppInterpolation interpolation);

        /// <summary>Трансформация 8u_C3R</summary>
        /// <param name="pSrc">Массив источник</param>
        /// <param name="srcSize">Размер источника</param>
        /// <param name="srcStep">Шаг строки</param>
        /// <param name="srcROI">Область интереса</param>
        /// <param name="pxMap">Массив искажений по x</param>
        /// <param name="xMapStep">Шаг массива искажений по x</param>
        /// <param name="pyMap">Массив искажений по x</param>
        /// <param name="yMapStep">Шаг массива искажений по y</param>
        /// <param name="pDst">Массив приемник</param>
        /// <param name="dstStep">Шаг строк массива приемника</param>
        /// <param name="dstRoiSize">Область интереса приемного массива</param>
        /// <param name="interpolation">Тип интерполяции</param>
        /// <returns></returns>
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiRemap_8u_C3R")]
        public static extern IppStatus ippiRemap_8u_C3R(byte[] pSrc, IppiSize srcSize, int srcStep, IppiRect srcROI,
                                                        float[,] pxMap, int xMapStep, float[,] pyMap, int yMapStep, byte[] pDst,
                                                        int dstStep, IppiSize dstRoiSize, IppInterpolation interpolation);

        /// <summary>Трансформация 8u_C4R</summary>
        /// <param name="pSrc">Массив источник</param>
        /// <param name="srcSize">Размер источника</param>
        /// <param name="srcStep">Шаг строки</param>
        /// <param name="srcROI">Область интереса</param>
        /// <param name="pxMap">Массив искажений по x</param>
        /// <param name="xMapStep">Шаг массива искажений по x</param>
        /// <param name="pyMap">Массив искажений по x</param>
        /// <param name="yMapStep">Шаг массива искажений по y</param>
        /// <param name="pDst">Массив приемник</param>
        /// <param name="dstStep">Шаг строк массива приемника</param>
        /// <param name="dstRoiSize">Область интереса приемного массива</param>
        /// <param name="interpolation">Тип интерполяции</param>
        /// <returns></returns>
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiRemap_8u_C4R")]
        public static extern IppStatus ippiRemap_8u_C4R(byte[] pSrc, IppiSize srcSize, int srcStep, IppiRect srcROI,
                                                        float[,] pxMap, int xMapStep, float[,] pyMap, int yMapStep, byte[] pDst,
                                                        int dstStep, IppiSize dstRoiSize, IppInterpolation interpolation);

        /// <summary>Получить размер буфера для маски исправлений искажений</summary>
        /// <param name="roiSize">Размер кадра</param>
        /// <param name="pBufsize">Получаемый размер буфера</param>
        /// <returns>Результат операции</returns>
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiUndistortGetSize")]
        public static extern IppStatus ippiUndistortGetSize(IppiSize roiSize, ref int pBufsize);

        /// <summary>Создание маски искажений</summary>
        /// <param name="pxMap">Массив маски смещений по горизонтали</param>
        /// <param name="xStep">Шаг строки маски смещений по горизонтали</param>
        /// <param name="pyMap">Массив маски смещений по вертикали</param>
        /// <param name="yStep">Шаг строки маски смещений по вертикали</param>
        /// <param name="roiSize">Область интереса маски</param>
        /// <param name="fx">Фокальное расстояние по оси x</param>
        /// <param name="fy">Фокальное расстояние по оси y</param>
        /// <param name="cx">x координата ключевой точкки</param>
        /// <param name="cy">y координата ключевой точкки</param>
        /// <param name="k1">Первый коэффициент радиальных искажений</param>
        /// <param name="k2">Второй коэффициент радиальных искажений</param>
        /// <param name="p1">Первый коэффициент тангенциальных искажений</param>
        /// <param name="p2">Второй коэффициент тангенциальных искажений</param>
        /// <param name="pBuffer">Массив временного буфера</param>
        /// <returns>Результат IppStatus</returns>
        [DllImport("ipp2016x64.dll")]
        public static extern IppStatus ippiCreateMapCameraUndistort_32f_C1R(float[] pxMap, int xStep, float[] pyMap,
                                                  int yStep, IppiSize roiSize,
                                                  float fx, float fy, float cx, float cy,
                                                  double k1, double k2,
                                                  float p1, float p2, byte[] pBuffer);

        /// <summary>Исправление искажений 8 бит/пиксель</summary>
        /// <param name="pSrc">Массив входного изображения</param>
        /// <param name="srcStep">Шаг строки входного изображения</param>
        /// <param name="pDst">Массив выходного изображения</param>
        /// <param name="dstStep">Шаг строки выходного изображения</param>
        /// <param name="roiSize">Область интереса</param>
        /// <param name="fx">Фокальное расстояние по оси x</param>
        /// <param name="fy">Фокальное расстояние по оси y</param>
        /// <param name="cx">x координата ключевой точкки</param>
        /// <param name="cy">y координата ключевой точкки</param>
        /// <param name="k1">Первый коэффициент радиальных искажений</param>
        /// <param name="k2">Второй коэффициент радиальных искажений</param>
        /// <param name="pBuffer">Массив временного буфера</param>
        /// <returns>Результат IppStatus</returns>
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiUndistortRadial_8u_C1R")]
        public static extern IppStatus ippiUndistortRadial_8u_C1R(byte[] pSrc, int srcStep, byte[] pDst,
                                       int dstStep, IppiSize roiSize,
                                       float fx, float fy, float cx, float cy,
                                       float k1, float k2, byte[] pBuffer);

        /// <summary>Исправление искажений 24 бит/пиксель</summary>
        /// <param name="pSrc">Массив входного изображения</param>
        /// <param name="srcStep">Шаг строки входного изображения</param>
        /// <param name="pDst">Массив выходного изображения</param>
        /// <param name="dstStep">Шаг строки выходного изображения</param>
        /// <param name="roiSize">Область интереса</param>
        /// <param name="fx">Фокальное расстояние по оси x</param>
        /// <param name="fy">Фокальное расстояние по оси y</param>
        /// <param name="cx">x координата ключевой точкки</param>
        /// <param name="cy">y координата ключевой точкки</param>
        /// <param name="k1">Первый коэффициент радиальных искажений</param>
        /// <param name="k2">Второй коэффициент радиальных искажений</param>
        /// <param name="pBuffer">Массив временного буфера</param>
        /// <returns>Результат IppStatus</returns>
        [DllImport("ipp2016x64.dll", EntryPoint = "ippiUndistortRadial_8u_C3R")]
        public static extern IppStatus ippiUndistortRadial_8u_C3R(byte[] pSrc, int srcStep, byte[] pDst,
                                       int dstStep, IppiSize roiSize,
                                       float fx, float fy, float cx, float cy,
                                       float k1, float k2, byte[] pBuffer);

    }
}
