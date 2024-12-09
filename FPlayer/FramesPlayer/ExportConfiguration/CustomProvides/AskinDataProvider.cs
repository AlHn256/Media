using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlfaPribor.ASKIN.Data;
using AlfaPribor.ASKIN.DataProvider;

namespace FramesPlayer.ExportConfiguration
{
    public class AskinDataProvider : IGetWagonData
    {
        #region Члены IGetWagonData

        public IList<WagonDataShort> GetWagonList(int trainID)
        {
            try
            {
                return (new DataProvider(SettingContainer.ConnectionSettings.BuildConnectionString())).GetWagonList(trainID).Select(
                            x => new WagonDataShort
                            {
                                WagId = x.WagId,
                                Sn = x.Sn,
                                SnSost = x.SnSost,
                                InvNumber = x.InvNumber,
                                InvNumByNL = x.InvNumByNL,
                                Accuracy = x.Accuracy,
                                TimeSpan = x.TimeSpan,
                                TimeSpanBegin = x.TimeSpanBegin,
                                SpeedBegin = x.Speed,
                                SpeedEnd = x.Speed
                            }
                    ).ToList();
            }
            catch {};
            return null;
        }
        #endregion


        public bool CheckConnection()
        {
              var provider = new DataProvider(SettingContainer.ConnectionSettings.BuildConnectionString());
              return  provider.CheckConnect() && provider.CheckDBversion();
        }
    }
}
