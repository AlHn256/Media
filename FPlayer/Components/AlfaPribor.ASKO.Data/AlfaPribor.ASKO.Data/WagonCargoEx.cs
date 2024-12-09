using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.ASKO.Data
{
    public class WagonCargoDataShort
    {
        public int WagonID { get; set; }
        public long TimeSpanBegin { get; set; }
        public long TimeSpanEnd { get; set; }
        public long SpeedBegin { get; set; }
        public long SpeedEnd { get; set; }
        public int DirectionBegin { get; set; }
        public int DirectionEnd { get; set; }

        public long LastSpeedEnd { get; set; }
        public long NextSpeedBegin { get; set; }

        public long ApproximateLen
        {
            get
            {
                return (long)((TimeSpanEnd - TimeSpanBegin) * (SpeedBegin + SpeedEnd) / (2 * 3.6));
            }
        }
        /*Дополнительные поля, необходимые для ведения статистики*/
        public int WagonSn { get; set; }
        public int TrainID { get; set; }
        public DateTime TrainStartDate { get; set; }
        public string TrainIdex { get; set; }
        public string TrainNumber { get; set; }

        public WagonCargoDataShort() { }
    }

    

}
