using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using AlfaPribor.AviFile;

namespace AlfaPribor.AviFile.Demo
{
    public partial class formMain : Form
    {
        #region Fields

        private AviFile aviFile;

        private AviFile aviDestFile;

        private IList<int> SelectedStreams;

        private Avi.AVICOMPRESSOPTIONS comp_opts;

        #endregion

        #region Methods

        public formMain()
        {
            InitializeComponent();
            aviFile = new AviFile();
            aviDestFile = new AviFile();
        }

        private void buttonChoiceFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxFileName.Text = openFileDialog.FileName;
            }
        }

        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            aviFile.Close();
            aviFile.Open(textBoxFileName.Text, SourceFileOpenMode);
            buttonView.Enabled = true;
            buttonViewInfo.Enabled = true;
        }

        private void buttonView_Click(object sender, EventArgs e)
        {
            using (formAviViewer Viewer = new formAviViewer())
            {
                Viewer.File = aviFile;
                Viewer.ShowDialog();
            }
        }

        private void buttonViewInfo_Click(object sender, EventArgs e)
        {
            using (formInfoViewer InfoViewer = new formInfoViewer())
            {
                List<AviStreamInfo> streams_info = new List<AviStreamInfo>();
                foreach (var item in aviFile.GetStreamsInfo())
                {
                    streams_info.Add(new AviStreamInfo(item));
                }
                InfoViewer.Data = streams_info;
                InfoViewer.ShowDialog();
            }
        }

        private void buttonChoiceDestFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxDestFileName.Text = openFileDialog.FileName;
            }
        }

        private void buttonOpenDestFile_Click(object sender, EventArgs e)
        {
            aviDestFile.Close();
            // Пробное открытие файла
            aviDestFile.Open(textBoxDestFileName.Text, DestFileOpenMode);
            aviDestFile.Close();

            buttonSelectSources.Enabled = true;
            buttonCopy.Enabled = true;
            SelectedStreams = null;
        }

        private void buttonSelectSources_Click(object sender, EventArgs e)
        {
            using (formInfoViewer InfoViewer = new formInfoViewer())
            {
                InfoViewer.AllowCheck = true;
                List<AviStreamInfo> streams_info = new List<AviStreamInfo>();
                foreach (var item in aviFile.GetStreamsInfo())
                {
                    streams_info.Add(new AviStreamInfo(item));
                }
                InfoViewer.Data = streams_info;
                InfoViewer.ShowDialog();
                SelectedStreams = InfoViewer.CheckedStreamIndexes;
            }
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (SelectedStreams == null)
            {
                MessageBox.Show(
                    "Не выбраны потоки для копирования!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );
                return;
            }
            StreamCopierParams _params = new StreamCopierParams(
                textBoxFileName.Text, SourceFileOpenMode, textBoxDestFileName.Text, DestFileOpenMode,
                new List<int>(SelectedStreams)
                );
            _params.Decompress = checkBoxDecompressStreams.Checked;
            _params.Compress = checkBoxCompressStreams.Checked;
            _params.CompresorOptions = comp_opts;

            formStreamCopyProgress progress_form = new formStreamCopyProgress();
            BackgroundWorker stream_copier = CreateStreamCopierThread(progress_form);
            stream_copier.RunWorkerAsync(_params);
            progress_form.BindingThread = stream_copier;
            progress_form.Show();
        }

        private BackgroundWorker CreateStreamCopierThread(formStreamCopyProgress owner_form)
        {
            BackgroundWorker thread = new BackgroundWorker();
            thread.WorkerReportsProgress = true;
            thread.WorkerSupportsCancellation = true;
            thread.ProgressChanged += owner_form.ProgressChangedHandler;
            thread.RunWorkerCompleted += owner_form.ProgressComplitedHandler;
            thread.DoWork += new DoWorkEventHandler(thread_DoWork);
            return thread;
        }

        void thread_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker thread = (BackgroundWorker)sender;
            StreamCopierParams _params = (StreamCopierParams)e.Argument;
            int PersentsComplitedAll = 0;
            int PersentsComplitedStream = 0;
            int CopyCount = 0;
            List<int> strreamIndexes = _params.StreamNumbers;
            AviFile aviSourceFile = new AviFile(_params.SourceFileName, _params.SourceFileOpenMode);
            AviFile aviDestFile = new AviFile(_params.DestFileName, _params.DestFileOpenMode);
            try
            {
                IList<Avi.AVISTREAMINFO> streamsInfo = aviSourceFile.GetStreamsInfo();

                thread.ReportProgress(
                    PersentsComplitedAll,
                    new StreamCopierInfo()
                    {
                        CopiedPersentsAll = PersentsComplitedAll,
                        CopiedPersentsStream = PersentsComplitedStream,
                        CopyCount = CopyCount,
                        CurrentStream = streamsInfo[CopyCount],
                        DestFileName = _params.DestFileName,
                        SourceFileName = _params.SourceFileName,
                        StreamsCount = strreamIndexes.Count
                    }
                    );

                bool canceled = false;
                foreach (int index in strreamIndexes)
                {
                    if (thread.CancellationPending)
                    {
                        canceled = true;
                        break;
                    }
                    Avi.AVISTREAMINFO info = streamsInfo[index];
                    info.szName = null;
                    Avi.BITMAPINFOHEADER biHeader = aviSourceFile.GetFrameInfo(index, 0);
                    bool decompress_stream = _params.Decompress && (biHeader.biCompression != Avi.BI_RGB);
                    if (decompress_stream)
                    {
                        aviSourceFile.BeginDecompress(index, biHeader);
                    }
                    int dest_index;
                    if (_params.Compress)
	                {
                        info.fccHandler = _params.CompresorOptions.fccHandler;
                        if (decompress_stream)
                        {
                            aviSourceFile.ReadDecompress(index, (int)info.dwStart, out biHeader);
                        }
                        dest_index = aviDestFile.CreateCompressedStream(info, biHeader, _params.CompresorOptions);
	                }
                    else
                    {
                        if (decompress_stream)
                        {
                            info.fccHandler = Avi.GetFourCC("DIB");
                            aviSourceFile.ReadDecompress(index, (int)info.dwStart, out biHeader);
                        }
                        dest_index = aviDestFile.CreateStream(info, biHeader);
                    }
                    int samples = 1;
                    for (uint frame_number = info.dwStart, step_count = 0; frame_number < info.dwLength; frame_number++, step_count++)
                    {
                        if (thread.CancellationPending)
                        {
                            canceled = true;
                            break;
                        }
                        if (step_count == 9)
                        {
                            step_count = 0;

                            thread.ReportProgress(
                                PersentsComplitedAll,
                                new StreamCopierInfo()
                                {
                                    CopiedPersentsAll = PersentsComplitedAll,
                                    CopiedPersentsStream = PersentsComplitedStream,
                                    CopyCount = CopyCount,
                                    CurrentStream = streamsInfo[CopyCount],
                                    DestFileName = _params.DestFileName,
                                    SourceFileName = _params.SourceFileName,
                                    StreamsCount = strreamIndexes.Count
                                }
                                );
                        }
                        byte[] data;
                        if (decompress_stream)
	                    {
                            Avi.BITMAPINFOHEADER bmi;
		                    data = aviSourceFile.ReadDecompress(index, (int)frame_number, out bmi);
	                    }
                        else
                        {
                            data = aviSourceFile.Read(index, (int)frame_number, ref samples);
                        }
                        if (_params.Compress)
                        {
                            aviDestFile.WriteCompress(dest_index, (int)frame_number, ref samples, data, 0, data.Length);
                        }
                        else
                        {
                            aviDestFile.Write(dest_index, (int)frame_number, ref samples, data, 0, data.Length);
                        }
                        PersentsComplitedStream = (int)((double)frame_number / (double)info.dwLength * 100.0);
                        PersentsComplitedAll = (int)((double)(CopyCount * 100 + PersentsComplitedStream) / (double)(strreamIndexes.Count * 100) * 100.0);
                    }
                    if (_params.Decompress && (biHeader.biCompression != Avi.BI_RGB))
                    {
                        aviSourceFile.EndDecompress(index);
                    }
                    ++CopyCount;
                }
                if (!canceled)
                {
                    PersentsComplitedAll = 100;
                    PersentsComplitedStream = 100;
                    e.Result = new StreamCopierInfo()
                    {
                        CopiedPersentsAll = PersentsComplitedAll,
                        CopiedPersentsStream = PersentsComplitedStream,
                        CopyCount = CopyCount,
                        CurrentStream = new Avi.AVISTREAMINFO(),
                        DestFileName = _params.DestFileName,
                        SourceFileName = _params.SourceFileName,
                        StreamsCount = strreamIndexes.Count
                    };
                }
            }
            finally
            {
                aviSourceFile.Dispose();
                aviDestFile.Dispose();
            }
        }

        #endregion

        #region Properties

        private int SourceFileOpenMode
        {
            get
            {
                int open_mode;
                if (radioButtonReadOnly.Checked)
                {
                    open_mode = Avi.OF_READ;
                }
                else if (radioButtonWriteOnly.Checked)
                {
                    open_mode = Avi.OF_WRITE;
                }
                else
                {
                    open_mode = Avi.OF_READWRITE;
                }
                return open_mode;
            }
        }

        private int DestFileOpenMode
        {
            get
            {
                int open_mode;
                if (radioButtonCreate.Checked)
                {
                    open_mode = Avi.OF_CREATE | Avi.OF_WRITE;
                }
                else
                {
                    open_mode = Avi.OF_WRITE;
                }
                return open_mode;
            }
        }

        #endregion

        private void buttonCompressorChoice_Click(object sender, EventArgs e)
        {
            Avi.COMPVARS settings = new Avi.COMPVARS();
            settings.cbSize = Marshal.SizeOf(typeof(Avi.COMPVARS));
            char[] title = Encoding.Default.GetChars(Encoding.Default.GetBytes("Выбор компрессора данных"));
            bool result = Avi.ICCompressorChoose(
                this.Handle,
                Avi.ICMF_CHOOSE_ALLCOMPRESSORS | Avi.ICMF_CHOOSE_DATARATE | Avi.ICMF_CHOOSE_KEYFRAME,
                IntPtr.Zero,
                IntPtr.Zero,
                ref settings,
                title
                );

            comp_opts = new Avi.AVICOMPRESSOPTIONS();
            if (!result)
            {
                return;
            }
            comp_opts.fccHandler = settings.fccHandler;
            comp_opts.fccType = Avi.StreamtypeVIDEO;
            if (settings.lKey != 0)
            {
                comp_opts.dwKeyFrameEvery = (uint)settings.lKey;
                comp_opts.dwFlags |= Avi.AVICOMPRESSF_KEYFRAMES;
            }
            comp_opts.dwQuality = (uint)settings.lQ;
            if (settings.lDataRate != 0)
            {
                comp_opts.dwBytesPerSecond = (uint)settings.lDataRate * 1024U;
                comp_opts.dwFlags |= Avi.AVICOMPRESSF_DATARATE;
            }
            Avi.ICCompressorFree(ref settings);
        }

        private void checkBoxCompressStreams_CheckedChanged(object sender, EventArgs e)
        {
            buttonCompressorChoice.Enabled = checkBoxCompressStreams.Checked;
        }
    }

    class StreamCopierParams
    {
        public string SourceFileName = string.Empty;

        public int SourceFileOpenMode = 0;

        public string DestFileName = string.Empty;

        public int DestFileOpenMode = 0;

        public List<int> StreamNumbers = null;

        public bool Decompress = false;

        public bool Compress = false;

        public Avi.AVICOMPRESSOPTIONS CompresorOptions;

        public StreamCopierParams(
            string source_file_name, int source_file_open_mode,
            string dest_file_name, int dest_file_open_mode,
            IList<int> stream_numbers)
        {
            SourceFileName = source_file_name;
            SourceFileOpenMode = source_file_open_mode;
            DestFileName = dest_file_name;
            DestFileOpenMode = dest_file_open_mode;
            StreamNumbers = new List<int>(stream_numbers);
        }
    }
}
