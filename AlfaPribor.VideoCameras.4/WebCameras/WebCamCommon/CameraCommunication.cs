using System;
using System.Text;
using System.Collections.Generic;
using System.Threading;

namespace WebCamCommon
{
	delegate void OnFrameHandler(byte[] frame, ulong ordinalNumber);
	delegate void OnStateHandler(CameraConnectionState prevState, CameraConnectionState state);

	enum eRawFrames { Disabled, Enabled, Paused }
    
    enum CameraConnectionState
	{
		Unknown,
		Connected,
		Disconnected
	}

	class CameraCommunication : IDisposable
	{

        bool _disposed = false;
		string _userName;
		string _userPassword;
		string _serverName;
		byte[] _rawFrameFinalizer = System.Text.ASCIIEncoding.ASCII.GetBytes("\r\n");
		protected int _streamId;

		protected MJPegWebCommandResult _cmdResult;
		protected WebService _webService;
		protected System.Diagnostics.Stopwatch _sw;
		private CameraConnectionState _connectionState = CameraConnectionState.Unknown;
		private eRawFrames _boRawFrames = eRawFrames.Disabled;
		protected string _resolution;
		protected int _fps;
		protected int _compression;

		public OnFrameHandler OnFrame;
		public OnFrameHandler OnRowFrame;
		public OnStateHandler OnState;
        
		public CameraCommunication()
		{
		}

		public CameraCommunication(string userName, string userPassword, string serverName, int streamId)
		{
			Initialize(userName, userPassword, serverName, streamId);
		}

		public bool Initialize(string userName, string userPassword, string serverName, int streamId)
		{
			_streamId = streamId;
			_serverName = serverName;
			_userName = userName;
			_userPassword = userPassword;
			_webService = new WebService(UserName, UserPassword);
			return true;
		}

		public void OverrideParams(string resolution, int fps, int compression)
		{
			_resolution = resolution;
			_fps = fps;
			_compression = compression;
		}

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
					FreeResources();
                }
            }

        }

		~CameraCommunication()
        {
            Dispose(false);
        }

        #endregion

		protected virtual void FreeResources()
		{
			StopVideo();
		}

		protected void SetState(CameraConnectionState state)
		{
			if (state == _connectionState)
			{
				return;
			}

			CameraConnectionState prevstate = _connectionState;
			_connectionState = state;
			if (OnState != null)
			{
				try
				{
					OnState(prevstate, _connectionState);
				}
				catch (Exception)
				{
				}
			}
		}

		public virtual void StartVideo()
		{
		}

		public virtual void StopVideo()
		{
			if (_cmdResult != null)
			{
				MJPegWebCommandResult cmdResult = _cmdResult;
				_cmdResult = null;
				cmdResult.StopProcessing();
				cmdResult = null;
			}
		}

		protected void OnCommandFinished(CommandResult cmd)
		{
			if (cmd != null)
			{
				MJPegWebCommandResult cmdRes = cmd as MJPegWebCommandResult;
				cmdRes.OnFrame -= new OnFrameHandler(OnMJpegFrame);
				cmdRes.OnOnCommandStarted -= new OnCommandStartedHandler(OnOnCommandStarted);
				cmdRes.OnCommandFinished -= new OnCommandFinishedHandler(OnCommandFinished);
			}

			SetState(CameraConnectionState.Disconnected);
			if (_cmdResult != null)
			{
                _cmdResult = null;
				// wait for 3 seconds and try again
				System.Threading.Thread.Sleep(3000);
				StartVideo();
			}
		}

		protected void OnOnCommandStarted(CommandResult cmd)
		{
			SetState(CameraConnectionState.Connected);
		}

		private byte[] BuildUpRawFrame(byte[] frame)
		{
			DateTime dt = DateTime.Now;
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("--myboundary\r\nTimeStamp: {0}\r\nStreamId: {2}\r\nContent-Type: image/jpeg\r\nContent-Length: {1}\r\n\r\n",
				(long)_sw.ElapsedMilliseconds, frame.Length, _streamId);
			string s = sb.ToString();
			byte[] sBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(s);
			byte[] bt = new byte[sBytes.Length + frame.Length + _rawFrameFinalizer.Length];
			sBytes.CopyTo(bt, 0);
			frame.CopyTo(bt, sBytes.Length);
			_rawFrameFinalizer.CopyTo(bt, frame.Length + sBytes.Length);
			return bt;
		}

        protected void OnMJpegFrame(byte[] frame, ulong imageOrdinalNumber)
		{
            if (OnFrame != null)
            {
                OnFrame(frame, imageOrdinalNumber);
            }
            if ((RawFrames == eRawFrames.Enabled) && (OnRowFrame != null))
            {
                byte[] rawFrame = BuildUpRawFrame(frame);
                OnRowFrame(rawFrame, imageOrdinalNumber);
            }
		}

		#region Properties

		public string UserName
		{
			get
			{
				return _userName;
			}
		}

		public string UserPassword
		{
			get
			{
				return _userPassword;
			}
		}

		public string ServerName
		{
			get
			{
				return _serverName;
			}
		}

		public bool IsConnected
		{
			get
			{
				return _connectionState == CameraConnectionState.Connected;
			}
		}

		public eRawFrames RawFrames
		{
			get
			{
				return _boRawFrames;
			}
			set
			{
				if (_boRawFrames == value)
				{
					return;
				}

				_boRawFrames = value;
				if (_boRawFrames == eRawFrames.Enabled)
				{
					if (_sw == null)
					{
						_sw = new System.Diagnostics.Stopwatch();
						_sw.Start();
					}
					else
					{
						_sw.Start();
					}
				}
				else if ( _boRawFrames == eRawFrames.Disabled )
				{
					if (_sw != null)
					{
						_sw = null;
					}
				}
				else if (_boRawFrames == eRawFrames.Paused)
				{
					if (_sw != null)
					{
						_sw.Stop();
					}
				}
			}
		}

		#endregion
	}
}