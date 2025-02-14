using System;
using System.IO;
using System.Net;
using System.Web;

namespace WebCamCommon
{
	delegate void AsycnWebResult(CommandResult cmdResult);

	class WebService
    {

		struct RequestGetter
		{
			public WebCommand cmd;
			public WebRequest request;
			public AsycnWebResult _asyncResult;

			public RequestGetter(WebCommand cmd, WebRequest request, AsycnWebResult asyncResult)
			{
				this.cmd = cmd;
				this.request = request;
				_asyncResult = asyncResult;
			}
		}

		ICredentials _credentials;

        public WebService(string user, string password)
        {
            _credentials = new NetworkCredential(user, password);
        }

		WebRequest PrepareToRun(ServiceCommand cmd)
		{
			System.Diagnostics.Debug.Assert(cmd != null);
			if (cmd == null)
			{
				return null;
			}

			WebCommand webCmd = cmd as WebCommand;
			System.Diagnostics.Debug.Assert(webCmd != null);
			if (webCmd == null)
			{
				return null;
			}
			webCmd.BuildCommand();

			//string url = System.Web.HttpUtility.UrlPathEncode(webCmd.Url);
            string url = System.Uri.EscapeUriString(webCmd.Url);//!!!!! проверить

            WebRequest request = null;
            try
            {
                request = WebRequest.Create(url);
                request.Timeout = 5000;
                request.Credentials = _credentials;
            }
            catch (Exception) { }
			return request;
		}

		public CommandResult RunCommand(ServiceCommand cmd)
        {
			WebRequest request = PrepareToRun(cmd);
			if (request != null)
			{
				WebCommand webCmd = cmd as WebCommand;
				return webCmd.GetCommandResult(request);
			}
			return null;
        }

		private void AsyncCallback(IAsyncResult ar)
		{
			RequestGetter getter = (RequestGetter)ar.AsyncState;
			try
			{
				WebResponse webResponse = getter.request.EndGetResponse(ar);
				if (getter._asyncResult != null)
				{
					getter._asyncResult(getter.cmd.GetCommandResult(webResponse as HttpWebResponse));
				}
			}
			catch (Exception ex)
			{
				if (getter._asyncResult != null)
				{
					getter._asyncResult(null);
				}
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
		}

		public void RunCommandAsynch(WebCommand cmd, AsycnWebResult asyncResult)
		{
			WebRequest request = PrepareToRun(cmd);
			if (request != null)
			{
				AsyncCallback callback = new AsyncCallback(AsyncCallback);
				request.BeginGetResponse(callback, new RequestGetter(cmd, request, asyncResult));
			}
		}

		public ICredentials Credentials
		{
			get
			{
				return _credentials;
			}
		}
    }
}