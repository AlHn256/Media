using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace AlfaPribor.AviFile.VideoExtractorDemo
{
    public partial class AviExportProperties : Form
    {
        private AviFile _File = null;

        private Avi.AVISTREAMINFO streamInfo;

        private int _StreamIndex = 0;

        private Avi.AVICOMPRESSOPTIONS comp_opts = new Avi.AVICOMPRESSOPTIONS();

        public AviExportProperties()
        {
            InitializeComponent();
            UpdateProperties();
        }

        public AviFile File
        {
            set
            {
                _File = value;
                UpdateProperties();
            }
            get { return _File; }
        }

        public string FileName
        {
            get 
            {
                return File != null ? File.FileName : string.Empty;
            }
            private set
            {
                textBoxFileName.Text = value;
            }
        }

        public int StreamIndex
        {
            get
            {
                return File != null ? _StreamIndex : Int32.MinValue;
            }
            private set { _StreamIndex = value; }
        }

        public int StartSample
        {
            get
            {
                return File != null ? (int)numericUpDownStartSample.Value : Int32.MinValue;
            }
            set 
            {
                if (_File != null)
                {
                    numericUpDownStartSample.Value = value;
                }
                else
                {
                    numericUpDownStartSample.Value = 0;
                }
            }
        }

        public int SamplesCount
        {
            get
            {
                return _File != null ? (int)streamInfo.dwLength : 0;
            }
            private set
            {
                if (_File != null)
                {
                    textBoxSamplesCount.Text = value.ToString();
                }
                else
                {
                    textBoxSamplesCount.Text = 0.ToString();
                }
            }
        }

        public int EndSample
        {
            get
            {
                return File != null ? (int)numericUpDownEndSample.Value : Int32.MinValue;
            }
            set
            {
                if (_File != null)
                {
                    numericUpDownEndSample.Value = value;
                }
                else
                {
                    numericUpDownEndSample.Value = 0;
                }
            }
        }

        public bool UseCompression
        {
            get { return checkBoxCompress.Checked; }
            set { checkBoxCompress.Checked = value; }
        }

        public Avi.AVICOMPRESSOPTIONS CompressOptions
        {
            get { return comp_opts; }
        }

        private void UpdateProperties()
        {
            if (_File == null || _File.IsOpen == false)
            {
                ClearProperties();
                return;
            }
            FileName = _File.FileName;
            _StreamIndex = FindStreamIndex();
            if (_StreamIndex >= 0)
            {
                streamInfo = _File.GetStreamInfo(_StreamIndex);
                StartSample = (int)streamInfo.dwStart;
                EndSample = (int)streamInfo.dwLength - 1;
                SamplesCount = (int)streamInfo.dwLength;
            }
            
        }

        private void ClearProperties()
        {
            _StreamIndex = Int32.MinValue;
            StartSample = Int32.MinValue;
            EndSample = Int32.MinValue;
        }

        private int FindStreamIndex()
        {
            if (_File == null)
            {
                return Int32.MinValue;
            }
            try
            {
                IList<Avi.AVISTREAMINFO> infoList = _File.GetStreamsInfo();
                Avi.AVISTREAMINFO streamInfo = infoList.First(info => info.fccType == Avi.StreamtypeVIDEO);
                return infoList.IndexOf(streamInfo);
            }
            catch { }
            return Int32.MinValue;
        }

        private void buttonChooseCodec_Click(object sender, EventArgs e)
        {
            Avi.COMPVARS codec_settings = new Avi.COMPVARS();
            codec_settings.cbSize = Marshal.SizeOf(codec_settings);
            codec_settings.Load(comp_opts);

            bool result = Avi.ICCompressorChoose(
                this.Handle,
                Avi.ICMF_CHOOSE_ALLCOMPRESSORS | Avi.ICMF_CHOOSE_DATARATE,
                IntPtr.Zero,
                IntPtr.Zero,
                ref codec_settings,
                "Выбор кодека".ToCharArray()
            );
            
            if (!result)
            {
                return;
            }
            Avi.ICCompressorFree(ref codec_settings);
            comp_opts.Load(codec_settings);
        }
    }
}
