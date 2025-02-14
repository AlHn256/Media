using System;
using System.Collections.Generic;
using System.Text;

namespace WebCamCommon
{
    abstract class ServiceCommand
    {
		public ServiceCommand()
        {
        }

        #region Abstract functions

        public abstract void BuildCommand();

        #endregion
    }
}