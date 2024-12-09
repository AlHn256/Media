using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AlfaPribor.ASKO.Data
{
    /// <summary>Результат сравнения моделей </summary>
    public class CompareCargoResult
    {
        /// <summary>Исходные точки. Точки модели, движущияся с большей скоростью без изменения </summary>
        public List<Tuple<long, Point[]>> SourcePoints { get; set; }
        /// <summary>Массив различий между точками  </summary>
        public List<Tuple<long, Point[]>> DiffPoints { get; set; }
        /// <summary>Несоответствия </summary>
        public Dictionary<int, PointF[]> DiffOversizes { get; set; }
        //Расчёты смещения центра масс
        public CargoResultData DiffCargoResult { get; set; }

        public CompareCargoResult()
        {
            SourcePoints = new List<Tuple<long, Point[]>>();
            DiffPoints = new List<Tuple<long, Point[]>>();
            DiffOversizes = new Dictionary<int, PointF[]>();
            DiffCargoResult = new CargoResultData();
        }
    }
}
