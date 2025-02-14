using System;
using System.Text;
using System.Net;
using WebCamCommon;

namespace PanasonicNamespace
{
	class PnsMJPegStopWebCommand : WebCommand
	{
		private string _param = "/cgi-bin/jpeg?connect=stop&UID={0}";
		private string _uid = string.Empty;

		public PnsMJPegStopWebCommand(string uid)
		{
			_uid = uid;
		}

		#region Implementation of Abstract base functions
		public override void BuildCommand()
		{
			this.Url = "http://" + ServerName + string.Format(_param, _uid);
		}
		#endregion
	}
}