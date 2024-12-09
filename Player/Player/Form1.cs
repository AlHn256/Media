using Player.Models;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Player
{
    public partial class Form1 : Form
    {
        private object _context;
        private VideoPlayer videoPlayer;
        private string videoDirectory = @"D:\Work\C#\AP\AlfaPribor.VideoStorage.2\AlfaPribor.VideoStorage2.ASKO.Test\";
        private string id = "test";
        public Form1()
        {
            InitializeComponent();
            if (SynchronizationContext.Current != null) _context = SynchronizationContext.Current;
            else _context = new SynchronizationContext();
            videoPlayer = new VideoPlayer(_context, id, videoDirectory);

            videoPlayer.ChangKadr += UpdateImg;
            videoPlayer.ShowKadr();
        }

        private void UpdateImg(Bitmap img)
        {
            pictureBox1.Image = img;
        }

        private async void PlayBtn_Click(object sender, EventArgs e)
        {
            if (videoPlayer == null) return;

            //videoPlayer.ChangKadr += UpdateImg;
            await videoPlayer.PlayAsync();
        }

        private void PrevBtn_Click(object sender, EventArgs e)
        {
            //VideoFrame videoFrame = new VideoFrame();
            //askoVideoRecord.ReadPrevFrame(nCam, 1, out videoFrame);
            //pictureBox1.Image = ByteToImage(videoFrame.FrameData);
        }

        private void NextBtn_Click(object sender, EventArgs e)
        {
            NextKadr();
        }

        private void NextKadr()
        {
            //VideoFrame videoFrame = new VideoFrame();
            //askoVideoRecord.ReadNextFrame(nCam, 1, out videoFrame);
            //pictureBox1.Image = ByteToImage(videoFrame.FrameData);
        }
    }
}