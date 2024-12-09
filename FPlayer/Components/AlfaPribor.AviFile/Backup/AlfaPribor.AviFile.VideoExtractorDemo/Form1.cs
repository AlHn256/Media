using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AlfaPribor.AviFile;

namespace AlfaPribor.AviFile.VideoExtractorDemo
{
    public partial class Form1 : Form
    {
        private AlfaPribor.AviFile.AviFile _SourceFile = null;

        private AlfaPribor.AviFile.AviFile _DestFile = null;

        private CopyRule exportRule;

        private Avi.AVICOMPRESSOPTIONS compressOptions = new Avi.AVICOMPRESSOPTIONS();

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonChooseSource_Click(object sender, EventArgs e)
        {
            openFileDialog.CheckFileExists = true;
            openFileDialog.ShowReadOnly = true;
            openFileDialog.Title = "Открыть файл-источник";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxSourceFileName.Text = openFileDialog.FileName;
            }
        }

        private void buttonChooseDest_Click(object sender, EventArgs e)
        {
            openFileDialog.CheckFileExists = false;
            openFileDialog.ShowReadOnly = false;
            openFileDialog.Title = "Открыть файл-приемник";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxDestFileName.Text = openFileDialog.FileName;
            }
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            ChangeSettings();
        }

        private void ChangeSettings()
        {
            CloseFile(ref _SourceFile);
            try
            {
                OpenFile(ref _SourceFile, textBoxSourceFileName.Text, Avi.OF_READ);
            }
            catch
            {
                MessageBox.Show(
                    "Не могу открыть файл-источник!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error
                );
                return;
            }
            using (AviExportProperties propertiesForm = new AviExportProperties())
            {
                propertiesForm.File = _SourceFile;
                propertiesForm.ShowDialog();
                compressOptions = propertiesForm.CompressOptions;
                UpdateExportSettings(propertiesForm.StreamIndex, propertiesForm.StartSample, propertiesForm.EndSample, propertiesForm.UseCompression);
            }
        }

        private void UpdateExportSettings(int source_index, int start_sample, int end_sample, bool use_compr)
        {
            exportRule.StartSample = start_sample;
            exportRule.SourceStreamIndex = source_index;
            exportRule.SamplesCount = end_sample - start_sample + 1;
            if (use_compr)
            {
                exportRule.WriteMode = CopyMode.Encode;
            }
            else
            {
                exportRule.WriteMode = CopyMode.Original;
            }
        }

        private void UpdateExportSettings()
        {
            exportRule.SourceFile = _SourceFile;
            if (
                exportRule.WriteMode == CopyMode.Encode &&
                _SourceFile.GetStreamInfo(exportRule.SourceStreamIndex).fccHandler != Avi.GetFourCC("DIB"))
            {
                exportRule.ReadMode = CopyMode.Decode;
            }
            else
            {
                exportRule.ReadMode = CopyMode.Original;
            }
            exportRule.DestFile = _DestFile;
        }

        private void OpenFile(ref AviFile file, string file_name, int open_mode)
        {
            file = new AviFile(file_name, open_mode);
        }

        private void CloseFile(ref AviFile file)
        {
            if (file != null)
            {
                file.Dispose();
                file = null;
            }
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            CloseFile(ref _SourceFile);
            CloseFile(ref _DestFile);
            //
            // ---- Первый способ, более ранний
            //
            //OpenFile(ref _SourceFile, textBoxSourceFileName.Text, Avi.OF_READ);
            //OpenFile(ref _DestFile, textBoxDestFileName.Text, Avi.OF_CREATE);
            //Avi.AVISTREAMINFO streamInfo = _SourceFile.GetStreamInfo(exportRule.SourceStreamIndex);
            //streamInfo.dwStart = 0;
            //streamInfo.dwLength = (uint)exportRule.SamplesCount;
            //Avi.BITMAPINFOHEADER bmiHeader = _SourceFile.GetFrameInfo(exportRule.SourceStreamIndex, (int)streamInfo.dwStart);
            //if (exportRule.WriteMode != CopyMode.Encode)
            //{
            //    exportRule.DestStreamIndex = _DestFile.CreateStream(streamInfo, bmiHeader);
            //}
            //else
            //{
            //    streamInfo.fccHandler = compressOptions.fccHandler;
            //    bmiHeader.biCompression = Avi.BI_RGB;
            //    exportRule.DestStreamIndex = _DestFile.CreateCompressedStream(streamInfo, bmiHeader, compressOptions);
            //}
            
            //UpdateExportSettings();

            //buttonExport.Enabled = false;
            //toolStripProgressBarExtract.Value = 0;
            //toolStripProgressBarExtract.Maximum = exportRule.SamplesCount;
            //toolStripProgressBarExtract.Visible = true;
            //using (AviFileCopier copier = new AviFileCopier())
            //{
            //    if (checkBoxDefaultBufferSize.Checked)
            //    {
            //        copier.SampleBufferSize = (int)streamInfo.dwSuggestedBufferSize;
            //    }
            //    else
            //    {
            //        copier.SampleBufferSize = 1024 * 1024 * (int)numericUpDownBufferSize.Value;
            //    }
            //    copier.CopySample += new EventHandler<CopySampleEventArgs>(copier_CopySample);
            //    copier.CopyException += new EventHandler<CopyExceptionEventArgs>(copier_CopyException);
            //    copier.Copy(new List<CopyRule> { exportRule });
            //}
            //
            // -----

            // ----- Второй способ, более поздний

            toolStripProgressBarExtract.Value = 0;
            toolStripProgressBarExtract.Maximum = exportRule.SamplesCount;
            toolStripProgressBarExtract.Visible = true;

            AviVideoExporter exporter = new AviVideoExporter();
            exporter.SourceFileName = textBoxSourceFileName.Text;
            exporter.DestFileName = textBoxDestFileName.Text;
            exporter.FirstFrame = exportRule.StartSample;
            exporter.FramesCount = exportRule.SamplesCount;
            exporter.UseCompressor = exportRule.WriteMode == CopyMode.Encode;
            exporter.CompressOptions = compressOptions;
            exporter.ExportFrame += new EventHandler<ExportFrameEventArgs>(exporter_ExportFrame);
            exporter.Export();

            buttonExport.Enabled = true;
            toolStripStatusLabelHint.Text = string.Empty;
            toolStripProgressBarExtract.Visible = false;
        }

        void exporter_ExportFrame(object sender, ExportFrameEventArgs e)
        {
            int count = toolStripProgressBarExtract.Value;
            ++count;
            toolStripStatusLabelHint.Text = string.Format("Скопировано {0,5} из {1,5} сэмплов", count, toolStripProgressBarExtract.Maximum);
            toolStripProgressBarExtract.Value = count;
            statusStrip.Refresh();
        }

        void copier_CopyException(object sender, CopyExceptionEventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Ошибка экспорта!\n" + e.InnerException.Message,
                this.Text,
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Error
            );
            e.Cancel = result == DialogResult.Cancel;
        }

        void copier_CopySample(object sender, CopySampleEventArgs e)
        {
            int count = toolStripProgressBarExtract.Value;
            ++count;
            toolStripStatusLabelHint.Text = string.Format("Скопировано {0,5} из {1,5} сэмплов", count, e.Rule.SamplesCount);
            toolStripProgressBarExtract.Value = count;
            statusStrip.Refresh();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownBufferSize.Enabled = !checkBoxDefaultBufferSize.Checked;
        }
    }
}
