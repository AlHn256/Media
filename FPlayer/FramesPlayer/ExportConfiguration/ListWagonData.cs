﻿using System.Collections.Generic;

namespace FramesPlayer.ExportConfiguration
{
    public class ListWagonData
    {
        public  int TrainID { get; set; }
        
        IList<WagonDataShort> _innerListWagonData;

        public  IList<WagonDataShort> WagonDataList
        {
            get
            {
                if (_innerListWagonData == null)
                    _innerListWagonData = SettingContainer.WagonDataProvider.GetWagonList(TrainID);  
                return _innerListWagonData;
            }
        }
        
        public void ReInit(int trainID)
        {
            TrainID = trainID;
           _innerListWagonData = SettingContainer.WagonDataProvider.GetWagonList(TrainID);
        }

        public ListWagonData(int trainID)
        {
            TrainID = trainID;
        }

    }
}
