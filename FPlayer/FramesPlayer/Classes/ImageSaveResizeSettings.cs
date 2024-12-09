using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FramesPlayer
{
    public class ImageSaveResizeSettings
    {
        public bool EnableSaveImage { get; set; }
        public bool EnableResize { get; set; }

        public int ChannelID { get; set; }
        public int ResolutionX { get; set; }
        public int ResolutionY { get; set; }
        public int FrameExportStep { get; set; }
        public int VideoFrameCount { get; set; }

        public string ImageSaveFolder { get; set; }

        public string TrainFolderId { get; set; }



        public ImageSaveResizeSettings()
        {
            EnableSaveImage = false;
            EnableResize = false;
            ChannelID = 0;
            FrameExportStep = 1;
            VideoFrameCount = -1;
            ImageSaveFolder = @"D:\FramesPlayerImages";
        }
    }
}
