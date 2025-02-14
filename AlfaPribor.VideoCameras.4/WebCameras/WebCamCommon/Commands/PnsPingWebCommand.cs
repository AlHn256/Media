using System;
using System.Text;
using System.Net;
using WebCamCommon;

namespace PanasonicNamespace
{
	class PnsPingWebCommand : WebCommand
	{
		private string _param = "/cgi-bin/keep_alive?mode=jpeg&protocol=http&UID={0}";
		private string _uid = string.Empty;

		public PnsPingWebCommand(string uid)
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