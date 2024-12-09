using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.IppInterop
{
    /// <summary>Тип границы для 64 бит</summary>
    public enum IppiBorderType
    {
        ippBorderConst = 0,
        ippBorderRepl = 1,
        ippBorderWrap = 2,
        ippBorderMirror = 3,
        ippBorderMirrorR = 4,
        ippBorderMirror2 = 5,
        ippBorderInMem = 6,
        ippBorderInMemTop = 0x0010,
        ippBorderInMemBottom = 0x0020,
        ippBorderInMemLeft = 0x0040,
        ippBorderInMemRight = 0x0080
    } ;

    /// <summary>Тип фильтра</summary>
    public enum enumFilter
    {
        /// <summary>Отсутствует</summary>
        None = 0,
        /// <summary>Медианный</summary>
        Median = 1,
        /// <summary>Box</summary>
        Box = 2,
        /// <summary>Фильтр Собеля</summary>
        Sobel = 3,
        /// <summary>Фильтр Робертса</summary>
        Roberts = 4,
        /// <summary>Фильтр Прюитта</summary>
        Prewitt = 5,
        /// <summary>Фильтр Лапласа</summary>
        Laplace = 6,
        /// <summary>Фильтр Гаусса</summary>
        Gauss = 7,
        /// <summary>Фильтр Hipass</summary>
        Hipass = 8,
        /// <summary>Фильтр Lowpass</summary>
        Lowpass = 9,
        /// <summary>Фильтр четкости</summary>
        Sharpen = 10,
        /// <summary>Фильтр "Максимум"</summary>
        Max = 11,
        /// <summary>Фильтр "Минимум"</summary>
        Min = 12,
    }

    /// <summary>Статус выполнения функции</summary>
    public enum IppStatus
    {
        /// <summary>Успешно</summary>
        ippStsNoErr = 0,
    };

    /// <summary>Размер маски</summary>
    public enum IppiMaskSize
    {
        /// <summary>1 х 3</summary>
        ippMaskSize1x3 = 13,
        /// <summary>1 х 5</summary>
        ippMaskSize1x5 = 15,
        /// <summary>3 х 1</summary>
        ippMaskSize3x1 = 31,
        /// <summary>3 х 3</summary>
        ippMaskSize3x3 = 33,
        /// <summary>5 х 1</summary>
        ippMaskSize5x1 = 51,
        /// <summary>5 х 5</summary>
        ippMaskSize5x5 = 55,
    };

    /// <summary>Интерполяция</summary>
    //public enum IppInterpolation
    //{
    //    /// <summary>Отсутствует</summary>
    //    IPPI_INTER_NN = 1,
    //    /// <summary>Линейная</summary>
    //    IPPI_INTER_LINEAR = 2,
    //    /// <summary>Кубическая</summary>
    //    IPPI_INTER_CUBIC = 4,
    //    /// <summary>Улучшенная</summary>
    //    IPPI_INTER_SUPER = 8,
    //    /// <summary>Краевая</summary>
    //    IPPI_SMOOTH_EDGE = (1 << 31),
    //};

   public enum IppInterpolation {
    IPPI_INTER_NN     = 1,
    IPPI_INTER_LINEAR = 2,
    IPPI_INTER_CUBIC  = 4,
    IPPI_INTER_CUBIC2P_BSPLINE,     /* two-parameter cubic filter (B=1, C=0) */
    IPPI_INTER_CUBIC2P_CATMULLROM,  /* two-parameter cubic filter (B=0, C=1/2) */
    IPPI_INTER_CUBIC2P_B05C03,      /* two-parameter cubic filter (B=1/2, C=3/10) */
    IPPI_INTER_SUPER  = 8,
    IPPI_INTER_LANCZOS = 16,
    IPPI_ANTIALIASING  = (1 << 29),
    IPPI_SUBPIXEL_EDGE = (1 << 30),
    IPPI_SMOOTH_EDGE   = (1 << 31)
};

    public enum IppiInterpolationType{
        ippNearest = IppInterpolation.IPPI_INTER_NN,
        ippLinear = IppInterpolation.IPPI_INTER_LINEAR,
        ippCubic = IppInterpolation.IPPI_INTER_CUBIC2P_CATMULLROM,
        ippLanczos = IppInterpolation.IPPI_INTER_LANCZOS,
        ippHahn = 0,
        ippSuper = IppInterpolation.IPPI_INTER_SUPER
};


    /// <summary>Режим округления</summary>
    public enum IppRoundMode
    {
        /// <summary>Отсечение</summary>
        ippRndZero = 0,
        /// <summary>Ближайшее</summary>
        ippRndNear = 1,
    };

    /// <summary>Ось поворота</summary>
    public enum IppiAxis
    {
        /// <summary>Горизонтальная</summary>
        ippAxsHorizontal = 0,
        /// <summary>Вертикальная</summary>
        ippAxsVertical = 1,
        /// <summary>Горизонтальная и вертикальная</summary>
        ippAxsBoth = 2,
    }
      
}
