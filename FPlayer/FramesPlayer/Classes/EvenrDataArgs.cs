using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FramesPlayer
{
    public class AutoExprortChangeSettingsEventArgs : EventArgs
    {
        public ImageSaveResizeSettings AutoExportSettings { get; set; }
        public AutoExprortChangeSettingsEventArgs()
        {

        }
        public AutoExprortChangeSettingsEventArgs(ImageSaveResizeSettings arg)
        {
            AutoExportSettings = arg;
        }


    }
}
