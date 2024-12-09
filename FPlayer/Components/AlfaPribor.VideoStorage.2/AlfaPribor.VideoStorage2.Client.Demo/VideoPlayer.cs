using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.ServiceModel;
using AlfaPribor.VideoStorage;
using AlfaPribor.VideoStorage.Server;

namespace AlfaPribor.VideoStorage.Client.Demo
{
    public partial class VideoPlayer : Form
    {
        private IVideoReaderService _VideoReader;

        private IVideoIndexService _VideoIndex;

        private int _CurrentCamId;

        private int _Step;

        public VideoPlayer()
        {
            InitializeComponent();
            _VideoReader = null;
            _VideoIndex = null;
            _CurrentCamId = -1;
            _Step = 0;
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        public IVideoReaderService VideoReader
        {
            get
            {
                return _VideoReader;
            }
            set
            {
                if (_VideoReader != null)
                {
                    try
                    {
                        _VideoReader.Close();
                    }
                    catch (Exception E)
                    {
                        HandleException(E);
                    }
                }
                _VideoReader = value;
            }
        }

        public IVideoIndexService VideoIndex
        {
            get
            {
                return _VideoIndex;
            }
            set
            {
                _VideoIndex = value;
                miVideocamera.DropDownItems.Clear();
                try
                {
                    if (_VideoIndex != null)
                    {
                        IList<VideoStreamInfo> streamInfoList = _VideoIndex.GetStreamInfoList();
                        bool first = true;
                        foreach (VideoStreamInfo info in streamInfoList)
                        {
                            ToolStripMenuItem item = new ToolStripMenuItem(info.Id.ToString());
                            item.Click += new EventHandler(StreamMenuItem_Click);
                            if (first)
                            {
                                item.Checked = true;
                                _CurrentCamId = info.Id;
                                first = false;
                            }
                            miVideocamera.DropDownItems.Add(item);
                        }
                    }
                }
                catch (Exception E)
                {
                    HandleException(E);
                    return;
                }
            }
        }

        private void HandleException(Exception E)
        {
            if (E is FaultException)
            {
                MessageBox.Show(
                    "Ошибка выполнения операции на сервере хранилища видеоданных!\n" +
                    E.Message,
                    this.Text,
                    MessageBoxButtons.OK
                );
            }
            else if (E is CommunicationException)
            {
                MessageBox.Show(
                    "Ошибка связи с сервером хранилища видеоданных!\n" +
                    E.Message,
                    this.Text,
                    MessageBoxButtons.OK
                );
            }
        }

        void StreamMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null)
            {
                return;
            }
            if (item.OwnerItem != null)
            {
                ToolStripMenuItem owner = item.OwnerItem as ToolStripMenuItem;
                foreach (ToolStripItem menu_item in owner.DropDownItems)
                {
                    (menu_item as ToolStripMenuItem).Checked = false;
                }
            }
            item.Checked = true;
            _CurrentCamId = Int32.Parse(item.Text);
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            if (_VideoReader == null)
            {
                return;
            }
            VideoFrameInfo frame;
            VideoStorageResult result;
            try
            {
                result = _VideoReader.ReadFirstFrame(_CurrentCamId, out frame);
            }
            catch (Exception E)
            {
                HandleException(E);
                return;
            }
            if (result != VideoStorageResult.Ok)
            {
                ShowError(result);
                return;
            }
            try
            {
                Stream stream = _VideoReader.ReadFrameData();
                MemoryStream data = new MemoryStream();
                ReadData(stream, data);
                DrawFrame(data);
            }
            catch (Exception E)
            {
                HandleException(E);
                return;
            }
        }

