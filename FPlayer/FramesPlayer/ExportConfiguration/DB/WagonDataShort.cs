using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace FramesPlayer.ExportConfiguration
{

    /// <summary>Данные о вагоне</summary>
    public class WagonDataShort
    {

        /// <summary>Идентификатор вагона</summary>
        public int WagId { get; set; }
        /// <summary>Номер пересечения</summary>
        public int Sn {get;set;}
        /// <summary>Номер в составе</summary>
        public int SnSost { get; set; }
        /// <summary>Инвентарный номер</summary>
        public string InvNumber { get; set; }
        /// <summary>Инвентарный номер в натурном листе</summary>
        public string InvNumByNL { get; set; }
        /// <summary>Достоверность распознавания</summary>
        public int Accuracy{get;set;}
        /// <summary>Метка времени окончания</summary>
        public int TimeSpan{get;set;}
        /// <summary>Метка времени начала</summary>
        public int TimeSpanBegin{get;set;}
        /// <summary>Метка времени начала</summary>
        public int SpeedBegin { get; set; }
        /// <summary>Метка времени начала</summary>
        public int SpeedEnd { get; set; }
       
        public WagonDataShort()
        {
            InvNumber = string.Empty;
            InvNumByNL = string.Empty;
        }

        public override string ToString()
        {
          return String.Format("{0} {1} {2} {3} {4}",
                               Sn,
                               SnSost,
                               string.IsNullOrEmpty(InvNumber) ? "_" : InvNumber,
                               Accuracy,
                               string.IsNullOrEmpty(InvNumByNL) ? "_" : InvNumByNL
                              );
        }
            
        public string MakeHeaderString()
        {
            return String.Format("{0} {1} {2} {3}", "Sn", "InvNumber", "Accuracy", "InvNumByNL");
        }

    }
}
