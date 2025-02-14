using System;
using System.Text;
using System.Net;
using System.IO;
using WebCamCommon;

namespace AxisNamespace
{
	class AxisMJPegWebCommandResult : MJPegWebCommandResult
    {
		public AxisMJPegWebCommandResult(WebRequest request)
			: base(request)
        {
		}
    }
}