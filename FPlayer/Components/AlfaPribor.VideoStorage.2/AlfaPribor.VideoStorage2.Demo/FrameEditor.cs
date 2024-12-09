using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AlfaPribor.VideoStorage.Demo
{
    public partial class FrameEditor : Form
    {
        /// <summary>Поток с видеоданными</summary>
        private Stream _Stream;

        public FrameEditor()
        {
            InitializeComponent();
            _Stream = null;
        }

        private void btnLoadVideoData_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileStream file = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read);
                try
                {
                    _Stream = new BufferedStream(file, 4096);
                    try
                    {
                        pbxVideoData.Image = Image.FromStream(_Stream);
                    }
                    catch
                    {
                        MessageBox.Show("Ошибка загрузки видеоданных!");
                    }
                }
                catch
                {
                    _Stream.Dispose();
                    _Stream = null;
                }
            }
        }

        /// <summary>Видеокадр - базовая единица видеозаписи</summary>
        public VideoFrame Frame
        {
            get
            {
                byte[] data = (_Stream != null) ? new byte[_Stream.Length] : null;
                if (_Stream != null)
                {
                    _Stream.Position = 0;
                    _Stream.Read(data, 0, data.Length);
                }
                VideoFrame frame = new VideoFrame(
                    (int)numCameraId.Value,
                    (int)numTimeStamp.Value,
                    txtbContetType.Text,
                    data
                );
                return frame;
            }
            set
            {
                numCameraId.Value = value.CameraId;
                numTimeStamp.Value = value.TimeStamp;
                txtbContetType.Text = value.ContentType.ToString();
                if (_Stream != null)
                {
                    _Stream.Dispose();
                }
                if (value.FrameData == null)
                {
                    _Stream = null;
                }
                else
                {
                    try
                    {
                        _Stream = new MemoryStream(value.FrameData);
                        try
                        {
                            pbxVideoData.Image = (_Stream != null) ? Image.FromStream(_Stream) : null;
                        }
                        catch { }
                    }
                    catch
                    {
                        _Stream.Dispose();
                        _Stream = null;
                    }
                }
            }
        }

        private void btnClearVideoData_Click(object sender, EventArgs e)
        {
            pbxVideoData.Image = null;
            if (_Stream != null)
            {
                _Stream.Dispose();
            }
            _Stream = null;
        }
    }
}