        private void ReadData(Stream stream, MemoryStream data)
        {
            byte[] buffer = new byte[4096];
            int count;
            while ((count = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                data.Write(buffer, 0, count);
            }
        }

        private void DrawFrame(VideoFrame frame)
        {
            if (frame == null)
            {
                return;
            }
            MemoryStream stream = new MemoryStream(frame.FrameData);
            pbxImage.Image = new Bitmap(stream);
        }

        private void DrawFrame(Stream data)
        {
            if (data == null)
            {
                return;
            }
            pbxImage.Image = new Bitmap(data);
        }

        private void ShowError(VideoStorageResult result)
        {
            MessageBox.Show("Не могу прочитать видеокадр!\nКод ошибки:" + result);
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            if (_VideoReader == null)
            {
                return;
            }
            VideoFrameInfo frame_info;
            VideoStorageResult result;
            try
            {
                result = _VideoReader.ReadLastFrame(_CurrentCamId, out frame_info);
            }
            catch (Exception E)
            {
                HandleException(E);
                return;
            }
            if (result != VideoStorageResult.Ok)
            {
                ShowError(result);
                return;
            }
            try
            {
                Stream stream = _VideoReader.ReadFrameData();
                MemoryStream data = new MemoryStream();
                ReadData(stream, data);
                DrawFrame(data);
            }
            catch (Exception E)
            {
                HandleException(E);
                return;
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (_VideoReader == null)
            {
                return;
            }
            VideoFrameInfo frame_info;
            VideoStorageResult result;
            try
            {
                result = _VideoReader.ReadPrevFrame(_CurrentCamId, 1, out frame_info);
            }
            catch (Exception E)
            {
                HandleException(E);
                return;
            }
            if (result != VideoStorageResult.Ok)
            {
                ShowError(result);
                return;
            }
            try
            {
                Stream stream = _VideoReader.ReadFrameData();
                MemoryStream data = new MemoryStream();
                ReadData(stream, data);
                DrawFrame(data);
            }
            catch (Exception E)
            {
                HandleException(E);
                return;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_VideoReader == null)
            {
                return;
            }
            VideoFrameInfo frame;
            VideoStorageResult result;
            try
            {
                result = _VideoReader.ReadNextFrame(_CurrentCamId, 1, out frame);
            }
            catch (Exception E)
            {
                HandleException(E);
                return;
            }
            if (result != VideoStorageResult.Ok)
            {
                ShowError(result);
                return;
            }
            try
            {
                Stream stream = _VideoReader.ReadFrameData();
                MemoryStream data = new MemoryStream();
                ReadData(stream, data);
                DrawFrame(data);
            }
            catch (Exception E)
            {
                HandleException(E);
                return;
            }
        }

        private void btnPlayForward_Click(object sender, EventArgs e)
        {
            _Step = 1;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (_VideoReader == null)
            {
                return;
            }
            timer.Stop();
            try
            {
                VideoFrameInfo frame = null;
                VideoStorageResult result = VideoStorageResult.Fault;
                if (_Step > 0)
                {
                    result = _VideoReader.ReadNextFrame(_CurrentCamId, 1, out frame);
                }
                else if (_Step < 0)
                {
                    result = _VideoReader.ReadPrevFrame(_CurrentCamId, 1, out frame);
                }
                if (result != VideoStorageResult.Ok)
                {
                    timer.Stop();
                }
                else
                {
                    Stream stream = _VideoReader.ReadFrameData();
                    MemoryStream data = new MemoryStream();
                    ReadData(stream, data);
                    DrawFrame(data);
                }
            }
            catch (Exception E)
            {
                HandleException(E);
                return;
            }
            timer.Start();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            _Step = 0;
            timer.Start();
        }

        private void btnPlayBackward_Click(object sender, EventArgs e)
        {
            _Step = -1;
            timer.Enabled = true;
        }

        private void VideoPlayer_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (_VideoReader != null)
                {
                    _VideoReader.Close();
                    _VideoReader = null;
                }
                if (_VideoIndex != null)
                {
                    _VideoIndex.Close();
                    _VideoIndex = null;
                }
            }
            catch { }
        }
    }
}
