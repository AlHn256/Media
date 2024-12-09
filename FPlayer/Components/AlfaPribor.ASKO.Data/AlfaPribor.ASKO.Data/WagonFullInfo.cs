using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.ASKO.Data
{
    public class WagonFullInfo
    {
        public WagonCargoDataShort WagonShortInfo { get; set; }
        public WagonData WagonInfo { get; set; }

        public WagonFullInfo()
        {
            WagonShortInfo = new WagonCargoDataShort();
            WagonInfo = new WagonData();
        }
    }
}
