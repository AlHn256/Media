using System;
using System.Text;
using System.Net;
using System.IO;
using WebCamCommon;

namespace PanasonicNamespace
{
	class PnsMJPegWebCommandResult : MJPegWebCommandResult
	{
		public PnsMJPegWebCommandResult(WebRequest request) : base(request)
		{
		}
	}
}