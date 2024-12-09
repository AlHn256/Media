using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using SD = System.Drawing;
using System.IO;

namespace AlfaPribor.IppInterop
{
    public class IppFunctions
    {
        
        /// <summary>Функция зеркалирования изображения 8 бит/канал, 1 канал цветности</summary>
        /// <param name="pSrc">Исходный массив изображения</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="pDst">Выходной массив изображения</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="roiSize">Область обработки</param>
        /// <param name="flip">Ось зеркалирования</param>
        /// <returns>результат выполнения операции (0 - успешно)</returns>
        public static IppStatus ippiMirror_8u_C1R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiSize roiSize, IppiAxis flip)
        {
            if (IntPtr.Size == 4) return IppFunctions32.ippiMirror_8u_C1R(pSrc, srcStep, pDst, dstStep, roiSize, flip);
            return IppFunctions64.ippiMirror_8u_C1R(pSrc, srcStep, pDst, dstStep, roiSize, flip);
        }

        /// <summary>Функция зеркалирования изображения 8 бит/канал, 3 канала</summary>
        /// <param name="pSrc">Исходный массив изображения</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="pDst">Выходной массив изображения</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="roiSize">Область обработки</param>
        /// <param name="flip">Ось зеркалирования</param>
        /// <returns>результат выполнения операции (0 - успешно)</returns>
        public static IppStatus ippiMirror_8u_C3R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiSize roiSize, IppiAxis flip)
        {
            if (IntPtr.Size == 4) return IppFunctions32.ippiMirror_8u_C3R(pSrc, srcStep, pDst, dstStep, roiSize, flip);
            return IppFunctions64.ippiMirror_8u_C3R(pSrc, srcStep, pDst, dstStep, roiSize, flip);
        }

        /// <summary>Функция зеркалирования изображения 8 бит/канал, 4 канала</summary>
        /// <param name="pSrc">Исходный массив изображения</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="pDst">Выходной массив изображения</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="roiSize">Область обработки</param>
        /// <param name="flip">Ось зеркалирования</param>
        /// <returns>результат выполнения операции (0 - успешно)</returns>
        public static IppStatus ippiMirror_8u_C4R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiSize roiSize, IppiAxis flip)
        {
            if (IntPtr.Size == 4) return IppFunctions32.ippiMirror_8u_C4R(pSrc, srcStep, pDst, dstStep, roiSize, flip);
            return IppFunctions64.ippiMirror_8u_C4R(pSrc, srcStep, pDst, dstStep, roiSize, flip);
        }
                       
        /// <summary>Копирование изображения 8 бит/канал, 3 канала цветности, в 
        /// изображение 8 бит/канал, 4 канала цветности, с прозрачным каналом</summary>
        /// <param name="pSrc">Исходный массив изображения</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="pDst">Выходной массив изображения</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="roiSize">Область обработки</param>
        /// <returns>результат выполнения операции (0 - успешно)</returns>
        public static IppStatus ippiCopy_8u_C3AC4R(byte[] pSrc, int srcStep, IntPtr pDst, int dstStep, IppiSize roiSize)
        {
            IppStatus result = IppStatus.ippStsNoErr;
            if (IntPtr.Size == 4) result = IppFunctions32.ippiCopy_8u_C3AC4R(pSrc, srcStep, pDst, dstStep, roiSize);
            else
            {
                try { IppFunctions64.ippiCopy_8u_C3AC4R(pSrc, srcStep, pDst, dstStep, roiSize); }
                catch
                {
                    try
                    {
                        byte[] dstArray = ConvertTo32BitArray(pSrc, ref roiSize);
                        Marshal.Copy(dstArray, 0, pDst, dstArray.Length);
                    }
                    catch { }
                }
            }
            return result;
        }

        static byte[] ConvertTo32BitArray(byte[] pSrc, ref IppiSize roiSize)
        {
            byte[] dstArray = new byte[4 * roiSize.width * roiSize.height];
            int j = 0;
            for (int i = 0; i < pSrc.Length; i++)
            {
                if ((i + j) >= dstArray.Length)
                    break;
                dstArray[i + j] = pSrc[i];
                if (i % 3 == 0 && i > 0)
                {
                    j++;
                    dstArray[i + j] = 0;
                }
            }
            return dstArray;
        }

        /// <summary>(Перегрузка) Копирование изображения 8 бит/канал, 3 канала цветности, в 
        /// изображение 8 бит/канал, 4 канала цветности, с прозрачным каналом</summary>
        /// <param name="pSrc">Исходный массив изображения</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="pDst">Выходной массив изображения</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="roiSize">Область обработки</param>
        /// <returns>результат выполнения операции (0 - успешно)</returns>
        public static IppStatus ippiCopy_8u_C3AC4R(IntPtr pSrc, int srcStep, IntPtr pDst, int dstStep, IppiSize roiSize)
        {
            IppStatus result = IppStatus.ippStsNoErr;
            if (IntPtr.Size == 8) result = IppFunctions32.ippiCopy_8u_C3AC4R(pSrc, srcStep, pDst, dstStep, roiSize);
            else
            {
                try { IppFunctions64.ippiCopy_8u_C3AC4R(pSrc, srcStep, pDst, dstStep, roiSize); }
                catch
                {
                    byte[] srcArray = new byte[3 * roiSize.width * roiSize.height];
                    Marshal.Copy(pSrc, srcArray, 0, srcArray.Length);
                    byte[] dstArray = ConvertTo32BitArray(srcArray, ref roiSize);
                    Marshal.Copy(dstArray, 0, pDst, dstArray.Length);
                }
            }
            return result;
        }

        /// <summary>Копирование изображения 8 бит/канал, 4 канала цветности</summary>
        /// <param name="pSrc">Исходный массив изображения</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="pDst">Указатель на массив выходного изображения</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="roiSize">Область обработки</param>
        /// <returns>результат выполнения операции (0 - успешно)</returns>
        public static IppStatus ippiCopy_8u_C4R(byte[] pSrc, int srcStep, IntPtr pDst, int dstStep, IppiSize roiSize)
        {
            IppStatus status = IppStatus.ippStsNoErr;
            if (IntPtr.Size == 4)
                status = IppFunctions32.ippiCopy_8u_C4R(pSrc, srcStep, pDst, dstStep, roiSize);
            else
            {
                try { IppFunctions64.ippiCopy_8u_C4R(pSrc, srcStep, pDst, dstStep, roiSize); }
                catch { Marshal.Copy(pSrc, 0, pDst, 4 * roiSize.width * roiSize.height); }
            }
            return status;
        }

        /// <summary>(Перегрузка) Копирование изображения 8 бит/канал, 4 канала цветности</summary>
        /// <param name="pSrc">Указатель на массив исходного изображения</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="pDst">Указатель на массив выходного изображения</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="roiSize">Область обработки</param>
        /// <returns>результат выполнения операции (0 - успешно)</returns>
        public static IppStatus ippiCopy_8u_C4R(IntPtr pSrc, int srcStep, IntPtr pDst, int dstStep, IppiSize roiSize)
        {
            IppStatus status = IppStatus.ippStsNoErr;
            if (IntPtr.Size == 4) status = IppFunctions32.ippiCopy_8u_C4R(pSrc, srcStep, pDst, dstStep, roiSize);
            else
            {
                try { IppFunctions64.ippiCopy_8u_C4R(pSrc, srcStep, pDst, dstStep, roiSize); }
                catch { Marshal.Copy(new IntPtr[] { pSrc }, 0, pDst, 4 * roiSize.width * roiSize.height); }
            }
            return status;
        }

        public static IppStatus ippiCopy_8u_C4R(IntPtr pSrc, int srcStep, byte[] pDst, int dstStep, IppiSize roiSize)
        {
            IppStatus status = IppStatus.ippStsNoErr;
            if (IntPtr.Size == 4)
                status = IppFunctions32.ippiCopy_8u_C4R(pSrc, srcStep, pDst, dstStep, roiSize);
            else
            {
                try { IppFunctions64.ippiCopy_8u_C4R(pSrc, srcStep, pDst, dstStep, roiSize); }
                catch{Marshal.Copy(pSrc, pDst, 0, 4 * roiSize.width * roiSize.height);}
            }
            return status;
        }

        /// <summary>Создание изображения 8 бит, 1 канал в оттенках серого из 8 бит, 3 канала, формата RGB</summary>
        /// <param name="pSrc">Указатель на массив исходного изображения</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="pDst">Указатель на массив выходного изображения</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="roiSize">Область обработки</param>
        /// <returns>результат выполнения операции (0 - успешно)</returns>
        public static IppStatus ippiRGBToGray_8u_C3C1R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiSize roiSize)
        {
            IppStatus status = IppStatus.ippStsNoErr;
            if (IntPtr.Size == 4) status = IppFunctions32.ippiRGBToGray_8u_C3C1R(pSrc, srcStep, pDst, dstStep, roiSize);
            else
            {
                try { IppFunctions64.ippiRGBToGray_8u_C3C1R(pSrc, srcStep, pDst, dstStep, roiSize); }
                catch
                {
                    pDst = new byte[roiSize.width * roiSize.height];
                    int j = 0;
                    for (int i = 0; i < pSrc.Length - 3; i += 3)
                        pDst[j] = (byte)(0.3d * pSrc[i] + 0.59d * pSrc[1 + 1] + 0.11d * pSrc[i + 2]);
                }
            }
            return status;
        }

        /// <summary>Изменение размера изображения 8 бит, 1 канал. Не обработан</summary>
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
        /// 
        /*
         #region Resize Support

        [DllImport("ipp2010x64.dll", EntryPoint = "ippiResizeGetSize_8u")]
        public static extern IppStatus ippiResizeGetSize_8u(IppiSize srcSize, IppiSize dstSize, IppInterpolation interpolation, int antialiasing, out int pSpecSize, out int pInitBufSize);

        [DllImport("ipp2010x64.dll", EntryPoint = "ippiResizeGetBufferSize_8u")]
        public static extern IppStatus  ippiResizeGetBufferSize_8u(IntPtr pSpec, IppiSize dstSize, int numChannels, out int pBufSize);

        [DllImport("ipp2010x64.dll", EntryPoint = "ippiResizeLinearInit_8u")]
        public static extern IppStatus ippiResizeLinearInit_8u(IppiSize srcSize, IppiSize dstSize, IntPtr pSpec);

        [DllImport("ipp2010x64.dll", EntryPoint = "ippiResizeCubicInit_8u")]
        IppStatus ippiResizeCubicInit_8u(IppiSize srcSize, IppiSize dstSize, float valueB, float valueC, IntPtr pSpec, IntPtr pInitBuf);
        
        #endregion
         */
        static IntPtr CreateComArray<T>(int size)
        {
            T[] array = new T[size];
            GCHandle handle = GCHandle.Alloc(array, GCHandleType.Pinned);
           return  handle.AddrOfPinnedObject();
        }

        static IntPtr CreateComArray<T>(int size, out GCHandle handle)
        {
            T[] array = new T[size];
            handle = GCHandle.Alloc(array, GCHandleType.Pinned);
            return handle.AddrOfPinnedObject();
        }

        public static IppStatus ippiResize_8u_C1R(byte[] pSrc, IppiSize srcSize, int srcStep, IppiRect srcRoi,
                                                         byte[] pDst, int dstStep, IppiSize dstRoiSize,
                                                         double xFactor, double yFactor, IppInterpolation interpolation)
        {
            if(IntPtr.Size==4) return IppFunctions32.ippiResize_8u_C1R(pSrc, srcSize, srcStep, srcRoi, pDst, dstStep, dstRoiSize, xFactor, yFactor, interpolation);
            IppStatus status = IppStatus.ippStsNoErr;
            byte[] buffer = new byte[pDst.Length];
            float[] pSpec = new float[pDst.Length];
         
            int specSize = 0;
            int intBufferSize = 0;
            int realBufferSize=0;
            IntPtr bufferPtr = IntPtr.Zero;
            IntPtr pSpecPtr = IntPtr.Zero;
            GCHandle bufferPtrHandle;
            GCHandle pSpecPtrHandle;
            byte[] bufferData=null;
           // GCHandle realBuffPtrHandle=null;
            switch (interpolation)
            {
                case IppInterpolation.IPPI_INTER_LINEAR:
                    //IppFunctions64.ippiResizeGetSize_8u(srcSize, dstRoiSize, IppiInterpolationType.ippCubic, 0, out specSize, out intBufferSize);
                    //bufferPtr = CreateComArray<byte>(intBufferSize, out bufferPtrHandle);
                    //pSpecPtr = CreateComArray<float>(specSize, out pSpecPtrHandle);
                    //IppFunctions64.ippiResizeLinearInit_8u(srcSize, dstRoiSize, pSpecPtr);
                    //IppFunctions64.ippiResizeGetBufferSize_8u(pSpecPtr, dstRoiSize, 1, out realBufferSize);
                    ////realBuff = CreateComArray<byte>(realBufferSize, out realBuffPtrHandle);
                    //bufferData = new byte[realBufferSize];
                    ////Resize
                     ArrayPointer<float> pSpL = new ArrayPointer<float>(specSize);
                     status = IppFunctions64.ippiResizeLinearInit_8u(srcSize, dstRoiSize, pSpL.Pointer);
                     status = IppFunctions64.ippiResizeGetBufferSize_8u(pSpL.Pointer, dstRoiSize, 1, out realBufferSize);
                     ArrayPointer<byte> realBuffL = new ArrayPointer<byte>(realBufferSize);
                    //Resize
                     try
                     {
                         status = IppFunctions64.ippiResizeLinear_8u_C1R(pSrc, srcStep, pDst, dstStep, new IppiPoint { x = 0, y = 0 }, dstRoiSize, IppiBorderType.ippBorderInMem, IntPtr.Zero, pSpL.Pointer, realBuffL.Pointer);
                     }
                     finally {
                         pSpL.Release();
                         realBuffL.Release();
                     }
                    break;
                case IppInterpolation.IPPI_INTER_CUBIC:
                   // status = IppFunctions64.ippiResizeGetSize_8u(srcSize, dstRoiSize, IppiInterpolationType.ippCubic, 0, out specSize, out intBufferSize);
                   // bufferPtr = CreateComArray<byte>(intBufferSize, out bufferPtrHandle);
                   // pSpecPtr = CreateComArray<float>(specSize, out pSpecPtrHandle);
                   // status = IppFunctions64.ippiResizeCubicInit_8u(srcSize, dstRoiSize, 1, 2, pSpecPtr, bufferPtr);
                   // status = IppFunctions64.ippiResizeGetBufferSize_8u(pSpecPtr, dstRoiSize, 1, out realBufferSize);
                   //byte[] tmp = new byte[realBufferSize];
                   // //Resize
                   // try
                   // {
                   //     status = IppFunctions64.ippiResizeCubic_8u_C1R(pSrc, srcStep, pDst, dstStep, new IppiPoint { x = 0, y = 0 }, dstRoiSize, IppiBorderType.ippBorderInMem, IntPtr.Zero, pSpecPtr, tmp);
                   //     tmp = null;
                   // }
                   // catch { }


                     status = IppFunctions64.ippiResizeGetSize_8u(srcSize, dstRoiSize, IppiInterpolationType.ippCubic, 0, out specSize, out intBufferSize);
                     ArrayPointer<byte> buffInit = new ArrayPointer<byte>(intBufferSize);
                     ArrayPointer<float> pSp = new ArrayPointer<float>(specSize);

                     status = IppFunctions64.ippiResizeCubicInit_8u(srcSize, dstRoiSize, 1, 2, pSp.Pointer, buffInit.Pointer);
                     status = IppFunctions64.ippiResizeGetBufferSize_8u(pSp.Pointer, dstRoiSize, 1, out realBufferSize);
                     ArrayPointer<byte> realBuff = new ArrayPointer<byte>(realBufferSize);
                    //Resize
                     try
                     {
                         status = IppFunctions64.ippiResizeCubic_8u_C1R(pSrc, srcStep, pDst, dstStep, new IppiPoint { x = 0, y = 0 }, dstRoiSize, IppiBorderType.ippBorderInMem, IntPtr.Zero, pSp.Pointer, realBuff.Pointer);
                     }
                     finally {
                         buffInit.Release();
                         pSp.Release();
                         realBuff.Release();
                     }

                    
                    break;
                default:
                     status = IppFunctions64.ippiResizeGetSize_8u(srcSize, dstRoiSize, IppiInterpolationType.ippCubic, 0, out specSize, out intBufferSize);
                     ArrayPointer<byte> buffInitd = new ArrayPointer<byte>(intBufferSize);
                     ArrayPointer<float> pSpd = new ArrayPointer<float>(specSize);

                     status = IppFunctions64.ippiResizeCubicInit_8u(srcSize, dstRoiSize, 1, 2, pSpd.Pointer, buffInitd.Pointer);
                     status = IppFunctions64.ippiResizeGetBufferSize_8u(pSpd.Pointer, dstRoiSize, 1, out realBufferSize);
                     ArrayPointer<byte> realBuffd = new ArrayPointer<byte>(realBufferSize);
                    //Resize
                     try
                     {
                         status = IppFunctions64.ippiResizeCubic_8u_C1R(pSrc, srcStep, pDst, dstStep, new IppiPoint { x = 0, y = 0 }, dstRoiSize, IppiBorderType.ippBorderInMem, IntPtr.Zero, pSpd.Pointer, realBuffd.Pointer);
                     }
                     finally {
                         buffInitd.Release();
                         pSpd.Release();
                         realBuffd.Release();
                     }
                    break;
            }
            try
            {
                //bufferPtrHandle.Free();
                //pSpecPtrHandle.Free();
            }
            catch { }
            return status;
        }

        /// <summary>Изменение размера изображения 8 бит, 3 канала. Не обработано.</summary>
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
        public static IppStatus ippiResize_8u_C3R(byte[] pSrc, IppiSize srcSize, int srcStep, IppiRect srcRoi,
                                                         byte[] pDst, int dstStep, IppiSize dstRoiSize,
                                                         double xFactor, double yFactor, IppInterpolation interpolation)
        {
            if(IntPtr.Size==4)return IppFunctions32.ippiResize_8u_C3R(pSrc, srcSize, srcStep, srcRoi, pDst, dstStep, dstRoiSize, xFactor, yFactor, interpolation);
            IppStatus status = IppStatus.ippStsNoErr;
            
            int specSize = 0;
            int intBufferSize = 0;
            int realBufferSize = 0;
            IntPtr bufferPtr = IntPtr.Zero;
            IntPtr pSpecPtr = IntPtr.Zero;
            switch (interpolation)
            {
                case IppInterpolation.IPPI_INTER_LINEAR:
                    ////Resize
                    ArrayPointer<float> pSpL = new ArrayPointer<float>(specSize);
                    status = IppFunctions64.ippiResizeLinearInit_8u(srcSize, dstRoiSize, pSpL.Pointer);
                    status = IppFunctions64.ippiResizeGetBufferSize_8u(pSpL.Pointer, dstRoiSize, 3, out realBufferSize);
                    ArrayPointer<byte> realBuffL = new ArrayPointer<byte>(realBufferSize);
                    //Resize
                    try
                    {
                        status = IppFunctions64.ippiResizeLinear_8u_C3R(pSrc, srcStep, pDst, dstStep, new IppiPoint { x = 0, y = 0 }, 
                                                dstRoiSize, IppiBorderType.ippBorderInMem, IntPtr.Zero, pSpL.Pointer, realBuffL.Pointer);
                    }
                    finally
                    {
                        pSpL.Release();
                        realBuffL.Release();
                    }
                    break;
                case IppInterpolation.IPPI_INTER_CUBIC:
                     default:
                    status = IppFunctions64.ippiResizeGetSize_8u(srcSize, dstRoiSize, IppiInterpolationType.ippCubic, 0, out specSize, out intBufferSize);
                    ArrayPointer<byte> buffInit = new ArrayPointer<byte>(intBufferSize);
                    ArrayPointer<float> pSp = new ArrayPointer<float>(specSize);

                    status = IppFunctions64.ippiResizeCubicInit_8u(srcSize, dstRoiSize, 1, 2, pSp.Pointer, buffInit.Pointer);
                    status = IppFunctions64.ippiResizeGetBufferSize_8u(pSp.Pointer, dstRoiSize, 3, out realBufferSize);
                    ArrayPointer<byte> realBuff = new ArrayPointer<byte>(realBufferSize);
                    //Resize
                    try
                    {
                        status = IppFunctions64.ippiResizeCubic_8u_C3R(pSrc, srcStep, pDst, dstStep, new IppiPoint { x = 0, y = 0 }, dstRoiSize, 
                                                                        IppiBorderType.ippBorderInMem, IntPtr.Zero, pSp.Pointer, realBuff.Pointer);
                    }
                    finally
                    {
                        buffInit.Release();
                        pSp.Release();
                        realBuff.Release();
                    }
                    break;
            }
            return status;
        }

        /// <summary>Изменение размера изображения 8 бит, 4 канала. Не обработано.</summary>
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
        public static IppStatus ippiResize_8u_C4R(byte[] pSrc, IppiSize srcSize, int srcStep, IppiRect srcRoi,
                                                         byte[] pDst, int dstStep, IppiSize dstRoiSize,
                                                         double xFactor, double yFactor, IppInterpolation interpolation)
        {
           if(IntPtr.Size==4) return IppFunctions32.ippiResize_8u_C4R(pSrc, srcSize, srcStep, srcRoi, pDst, dstStep, dstRoiSize, xFactor, yFactor, interpolation);
            IppStatus status = IppStatus.ippStsNoErr;
            int specSize = 0;
            int intBufferSize = 0;
            int realBufferSize = 0;
            switch (interpolation)
            {
                case IppInterpolation.IPPI_INTER_LINEAR:
                    ////Resize
                    ArrayPointer<float> pSpL = new ArrayPointer<float>(specSize);
                    status = IppFunctions64.ippiResizeLinearInit_8u(srcSize, dstRoiSize, pSpL.Pointer);
                    status = IppFunctions64.ippiResizeGetBufferSize_8u(pSpL.Pointer, dstRoiSize, 4, out realBufferSize);
                    ArrayPointer<byte> realBuffL = new ArrayPointer<byte>(realBufferSize);
                    //Resize
                    try
                    {
                        status = IppFunctions64.ippiResizeLinear_8u_C4R(pSrc, srcStep, pDst, dstStep, new IppiPoint { x = 0, y = 0 },
                                                dstRoiSize, IppiBorderType.ippBorderInMem, IntPtr.Zero, pSpL.Pointer, realBuffL.Pointer);
                    }
                    finally
                    {
                        pSpL.Release();
                        realBuffL.Release();
                    }
                    break;
                case IppInterpolation.IPPI_INTER_CUBIC:
                default:
                    status = IppFunctions64.ippiResizeGetSize_8u(srcSize, dstRoiSize, IppiInterpolationType.ippCubic, 0, out specSize, out intBufferSize);
                    ArrayPointer<byte> buffInit = new ArrayPointer<byte>(intBufferSize);
                    ArrayPointer<float> pSp = new ArrayPointer<float>(specSize);

                    status = IppFunctions64.ippiResizeCubicInit_8u(srcSize, dstRoiSize, 1, 2, pSp.Pointer, buffInit.Pointer);
                    status = IppFunctions64.ippiResizeGetBufferSize_8u(pSp.Pointer, dstRoiSize, 4, out realBufferSize);
                    ArrayPointer<byte> realBuff = new ArrayPointer<byte>(realBufferSize);
                    //Resize
                    try
                    {
                        status = IppFunctions64.ippiResizeCubic_8u_C4R(pSrc, srcStep, pDst, dstStep, new IppiPoint { x = 0, y = 0 }, dstRoiSize,
                                                                        IppiBorderType.ippBorderInMem, IntPtr.Zero, pSp.Pointer, realBuff.Pointer);
                    }
                    finally
                    {
                        buffInit.Release();
                        pSp.Release();
                        realBuff.Release();
                    }
                    break;
            }
            return status;

        }

        /// <summary>Изменение размера изображения 8 бит, 4 канала. Не обработано. </summary>
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
        public static IppStatus ippiResize_8u_C4R(byte[] pSrc, IppiSize srcSize, int srcStep, IppiRect srcRoi,
                                                         IntPtr pDst, int dstStep, IppiSize dstRoiSize,
                                                         double xFactor, double yFactor, IppInterpolation interpolation)
        {
           if(IntPtr.Size==4) return IppFunctions32.ippiResize_8u_C4R(pSrc, srcSize, srcStep, srcRoi, pDst, dstStep, dstRoiSize, xFactor, yFactor, interpolation);
            IppStatus status = IppStatus.ippStsNoErr;
           
            int specSize = 0;
            int intBufferSize = 0;
            int realBufferSize = 0;
            switch (interpolation)
            {
                case IppInterpolation.IPPI_INTER_LINEAR:
                    ////Resize
                    ArrayPointer<float> pSpL = new ArrayPointer<float>(specSize);
                    status = IppFunctions64.ippiResizeLinearInit_8u(srcSize, dstRoiSize, pSpL.Pointer);
                    status = IppFunctions64.ippiResizeGetBufferSize_8u(pSpL.Pointer, dstRoiSize, 4, out realBufferSize);
                    ArrayPointer<byte> realBuffL = new ArrayPointer<byte>(realBufferSize);
                    //Resize
                    try
                    {
                        status = IppFunctions64.ippiResizeLinear_8u_C4R(pSrc, srcStep, pDst, dstStep, new IppiPoint { x = 0, y = 0 },
                                                dstRoiSize, IppiBorderType.ippBorderInMem, IntPtr.Zero, pSpL.Pointer, realBuffL.Pointer);
                    }
                    finally
                    {
                        pSpL.Release();
                        realBuffL.Release();
                    }
                    break;
                case IppInterpolation.IPPI_INTER_CUBIC:
                default:
                    status = IppFunctions64.ippiResizeGetSize_8u(srcSize, dstRoiSize, IppiInterpolationType.ippCubic, 0, out specSize, out intBufferSize);
                    ArrayPointer<byte> buffInit = new ArrayPointer<byte>(intBufferSize);
                    ArrayPointer<float> pSp = new ArrayPointer<float>(specSize);

                    status = IppFunctions64.ippiResizeCubicInit_8u(srcSize, dstRoiSize, 1, 2, pSp.Pointer, buffInit.Pointer);
                    status = IppFunctions64.ippiResizeGetBufferSize_8u(pSp.Pointer, dstRoiSize, 4, out realBufferSize);
                    ArrayPointer<byte> realBuff = new ArrayPointer<byte>(realBufferSize);
                    //Resize
                    try
                    {
                        status = IppFunctions64.ippiResizeCubic_8u_C4R(pSrc, srcStep, pDst, dstStep, new IppiPoint { x = 0, y = 0 }, dstRoiSize,
                                                                        IppiBorderType.ippBorderInMem, IntPtr.Zero, pSp.Pointer, realBuff.Pointer);
                    }
                    finally
                    {
                        buffInit.Release();
                        pSp.Release();
                        realBuff.Release();
                    }
                    break;
            }
            return status;

        }

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
        public static IppStatus ippiRotateCenter_8u_C1R(byte[] pSrc,
                                           IppiSize srcSize, int srcStep, IppiRect srcRoi,
                                           byte[] pDst, int dstStep, IppiRect dstRoi, double angle,
                                           double xCenter, double yCenter, int interpolation)
        {
            if(IntPtr.Size==4)
                return IppFunctions32.ippiRotateCenter_8u_C1R(pSrc,
                                           srcSize, srcStep, srcRoi,
                                           pDst, dstStep, dstRoi, angle,
                                           xCenter, yCenter, interpolation);
            try
            {//GDI Rotate
                if (angle == 0 || angle == 360)
                {
                    pSrc.CopyTo(pDst, 0);
                }
                else
                {
                    MemoryStream ms = new MemoryStream(pSrc);
                    ms.Position = 0;
                    SD.Image img = SD.Image.FromStream(ms);
                    if (angle == 90)
                        img.RotateFlip(SD.RotateFlipType.Rotate90FlipNone);
                    if (angle == 180)
                        img.RotateFlip(SD.RotateFlipType.Rotate180FlipNone);
                    if (angle == 270)
                        img.RotateFlip(SD.RotateFlipType.Rotate270FlipNone);
                    ms.Close();
                    using (MemoryStream mss = new MemoryStream())
                    {
                        img.Save(mss, SD.Imaging.ImageFormat.Jpeg);
                        byte[] array = ms.GetBuffer();
                        array.CopyTo(pDst, 0);
                    }
                }

            }
            catch { return IppStatus.ippStsNoErr; }
            return IppStatus.ippStsNoErr;
        }

        /// <summary>Вычисление среднего значения изображения 8 бит 1 канал</summary>
        /// <param name="pSrc">Входное изображение</param>
        /// <param name="srcStep">Шаг строки в байтах</param>
        /// <param name="srcSize">Регион обработки</param>
        /// <param name="mean">Среднее значение точек</param>
        public static IppStatus ippiMean_8u_C1R(byte[] pSrc, int srcStep, IppiSize srcSize, ref double mean)
        {
            if (IntPtr.Size == 4) return IppFunctions32.ippiMean_8u_C1R(pSrc, srcStep, srcSize, ref mean);
            else
            {
                try { IppFunctions64.ippiMean_8u_C1R(pSrc, srcStep, srcSize, ref mean); }
                catch
                {
                    mean = 0;
                    for (int i = 0; i < pSrc.Length; i++)
                        mean += ((double)pSrc[i] / pSrc.Length);
                }
            }
            return IppStatus.ippStsNoErr;
        }

        /// <summary>(Перегрузка) Вычисление среднего значения изображения 8 бит 1 канал</summary>
        /// <param name="pSrc">Входное изображение</param>
        /// <param name="srcStep">Шаг строки в байтах</param>
        /// <param name="srcSize">Регион обработки</param>
        /// <param name="mean">Среднее значение точек</param>
        public static IppStatus ippiMean_8u_C1R(IntPtr pSrc, int srcStep, IppiSize srcSize, ref double mean)
        {
            if (IntPtr.Size == 4) return IppFunctions32.ippiMean_8u_C1R(pSrc, srcStep, srcSize, ref mean);
            else
            {
                try { IppFunctions64.ippiMean_8u_C1R(pSrc, srcStep, srcSize, ref mean); }
                catch
                {
                    mean = 0;
                    byte[] array = new byte[srcSize.width * srcSize.height];
                    Marshal.Copy(pSrc, array, 0, srcSize.width * srcSize.height);
                    for (int i = 0; i < array.Length; i++)
                        mean += ((double)array[i] / array.Length);
                }
            }
            return IppStatus.ippStsNoErr;

        }

        /// <summary>Преобразование изображения 8 бит/пиксель в 24 бит пиксель с
        /// заполнением цветного изображения пикселями серого</summary>
        /// <param name="pSrc">Входное изображение</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="pDst">Выходное цветное изображение</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="roiSize">Регион обработки</param>
        /// <returns>Результат операции</returns>
        public static IppStatus ippiDup_8u_C1C3R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiSize roiSize)
        {
            if (IntPtr.Size == 4) return IppFunctions32.ippiDup_8u_C1C3R(pSrc, srcStep, pDst, dstStep, roiSize);
            return IppFunctions64.ippiDup_8u_C1C3R(pSrc, srcStep, pDst, dstStep, roiSize);
        }

        /// <summary>Конвертация уветного изображения в серое с коэффициентами преобразования</summary>
        /// <param name="pSrc">Входное цветное изображение</param>
        /// <param name="srcStep">Шаг строки входного изображения в байтах</param>
        /// <param name="pDst">Выходное черно белое изображение</param>
        /// <param name="dstStep">Шаг строки выходного изображения в байтах</param>
        /// <param name="roiSize">Регион обработки</param>
        /// <param name="coeffs">Коэффицианты преобразования R G B</param>
        /// <returns>Результат операции</returns>
        public static IppStatus ippiColorToGray_8u_C3C1R(byte[] pSrc, int srcStep, byte[] pDst, int dstStep, IppiSize roiSize, float[] coeffs)
        {
            if (IntPtr.Size == 4) return IppFunctions32.ippiColorToGray_8u_C3C1R(pSrc, srcStep, pDst, dstStep, roiSize, coeffs);
            return IppFunctions64.ippiColorToGray_8u_C3C1R(pSrc, srcStep, pDst, dstStep, roiSize, coeffs);
        }

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
        public static IppStatus ippiRemap_8u_C1R(byte[] pSrc, IppiSize srcSize, int srcStep, IppiRect srcROI,
                                   float[,] pxMap, int xMapStep, float[,] pyMap, int yMapStep, byte[] pDst,
                                   int dstStep, IppiSize dstRoiSize, IppInterpolation interpolation)
        {
            if (IntPtr.Size == 4) return IppFunctions32.ippiRemap_8u_C1R(pSrc, srcSize, srcStep, srcROI,
                                                                         pxMap, xMapStep, pyMap, yMapStep, pDst,
                                                                         dstStep, dstRoiSize, interpolation);
            return IppFunctions64.ippiRemap_8u_C1R(pSrc, srcSize, srcStep, srcROI,
                                                   pxMap, xMapStep, pyMap, yMapStep, pDst,
                                                   dstStep, dstRoiSize, interpolation);
        }

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
        public static IppStatus ippiRemap_8u_C3R(byte[] pSrc, IppiSize srcSize, int srcStep, IppiRect srcROI,
                                   float[,] pxMap, int xMapStep, float[,] pyMap, int yMapStep, byte[] pDst,
                                   int dstStep, IppiSize dstRoiSize, IppInterpolation interpolation)
        {
            if (IntPtr.Size == 4) return IppFunctions32.ippiRemap_8u_C3R(pSrc, srcSize, srcStep, srcROI,
                                                                         pxMap, xMapStep, pyMap, yMapStep, pDst,
                                                                         dstStep, dstRoiSize, interpolation);
            return IppFunctions64.ippiRemap_8u_C3R(pSrc, srcSize, srcStep, srcROI,
                                                   pxMap, xMapStep, pyMap, yMapStep, pDst,
                                                   dstStep, dstRoiSize, interpolation);
        }

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
        public static IppStatus ippiRemap_8u_C4R(byte[] pSrc, IppiSize srcSize, int srcStep, IppiRect srcROI,
                                   float[,] pxMap, int xMapStep, float[,] pyMap, int yMapStep, byte[] pDst,
                                   int dstStep, IppiSize dstRoiSize, IppInterpolation interpolation)
        {
            if (IntPtr.Size == 4) return IppFunctions32.ippiRemap_8u_C4R(pSrc, srcSize, srcStep, srcROI,
                                                                         pxMap, xMapStep, pyMap, yMapStep, pDst,
                                                                         dstStep, dstRoiSize, interpolation);
            return IppFunctions64.ippiRemap_8u_C4R(pSrc, srcSize, srcStep, srcROI,
                                                   pxMap, xMapStep, pyMap, yMapStep, pDst,
                                                   dstStep, dstRoiSize, interpolation);
        }

        /// <summary>Получить размер буфера для маски исправлений искажений</summary>
        /// <param name="roiSize">Размер кадра</param>
        /// <param name="pBufsize">Получаемый размер буфера</param>
        /// <returns>Результат операции</returns>
        public static IppStatus ippiUndistortGetSize(IppiSize roiSize, ref int pBufsize)
        {
            if (IntPtr.Size == 4) return IppFunctions32.ippiUndistortGetSize(roiSize, ref pBufsize);
            try { IppFunctions64.ippiUndistortGetSize(roiSize, ref pBufsize); }
            catch { pBufsize = Marshal.SizeOf(roiSize); }
            return IppStatus.ippStsNoErr;
        }

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
        public static IppStatus ippiCreateMapCameraUndistort_32f_C1R(float[] pxMap, int xStep, float[] pyMap,
                                                  int yStep, IppiSize roiSize,
                                                  float fx, float fy, float cx, float cy,
                                                  double k1, double k2,
                                                  float p1, float p2, byte[] pBuffer)
        {
            if(IntPtr.Size == 4) return IppFunctions32.ippiCreateMapCameraUndistort_32f_C1R(pxMap, xStep, pyMap,
                                                                                            yStep, roiSize,
                                                                                            fx, fy, cx, cy,
                                                                                            k1, k2,
                                                                                            p1, p2, pBuffer);
            return IppFunctions64.ippiCreateMapCameraUndistort_32f_C1R(pxMap, xStep, pyMap,
                                                                       yStep, roiSize,
                                                                       fx, fy, cx, cy,
                                                                       k1, k2,
                                                                       p1, p2, pBuffer);
        }

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
        public static IppStatus ippiUndistortRadial_8u_C1R(byte[] pSrc, int srcStep, byte[] pDst,
                                       int dstStep, IppiSize roiSize,
                                       float fx, float fy, float cx, float cy,
                                       float k1, float k2, byte[] pBuffer)
        {
            if (IntPtr.Size == 4) return IppFunctions32.ippiUndistortRadial_8u_C1R(pSrc, srcStep, pDst,
                                                                                   dstStep, roiSize,
                                                                                   fx, fy, cx, cy,
                                                                                   k1, k2, pBuffer);
            return IppFunctions64.ippiUndistortRadial_8u_C1R(pSrc, srcStep, pDst,
                                                             dstStep, roiSize,
                                                             fx, fy, cx, cy,
                                                             k1, k2, pBuffer);
        }
        
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
        public static IppStatus ippiUndistortRadial_8u_C3R(byte[] pSrc, int srcStep, byte[] pDst,
                                       int dstStep, IppiSize roiSize,
                                       float fx, float fy, float cx, float cy,
                                       float k1, float k2, byte[] pBuffer)
        {
            if (IntPtr.Size == 4) return IppFunctions32.ippiUndistortRadial_8u_C3R(pSrc, srcStep, pDst,
                                                                                   dstStep, roiSize,
                                                                                   fx, fy, cx, cy,
                                                                                   k1, k2, pBuffer);
            return IppFunctions64.ippiUndistortRadial_8u_C3R(pSrc, srcStep, pDst,
                                                             dstStep, roiSize,
                                                             fx, fy, cx, cy,
                                                             k1, k2, pBuffer);
        }

        #region Функции исправления искажений "рыбий глаз" с использованием простых параметров

        /// <summary>Создание масок исправления искажений</summary>
        /// <param name="Coeff">Коэффициент исправления искажений</param>
        /// <param name="width">Ширина</param>
        /// <param name="height">Высота</param>
        /// <param name="xMap">Массив смещений по горизонтали</param>
        /// <param name="yMap">Массив смещений по вертикали</param>
        public static void CreateUnFishMap24bit(float Coeff, int width, int height, out float[,] xMap, out float[,] yMap)
        {
            xMap = new float[height, width];
            yMap = new float[height, width];
            if (Coeff == 0) return;//Если коэффициент исправлений искажений  = 0 выход
            //расчет коэффициентов
            double d, a;
            //Половины ширины/высота для расчета радиусов
            double rx = width / 2;
            double ry = height / 2;
            double R = rx > ry ? ry : rx - 4;
            int Rrun = (int)Math.Round((Math.Sqrt((rx * rx) + (ry * ry))));//радиус пробега = от центра до угла изображения
            double sh = R * Coeff; //distortion, коэффициент искажения
            double sr = (sh * sh + R * R) / sh / 2.0;
            double aw = 1.0 / Math.Atan(R / sr);
            // Создание маски анти-рыбий глаз
            double new_x, new_y;//Рассчитываемые значения смещений
            int x = 0, y = 0;
            for (int i = 0; i < height; i++)//Высота
            {
                for (int j = 0; j < width; j++)//Ширина
                {
                    d = Math.Sqrt((j - rx) * (j - rx) + (i - ry) * (i - ry));
                    if ((d < Rrun) && (d > 0))
                    {
                        a = Math.Atan(d / sr) * aw * R / d;
                        new_x = rx + (j - rx) * a;// +x;
                        new_y = ry + (i - ry) * a;// +y;
                    }
                    else
                    {
                        new_x = j;
                        new_y = i;
                    }
                    xMap[y, x] = (float)(new_x);
                    yMap[y, x] = (float)(new_y);
                    x++;//Следующий столбец
                }
                x = 0;
                y++;
            }
        }        
        
        /// <summary>Исправление искажений 24-битного изображения 
        /// с уже готовой маской искажений</summary>
        /// <param name="pIn">Входной массив</param>
        /// <param name="width">Ширина кадра</param>
        /// <param name="height">Высота кадра</param>
        /// <param name="xMap">Массив смещений по горизонтали</param>
        /// <param name="yMap">Массив смещений по вертикали</param>
        /// <returns>Выходное изображение</returns>
        public static byte[] AntiFishEye24bit(byte[] pIn, int width, int height, float[,] xMap, float[,] yMap)
        {
            int srcStep = width * 3;        //Длина строки в байтах
            int dstStep = srcStep;      //Длина строки в байтах
            byte[] data = new byte[width * height * 3];
            //Области обработки
            IppiSize roiSize;
            roiSize.width = width;
            roiSize.height = height;
            IppiRect rect;
            rect.x = 0; rect.y = 0;
            rect.width = roiSize.width; rect.height = roiSize.height;
            //Перестановка байт
            int m_xMapStep = width * 4;
            int m_yMapStep = width * 4;
            IppStatus g = ippiRemap_8u_C3R(pIn, roiSize, srcStep, rect,
                                           xMap, m_xMapStep, yMap, m_yMapStep, data, dstStep,
                                           roiSize, IppInterpolation.IPPI_INTER_LINEAR); //Трансформация с уже готовой маской
            return data;
        }

        /// <summary>Исправление искажений 24-битного изображения 
        /// с уже готовой маской искажений</summary>
        /// <param name="pIn">Входной массив</param>
        /// <param name="width">Ширина кадра</param>
        /// <param name="height">Высота кадра</param>
        /// <param name="xMap">Массив смещений по горизонтали</param>
        /// <param name="yMap">Массив смещений по вертикали</param>
        /// <returns>Выходное изображение</returns>
        public static byte[] AntiFishEye32bit(byte[] pIn, int width, int height, float[,] xMap, float[,] yMap)
        {
            int srcStep = width * 4;    //Длина строки в байтах
            int dstStep = srcStep;      //Длина строки в байтах
            byte[] data = new byte[width * height * 4];
            //Области обработки
            IppiSize roiSize;
            roiSize.width = width;
            roiSize.height = height;
            IppiRect rect;
            rect.x = 0; rect.y = 0;
            rect.width = roiSize.width; rect.height = roiSize.height;
            //Перестановка байт
            int m_xMapStep = width * 4;
            int m_yMapStep = width * 4;
            IppStatus g = ippiRemap_8u_C4R(pIn, roiSize, srcStep, rect,
                                           xMap, m_xMapStep, yMap, m_yMapStep, data, dstStep,
                                           roiSize, IppInterpolation.IPPI_INTER_LINEAR); //Трансформация с уже готовой маской
            return data;
        }

        /// <summary>Исправление искажений 8-битного изображения 
        /// с уже готовой маской искажений</summary>
        /// <param name="pIn">Входной массив</param>
        /// <param name="width">Ширина кадра</param>
        /// <param name="height">Высота кадра</param>
        /// <param name="xMap">Массив смещений по горизонтали</param>
        /// <param name="yMap">Массив смещений по вертикали</param>
        /// <returns>Выходное изображение</returns>
        public static byte[] AntiFishEye8bit(byte[] pIn, int width, int height, float[,] xMap, float[,] yMap)
        {
            int srcStep = width;        //Длина строки в байтах
            int dstStep = srcStep;      //Длина строки в байтах
            byte[] data = new byte[width * height];
            //Области обработки
            IppiSize roiSize;
            roiSize.width = width;
            roiSize.height = height;
            IppiRect rect;
            rect.x = 0; rect.y = 0;
            rect.width = roiSize.width; rect.height = roiSize.height;
            //Перестановка байт
            int m_xMapStep = width * 4;
            int m_yMapStep = width * 4;
            IppStatus g = ippiRemap_8u_C1R(pIn, roiSize, srcStep, rect,
                                           xMap, m_xMapStep, yMap, m_yMapStep, data, dstStep,
                                           roiSize, IppInterpolation.IPPI_INTER_LINEAR); //Трансформация с уже готовой маской
            return data;
        }
        
        /// <summary>Исправление искажений 8-битного изображения 
        /// с уже готовой маской искажений</summary>
        /// <param name="pIn">Входной массив</param>
        /// <param name="width">Ширина кадра</param>
        /// <param name="height">Высота кадра</param>
        /// <param name="xMap">Массив смещений по горизонтали</param>
        /// <param name="yMap">Массив смещений по вертикали</param>
        /// <returns>Выходное изображение</returns>
        public static byte[] AntiFishEye8bitByData(byte[] pIn, int width, int height, float[,] xMap, float[,] yMap)
        {
            int srcStep = width;        //Длина строки в байтах
            int dstStep = srcStep;      //Длина строки в байтах
            byte[] data = new byte[width * height];
            //Области обработки
            IppiSize roiSize;
            roiSize.width = width;
            roiSize.height = height;
            IppiRect rect;
            rect.x = 0; rect.y = 0;
            rect.width = roiSize.width; rect.height = roiSize.height;
            //Перестановка байт
            int m_xMapStep = width;
            int m_yMapStep = width;
            IppStatus g = ippiRemap_8u_C1R(pIn, roiSize, srcStep, rect,
                                           xMap, m_xMapStep, yMap, m_yMapStep, data, dstStep,
                                           roiSize, IppInterpolation.IPPI_INTER_LINEAR); //Трансформация с уже готовой маской
            return data;
        }
        #endregion

    }

}
