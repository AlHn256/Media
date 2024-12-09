using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AlfaPribor.ASKO.Data
{
    public class CargoTotalResultItem
    {
        public int TrainID { get; set; }
        public int WagonSN { get; set; }
        public long CargoStrartTime { get; set; }
        public long CargoEndTime { get; set; }
        /*Количество точек модели*/
        public int VertexCount { get; set; }

        /*Model*/
        public List<Tuple<long, PointF[]>> Vertex { get; set; }
        public Dictionary<int, PointF[]> OversizeVertex { get; set; }

        public CargoResultDataEx CalculationResult { get; set; }

        public CargoTotalResultItem()
        {
            Vertex = new List<Tuple<long, PointF[]>>();
            OversizeVertex = new Dictionary<int, PointF[]>();

        }
    }

    //Данные, которые пойдут на сравнение.
    public class CaroCompareTotalResult
    {
        public string RemoteServerIP { get; set; }
        public int RemoteServerPort { get; set; }
        public CargoTotalResultItem SourceModel { get; set; }
        public CargoTotalResultItem RemoteModel { get; set; }

        public CaroCompareTotalResult() { }

    }
}
