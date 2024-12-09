using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FramesPlayer
{
    public partial class FormAutoExportSettings : Form
    {
        public delegate void AutoExportSettingsChangedEventHandler(object sender, AutoExprortChangeSettingsEventArgs e);

        public AutoExportSettingsChangedEventHandler AutoExportSettingsChanged;

        protected void OnAutoExportSettingsChanged(object sender, AutoExprortChangeSettingsEventArgs e)
        {
            if (AutoExportSettingsChanged != null)
                AutoExportSettingsChanged(sender, e);
        }




        public FormAutoExportSettings()
        {
            InitializeComponent();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void bSelectVideoFramesPath_Click(object sender, EventArgs e)
        {
            try
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog { ShowNewFolderButton = true })
                {
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        tbVideoCadrsStorragePath.Text = fbd.SelectedPath;
                    }
                }

            }
            catch { }
        }

      

        private void bSaveData_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(tbVideoCadrsStorragePath.Text) && cbUseAutoViewVideoExport.Checked)
            {
                MessageBox.Show("Задайте каталог для экспорта видео");
                return;
            }

            ImageSaveResizeSettings currentSettigns = new ImageSaveResizeSettings();
            currentSettigns.EnableSaveImage = cbUseAutoViewVideoExport.Checked;
            currentSettigns.ImageSaveFolder = tbVideoCadrsStorragePath.Text;
            currentSettigns.FrameExportStep = (int)nudExportVideoStep.Value;
            currentSettigns.VideoFrameCount = (int)nudMaxVideoFrame.Value;
            currentSettigns.EnableResize = cbUseImageResize.Checked;
            currentSettigns.ResolutionX = (int)nudWidth.Value;
            currentSettigns.ResolutionY = (int)nudHeight.Value;
            currentSettigns.ChannelID = (int)nudChannelID.Value;
            OnAutoExportSettingsChanged(this, new AutoExprortChangeSettingsEventArgs( currentSettigns));

        }
    }
}
