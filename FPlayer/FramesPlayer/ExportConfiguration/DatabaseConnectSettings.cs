using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FramesPlayer.DataTypes;

namespace FramesPlayer.ExportConfiguration
{

    public class DatabaseConnectSettingsBase
    {

        public string ServerName { get; set; }

        public bool AutorizationType { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string DataBaseName { get; set; }

        public DatabaseConnectSettingsBase() { }

        public string BuildConnectionString()
        {
           string result= string.Empty;
           if (!AutorizationType)
               result = string.Format("Data Source={0};Initial Catalog={1};Integrated Security=SSPI;", 
                                      ServerName, DataBaseName);
           else
               result = string.Format(@"Data Source={0}; Initial Catalog={1};User ID={2};Password={3};",
                                      ServerName, DataBaseName, Login, Password);
           return result;
        }

    }

    public class DatabaseConnectionSettings : DatabaseConnectSettingsBase
    {
        public DatabaseConnectionSettings() { }

        public void Load()
        {
            try
            {
                this.ServerName = FramePlayerSettings.Default.ServerName;
                this.AutorizationType = FramePlayerSettings.Default.AutorizationType;
                this.Login = FramePlayerSettings.Default.Login;
                this.Password = FramePlayerSettings.Default.Passsword;
                this.DataBaseName = FramePlayerSettings.Default.DataBaseName;
            }
            catch { }
        }

        public void Save()
        {
            try
            {
                FramePlayerSettings.Default.ServerName = ServerName;
                FramePlayerSettings.Default.AutorizationType = AutorizationType;
                FramePlayerSettings.Default.Login = Login;
                FramePlayerSettings.Default.Passsword = Password;
                FramePlayerSettings.Default.DataBaseName = DataBaseName;
                FramePlayerSettings.Default.Save();
            }
            catch { }
        }
    }

}
