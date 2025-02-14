using System;
using System.Text;
using System.Net;
using WebCamCommon;

namespace PanasonicNamespace
{
	class PnsMJpegWebCommand : WebCommand
	{
		private string _param = "/cgi-bin/jpeg?connect=start&framerate={1}&resolution={2}&UID={0}";
		private string _uid = string.Empty;
		protected string _resolution = string.Empty;
		protected int _fps = 0;
		protected int _compression = -1;

		public PnsMJpegWebCommand(string uid, string resolution, int fps, int compression)
		{
			_uid = uid;
			_resolution = resolution;
			_fps = fps;
			_compression = compression;
		}

		public override CommandResult GetCommandResult(WebRequest response)
		{
			return new PnsMJPegWebCommandResult(response);
		}

		#region Implementation of Abstract base functions

		public override void BuildCommand()
		{
			if (string.IsNullOrEmpty(_resolution) == false)
			{
				_resolution = "1280";
			}
			this.Url = "http://" +
				ServerName +
				string.Format(_param, _uid, 30, _resolution);
		}

		#endregion
	}
}