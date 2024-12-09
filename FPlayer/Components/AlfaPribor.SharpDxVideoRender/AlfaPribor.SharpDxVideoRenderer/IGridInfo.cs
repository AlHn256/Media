using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.SharpDXVideoRenderer
{

    /// <summary>Интерфейс параметров сетки</summary>
    public interface IGridSettingInfo
   {
       double a { get; }
       double b { get; }
   }

   /// <summary>Параметры сетки</summary>
   public class GridSettingBase : IGridSettingInfo
   {
       public GridSettingBase()
       {
            _a = 640.0f;
            _b = 480.0f;
       }

       protected double _a;
       protected double _b;

       public double a { get { return _a; } }
       public double b { get { return _b; } }
   }

    /// <summary>Параметры сетки для не повернутого изображения</summary>
    public class DirectGridSetting : GridSettingBase
    {
        //public DirectGridSetting() : base() { _a = 1280.0f; _b = 960.0f; }
        //public DirectGridSetting() : base() { _a = 800.0f; _b = 600.0f; }
        public DirectGridSetting() : base() { _a = 640.0f; _b = 480.0f; }
        //public DirectGridSetting() : base() { _a = 640.0f; _b = 360.0f; }
        //public DirectGridSetting() : base() { _a = 640.0f; _b = 640.0f; }
    }

    /// <summary>Параметры сетки для повернутого изображения</summary>
    public class RotationGridSetting : GridSettingBase
    {
        //public RotationGridSetting() : base() { _a = 960.0f; _b = 1280.0f; }
        public RotationGridSetting() : base() { _a = 480.0f; _b = 640.0f; }
        //public RotationGridSetting() : base() { _a = 640.0f; _b = 640.0f; }
    }

}
