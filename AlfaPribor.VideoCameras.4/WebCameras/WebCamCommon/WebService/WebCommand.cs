using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace WebCamCommon
{
    abstract class WebCommand : ServiceCommand
    {
        private string _serverName;
        private string _url;

        public WebCommand()
        {
        }

		void AsyncCallback(IAsyncResult ar)
		{
		}

		public virtual CommandResult GetCommandResult(WebService webService)
		{
			if (webService != null)
			{
				return webService.RunCommand(this);
			}
			return null;
		}

		public virtual CommandResult GetCommandResult(WebRequest response)
        {
			Stream dataStream = response.GetResponse().GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string result = reader.ReadToEnd();
			return new CommandResult(result);
        }

		public virtual CommandResult GetCommandResult(HttpWebResponse response)
		{
			Stream dataStream = response.GetResponseStream();
			StreamReader reader = new StreamReader(dataStream);
			string result = reader.ReadToEnd();
			return new CommandResult(result);
		}

        #region Properties
        public string ServerName
        {
            get
            {
                return _serverName;
            }

            set
            {
                _serverName = value;
            }
        }

        public string Url
        {
            get
            {
                return _url;
            }

            protected set
            {
                _url = value;
            }
        }
        #endregion
    }
}