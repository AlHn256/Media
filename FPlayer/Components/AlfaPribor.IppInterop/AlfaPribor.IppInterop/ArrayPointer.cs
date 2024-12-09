using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace AlfaPribor.IppInterop
{
   public class ArrayPointer<T>
    {
       private IntPtr _innerPointer;
       private T[] _array;
       private GCHandle _handle;
       public IntPtr Pointer
       {
           get { return _innerPointer;}
           set { _innerPointer = value; }
       }
       public ArrayPointer() { }
       public ArrayPointer(int size) {
           Create(size);
        
       }

       public void Create(int size)
       {
           _array = new T[size];
           _handle = GCHandle.Alloc(_array,GCHandleType.Pinned);
           _innerPointer = _handle.AddrOfPinnedObject();
       }
       public void Release(){
           try
           {
               _handle.Free();
               _array = null;
           }
           catch { }
       }
    }
}
