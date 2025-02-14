using System;
using System.Text;
using WebCamCommon;
using System.Threading;

namespace AxisNamespace
{

	class AxisCommunication : CameraCommunication
    {

		public AxisCommunication() { }

		public AxisCommunication(string userName, string userPassword, string serverName, int streamId)
			                     : base(userName, userPassword, serverName, streamId) { }

        public override void StartVideo()
        {
            AxisMJpegWebCommand cmd = new AxisMJpegWebCommand(this._resolution, this._fps, this._compression);
            cmd.ServerName = ServerName;
			_cmdResult = _webService.RunCommand(cmd) as MJPegWebCommandResult;
			_cmdResult.OnFrame += new OnFrameHandler(OnMJpegFrame);
			_cmdResult.OnOnCommandStarted += new OnCommandStartedHandler(OnOnCommandStarted);
			_cmdResult.OnCommandFinished += new OnCommandFinishedHandler(OnCommandFinished);
            _cmdResult.StartProcessing();
        }
    }
       
}
