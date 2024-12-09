using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASKO = AlfaPribor.ASKO.DataProvider;
using ASKO_Data = AlfaPribor.ASKO.Data;

namespace FramesPlayer.ExportConfiguration
{
    public class AskoDataProvider : IGetWagonData
    {

        #region Члены IGetWagonData

        public IList<WagonDataShort> GetWagonList(int trainID)
        {
            ASKO.DataProvider askoProvider = new ASKO.DataProvider(SettingContainer.ConnectionSettings.BuildConnectionString());
            List<WagonDataShort> resultList = askoProvider.GetWagonList(trainID).Select(
                        x => new WagonDataShort
                        {
                            WagId = x.WagId,
                            Sn = x.Sn,
                            SnSost = x.Sn,
                            InvNumber = x.InvNumber,
                            InvNumByNL = x.InvNumByNL,
                            TimeSpan = x.TimeSpanEnd,
                            TimeSpanBegin = x.TimeSpanBegin,
                            SpeedBegin = x.SpeedBegin,
                            SpeedEnd = x.SpeedEnd
                        }
                ).ToList();
            return resultList;
        }

        #endregion



        public bool CheckConnection()
        {
            ASKO.DataProvider askoProvider = new ASKO.DataProvider(SettingContainer.ConnectionSettings.BuildConnectionString());
            return askoProvider.CheckConnect() && askoProvider.CheckDBversion();
        }
    }
}
