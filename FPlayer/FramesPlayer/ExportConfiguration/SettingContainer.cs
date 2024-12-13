using System.Collections.Generic;
using FramesPlayer.DataTypes;
using ASKO_DP = AlfaPribor.ASKIN.DataProvider;

namespace FramesPlayer.ExportConfiguration
{

    public class SettingContainer
    {
        public static string VideoFileName { get; set; }
        public static string VideoFileFullName { get; set; }

        public static ProviderType DatabaseProviderType 
        {
            get { return (ProviderType)FramePlayerSettings.Default.ProviderType; } 
            set {
                try
                {
                    FramePlayerSettings.Default.ProviderType = (int)value;
                    FramePlayerSettings.Default.Save();
                }
                catch { }
            }
        }

        static DatabaseConnectionSettings _connectionSettings;

        public static DatabaseConnectionSettings ConnectionSettings
        {
            get
            {
                if (_connectionSettings == null)
                {
                    _connectionSettings = new DatabaseConnectionSettings();
                    _connectionSettings.Load();
                }
                return _connectionSettings;
            }
        }
       
        static ASKO_DP.DataProvider _innerDataBaseProvider;

        public static ASKO_DP.DataProvider DataBaseProvider 
        {
            get
            {
                if (_innerDataBaseProvider == null)
                {
                    _innerDataBaseProvider = new ASKO_DP.DataProvider(ConnectionSettings.BuildConnectionString());
                }
                return _innerDataBaseProvider;
            }
        }

        public static void ReInitDataBaseProvider()
        {
            _innerDataBaseProvider = new ASKO_DP.DataProvider(ConnectionSettings.BuildConnectionString());
        }

        private static IGetWagonData _wagonDataProvider;

        public static IGetWagonData WagonDataProvider
        {
            get
            {
                if (_wagonDataProvider == null)
                {
                    if (DatabaseProviderType == ProviderType.ASKIN)
                        _wagonDataProvider = new AskinDataProvider();
                    else
                        _wagonDataProvider = new AskoDataProvider();
                }
                return _wagonDataProvider;
            }
        }

        public static void ResetProvider()
        {
            if (_wagonDataProvider != null)
            {

                lock (_wagonDataProvider)
                {
                    _wagonDataProvider = null;
                }

            }
        }

        static ListWagonData _innerListWagonData;

        public static ListWagonData WagonList
        {
            get
            {
                if (_innerListWagonData == null)
                    _innerListWagonData = new ListWagonData(NamingUtilits.GetTrainID(VideoFileName));
                return _innerListWagonData;
            }
        }

        public static  void RefreshWagonList(string videoFileName)
        {
            int trainID = NamingUtilits.GetTrainID(VideoFileName);
            if (_innerListWagonData == null)
                _innerListWagonData = new ListWagonData(trainID);
            else
                _innerListWagonData.ReInit(trainID);
        }
        
        static List<WagonDataShort> _selectedWagons;
        
        public static List<WagonDataShort> SelectedWagons
        {
            get
            {
                if (_selectedWagons == null)
                    _selectedWagons = new List<WagonDataShort>();
                return _selectedWagons;
            }
            set { _selectedWagons = value; }
        }

        public static bool TestConnectionString()
        {
            SQLCommandExecuter executer = new SQLCommandExecuter();
            return executer.TestConnection(ConnectionSettings.BuildConnectionString()) && !string.IsNullOrEmpty(ConnectionSettings.DataBaseName);
        }

    }

}
