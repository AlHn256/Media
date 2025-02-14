using System;
using System.Text;
using System.Net;
using WebCamCommon;

namespace AxisNamespace
{
    class AxisMJpegWebCommand : WebCommand
    {

        private string _param = "/axis-cgi/mjpg/video.cgi";
		protected string _resolution = string.Empty;
		protected int _fps = 0;
		protected int _compression = -1;

		public AxisMJpegWebCommand()
        {
        }

		public AxisMJpegWebCommand(string resolution, int fps, int compression)
		{
			_resolution = resolution;
			_fps = fps;
			_compression = compression;
		}

		void AsyncCallback(IAsyncResult ar)
		{
		}

		public override CommandResult GetCommandResult(WebRequest request)
        {
			return new AxisMJPegWebCommandResult(request);
        }

        #region Implementation of Abstract base functions

        public override void BuildCommand()
        {
			string args = "?";
			if (string.IsNullOrEmpty(_resolution) == false)
			{
				args += "resolution=" + _resolution;
			}
			if (_fps > 0)
			{
				if (args.Length != 1) args += "&";
                args += "fps=" +_fps.ToString();
			}
			if (_compression > 0)
			{
				if (args.Length != 1) args += "&";
    			args += "compression=" + _compression.ToString();
			}

			if (string.IsNullOrEmpty(args) == false) this.Url = "http://" + ServerName + _param + args;
			else this.Url = "http://" + ServerName + _param;
        }

        #endregion
    }
}