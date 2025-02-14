using System;
using System.Text;
using WebCamCommon;

namespace PanasonicNamespace
{
	class PnsCommunication : CameraCommunication
	{

		bool _stopped = true;
		string _uid;
		System.Diagnostics.Stopwatch _swPing = new System.Diagnostics.Stopwatch();

		public PnsCommunication()
		{
		}

		public PnsCommunication(string userName, string userPassword, string serverName, int streamId)
			: base(userName, userPassword, serverName, streamId)
		{
		}

		void GetUIDAsycnHandler(CommandResult cmdResult)
		{
			if (cmdResult == null)
			{
				if (_stopped == false)
				{
					StartVideo();
				}
				return;
			}

			_uid = string.Empty;
			string result = cmdResult.Result;
			string[] items = result.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string item in items)
			{
				if (item.IndexOf("UID") != -1)
				{
					string[] ss = item.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
					_uid = ss[1];
					break;
				}
			}

			PnsMJpegWebCommand cmd = new PnsMJpegWebCommand(_uid, this._resolution, this._fps, this._compression);
			_swPing.Reset();
			_swPing.Start();
			cmd.ServerName = ServerName;
			_cmdResult = _webService.RunCommand(cmd) as MJPegWebCommandResult;
			_cmdResult.OnFrame += new OnFrameHandler(OnMJpegFrame);
			_cmdResult.OnOnCommandStarted += new OnCommandStartedHandler(OnOnCommandStarted);
			_cmdResult.OnCommandFinished += new OnCommandFinishedHandler(OnCommandFinished);
			_cmdResult.StartProcessing();
		}

		public override void StartVideo()
		{
			_stopped = false;
			PnsGetUIDWebCommand cmd = new PnsGetUIDWebCommand();
			cmd.ServerName = ServerName;
			_webService.RunCommandAsynch(cmd, new AsycnWebResult(GetUIDAsycnHandler));
		}

		protected new void OnMJpegFrame(byte[] frame, ulong imageOrdinalNumber)
		{
			base.OnMJpegFrame(frame, imageOrdinalNumber);

			if (_swPing.ElapsedMilliseconds > 30000)
			{
				//System.Diagnostics.Debug.WriteLine("Sending keep alive ping");
				PnsPingWebCommand cmd = new PnsPingWebCommand(_uid);
				cmd.ServerName = ServerName;
				_webService.RunCommandAsynch(cmd, new AsycnWebResult(PingAsycnHandler));
				_swPing.Reset();
				_swPing.Start();
			}
		}

		void PingAsycnHandler(CommandResult cmdResult)
		{
			if (cmdResult != null)
			{
				string result = cmdResult.Result;
				result = string.Empty;
			}
		}

		public override void StopVideo()
		{
			if (string.IsNullOrEmpty(_uid) == false)
			{
				_swPing.Reset();
				base.StopVideo();
				PnsMJPegStopWebCommand cmd = new PnsMJPegStopWebCommand(_uid);
				cmd.ServerName = ServerName;
				_webService.RunCommandAsynch(cmd, new AsycnWebResult(StopAsycnHandler));
			}
			_stopped = true;
		}

		void StopAsycnHandler(CommandResult cmdResult)
		{
			if (cmdResult != null)
			{
				string result = cmdResult.Result;
				if (_cmdResult != null)
				{
					MJPegWebCommandResult cmd = _cmdResult;
					_cmdResult = null;
					cmd.StopProcessing();
					cmd = null;
				}
			}
		}

	}
}