using System;
using System.Text;
using WebCamCommon;

namespace PanasonicNamespace
{
	class PnsGetUIDWebCommand : WebCommand
	{
		private string _Param = "/cgi-bin/getuid?FILE=2&vcodec=jpeg";

		public PnsGetUIDWebCommand()
		{
		}

		#region Implementation of Abstract base functions
		public override void BuildCommand()
		{
			this.Url = "http://" + ServerName + _Param;
		}
		#endregion
	}
}