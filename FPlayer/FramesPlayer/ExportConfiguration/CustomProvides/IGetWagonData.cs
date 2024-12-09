using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlfaPribor.ASKIN.Data;

namespace FramesPlayer.ExportConfiguration
{

    public interface IGetWagonData
    {
        IList<WagonDataShort> GetWagonList(int trainID);
        bool CheckConnection();
    }

}
