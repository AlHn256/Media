using AlfaPribor.VideoStorage2;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Player.Models
{
    public class VideoPlayer
    {
        VideoStorageType videoStorageType = VideoStorageType.Original;
        private string VideoDirectory { get; set; }
        private string Id { get; set; }      

        //VideoStorageType videoStorageType = VideoStorageType.ASKO;
        //private string VideoDirectory = @"E:\Video.frames";
        //private string id = "4";
        private int nCam = 1, kadr = 0, maxKadr = 0;
        private SynchronizationContext context;
        private AskoVideoRecord askoVideoRecord;
        VideoFrame videoFrame = new VideoFrame();

        public void ShowKadr()
        {
            if (videoFrame != null && context != null)context.Send(OnChangKadr, ByteToImage(videoFrame.FrameData));
        }

        public VideoPlayer(object param,string id, string videoDirectory)
        {
            Id = id;    
            VideoDirectory = videoDirectory;
            context = (SynchronizationContext)param;
            askoVideoRecord = new AskoVideoRecord(Id, new VideoPartition(videoStorageType, 0, true, VideoDirectory), VideoRecordOpenMode.Read);
            askoVideoRecord.ReadFirstFrame(nCam, out videoFrame);
            var asd = askoVideoRecord.VideoIndex;
            maxKadr = askoVideoRecord.GetFramesCount(nCam);
        }

        public event Action<Bitmap> ChangKadr;
        public void OnChangKadr(object txt)
        {
            if (ChangKadr != null) ChangKadr((Bitmap)txt);
        }

        public async Task<bool> PlayAsync(bool playForeward = true)
        {
            if (maxKadr == 0 || askoVideoRecord == null) return false;
            if (kadr >= maxKadr) playForeward = false;

            while (true)
            {
                VideoFrame videoFrame = new VideoFrame();
                if (playForeward)
                {
                    if (++kadr >= maxKadr) break;
                    askoVideoRecord.ReadNextFrame(nCam, 1, out videoFrame);
                }
                else
                {
                    if (--kadr <= 1) break;
                    askoVideoRecord.ReadPrevFrame(nCam, 1, out videoFrame);
                }
                if (context != null) context.Send(OnChangKadr, ByteToImage(videoFrame.FrameData));
                await Task.Delay(40);
            }
            return true;
        }

        private static Bitmap ByteToImage(byte[] blob)
        {
            MemoryStream mStream = new MemoryStream();
            byte[] pData = blob;
            mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
            Bitmap bm = new Bitmap(mStream, false);
            mStream.Dispose();
            return bm;
        }
    }
}
