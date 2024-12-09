using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AlfaPribor.AviFile;

namespace AlfaPribor.AviFile.Demo
{
    public partial class formAviViewer : Form
    {
        private AviFile _File;

        private List<Avi.AVISTREAMINFO> streamsInfo;

        private int _FramePos;

        private bool _Compressed;

        private int _StartFrame;

        private int _FramesCount;

        private int _StreamIndex;

        public formAviViewer()
        {
            InitializeComponent();
            _File = null;
            streamsInfo = null;
            _Compressed = false;
            _StreamIndex = -1;
        }

        public AviFile File
        {
            get { return _File; }
            set
            {
                if (_File != null)
                {
                    CloseFile();
                }
                _File = value;
                OpenFile();
            }
        }

        public bool IsPlaying
        {
            get
            {
                return (_File != null) && timerPlay.Enabled;
            }
        }

        private void OpenFile()
        {
            textBoxFileName.Text = _File.FileName;
            var streams = _File.GetStreamsInfo();
            var videoStreams = from stream in streams
                               where stream.fccType == Avi.StreamtypeVIDEO
                               //orderby stream.szName ascending
                               select stream;
            streamsInfo = new List<Avi.AVISTREAMINFO>(videoStreams);
            if (streamsInfo.Count > 0)
            {
                foreach (var info in streamsInfo)
                {
                    comboBoxStreamSelector.Items.Add(new string(info.szName));
                }
                comboBoxStreamSelector.SelectedIndex = 0;
            }
        }

        private void CloseFile()
        {
            if (IsPlaying)
            {
                StopPlay();
            }
            _File.EndDecompress(comboBoxStreamSelector.SelectedIndex);
            _File = null;
            streamsInfo.Clear();
            streamsInfo = null;
            _Compressed = false;
            InitForm();
        }

        private void StopPlay()
        {
            timerPlay.Enabled = false;
            comboBoxStreamSelector.Enabled = true;
            buttonPlayPause.Enabled = true;
            buttonPlayPause.Text = "Играть";
            _FramePos = _StartFrame;
        }

        private void InitForm()
        {
            textBoxFileName.Text = string.Empty;
            comboBoxStreamSelector.Items.Clear();
            pictureBoxImage.Image = null;
            buttonPlayPause.Enabled = false;
            comboBoxStreamSelector.Enabled = false;
        }

        private void timerPlay_Tick(object sender, EventArgs e)
        {
            Avi.BITMAPINFOHEADER biHeader = _File.GetFrameInfo(StreamIndex, _FramePos);
            byte[] data;
            try
            {
                if (!_Compressed)
                {
                    int readed = 1;
                    data = _File.Read(StreamIndex, _FramePos, ref readed);
                }
                else
                {
                    data = _File.ReadDecompress(StreamIndex, _FramePos, out biHeader);
                }
                Bitmap image = AviFile.GetBitmap(biHeader, data);
                pictureBoxImage.Image = image;
            }
            catch(Exception E)
            {
                StopPlay();
                MessageBox.Show(
                    "Ошибка чтения видеоданных!\n" + E.Message, 
                    this.Text, 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Stop
                    );
                return;
            }
            if (_FramePos < _FramesCount-1)
            {
                ++_FramePos;
            }
            else
            {
                StopPlay();
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            CloseFile();
            base.Close();
        }

        private void buttonPlayPause_Click(object sender, EventArgs e)
        {
            if (IsPlaying)
            {
                timerPlay.Enabled = false;
                buttonPlayPause.Text = "Играть";
                comboBoxStreamSelector.Enabled = true;
            }
            else
            {
                timerPlay.Enabled = true;
                buttonPlayPause.Text = "Пауза";
                comboBoxStreamSelector.Enabled = false;
            }
        }

        private void comboBoxStreamSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            StreamIndex = comboBoxStreamSelector.SelectedIndex;
        }

        public int StreamIndex
        {
            get
            {
                return _StreamIndex;
            }

            set
            {
                StopPlay();
                if ((_StreamIndex >= 0) && _Compressed)
                {
                    _File.EndDecompress(_StreamIndex);
                }
                _StreamIndex = comboBoxStreamSelector.SelectedIndex;
                pictureBoxImage.Image = null;
                if (_StreamIndex < 0)
                {
                    return;
                }
                Avi.AVISTREAMINFO info = streamsInfo[_StreamIndex];
                timerPlay.Interval = (int)((double)info.dwScale / (double)info.dwRate * 1000.0);
                _StartFrame = (int)info.dwStart;
                _FramePos = _StartFrame;
                _FramesCount = (int)info.dwLength;
                uint fcc = streamsInfo[_StreamIndex].fccHandler;
                string fcc_str = Avi.GetFourCCstr(fcc);
                _Compressed = fcc != Avi.GetFourCC("DIB");
                if (_Compressed)
                {
                    _File.BeginDecompress(_StreamIndex, _File.GetFrameInfo(_StreamIndex, _FramePos));
                }
            }
        }
    }
}
