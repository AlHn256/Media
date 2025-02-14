using System;
using System.Collections.Generic;
using System.Text;

namespace WebCamCommon
{
    class CommandResult
    {

        string _result;

        public CommandResult()
        {
        }

        public CommandResult(string result)
        {
            _result = result;
        }

		public string Result
		{
			get
			{
				return _result;
			}
		}

    }
}