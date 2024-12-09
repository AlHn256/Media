using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace AlfaPribor.SharpDXVideoRenderer
{

    /// <summary>Служебный класс работы с массивами байт</summary>
    class ClassByteUtils
    {

        /// <summary>Преобразование объекта в массив байт</summary>
        /// <param name="obj">Объект</param>
        /// <returns>Массив байт</returns>
        public static byte[] StructureToByteArray(object obj)
        {
            int len = Marshal.SizeOf(obj);
            byte[] arr = new byte[len];
            IntPtr ptr = Marshal.AllocHGlobal(len);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, len);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        /// <summary>Преобразование объекта заданного размера в массив байт</summary>
        /// <param name="obj">Объект</param>
        /// <returns>Массив байт</returns>
        public static byte[] StructureToByteArray(string str, int len)
        {
            byte[] arr = new byte[len];
            IntPtr ptr = Marshal.AllocHGlobal(len);
            ptr = Marshal.StringToHGlobalAnsi(str);
            Marshal.Copy(ptr, arr, 0, len);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

    }

}
