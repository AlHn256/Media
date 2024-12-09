using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AlfaPribor.AviFile.Demo
{
    public partial class formStreamCopyProgress : Form
    {
        private BackgroundWorker thread;
        public formStreamCopyProgress()
        {
            InitializeComponent();
        }

        public void ProgressChangedHandler(Object sender, ProgressChangedEventArgs args)
        {
            StreamCopierInfo progress_info = (StreamCopierInfo)args.UserState;
            UpdateInfo(progress_info);
        }

        private void UpdateInfo(StreamCopierInfo progress_info)
        {
            labelSourceFileName.Text = progress_info.SourceFileName;
            labelDestFileName.Text = progress_info.DestFileName;
            string stream_name = new string(progress_info.CurrentStream.szName);
            int index = stream_name.IndexOf('\0');
            labelStreamName.Text = index >=0 ? stream_name.Remove(index) : string.Empty;
            labelCopyCount.Text = progress_info.CopyCount.ToString();
            labelStreamsCount.Text = progress_info.StreamsCount.ToString();
            labelCopiedPersentsAll.Text = progress_info.CopiedPersentsAll.ToString();
            progressBarCopiedAll.Value = progress_info.CopiedPersentsAll;
            labelCopiedPersentsStream.Text = progress_info.CopiedPersentsStream.ToString();
            progressBarCopiedSream.Value = progress_info.CopiedPersentsStream;
        }

        public void ProgressComplitedHandler(Object sender, RunWorkerCompletedEventArgs args)
        {
            buttonCancel.Text = "Закрыть";
            buttonCancel.Click += new EventHandler(buttonClose_Click);
            if (args.Cancelled)
            {
                buttonCancel.DialogResult = DialogResult.Cancel;
            }
            else
            {
                buttonCancel.DialogResult = DialogResult.OK;
                if (args.Result != null)
                {
                    StreamCopierInfo result = (StreamCopierInfo)args.Result;
                    UpdateInfo(result);
                } 
            }
            buttonCancel.Enabled = true;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (thread != null)
            {
                thread.CancelAsync();
                buttonCancel.Enabled = false;
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        public BackgroundWorker BindingThread
        {
            get { return thread; }
            set { thread = value; }
        }
    }

    class StreamCopierInfo
    {
        public string SourceFileName = string.Empty;

        public string DestFileName = string.Empty;

        public Avi.AVISTREAMINFO CurrentStream;

        public int CopyCount = 0;

        public int StreamsCount = 0;

        public int CopiedPersentsAll = 0;

        public int CopiedPersentsStream = 0;
    }
}
