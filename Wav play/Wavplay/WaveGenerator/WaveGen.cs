using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace Wavplay.WaveGenerator
{
    public partial class WaveGen : Form
    {
        string FilePath { get; set; }
        int oldHScrollBarValue = 440;
        public WaveGen(string filePath = @"C:\CSH\Wav play\test.wav")
        {
            FilePath = filePath;
            InitializeComponent();
        }

        private void ReGenerat()
        {
            uint DurationTextBoxVal = 1;
            uint.TryParse(DurationTextBox.Text, out DurationTextBoxVal);
            WaveGenerator waveGenerator = new WaveGenerator(HzScBar.Value, WaveExampleType.ExampleSineWave, VolumhScBar.Value, DurationTextBoxVal);
            waveGenerator.Save(FilePath);
            ShpowPic(waveGenerator);
            if (AutoPlayChkBox.Checked) PlayFile();
        }

        private void PlayFile()
        {
            if (File.Exists(FilePath))
            {
                SoundPlayer player = new SoundPlayer(FilePath);
                player.Play();
            }
        }

        private void ShpowPic(WaveGenerator waveGenerator)
        {
            var Data = waveGenerator.GetData();
            short[] dataArray = Data.shortArray;
            var Format = waveGenerator.GetFormat();
            var WChannels = Format.wChannels;

            picBoxRight.Image = new Bitmap(picBoxRight.Width, picBoxRight.Height);
            picBoxLeft.Image = new Bitmap(picBoxLeft.Width, picBoxLeft.Height);
            using (Graphics picLeft = Graphics.FromImage(picBoxLeft.Image))
            {
                using (Graphics picRight = Graphics.FromImage(picBoxRight.Image))
                {
                    picRight.Clear(Color.Black);
                    picLeft.Clear(Color.Black);
                    picRight.DrawLine(new Pen(Color.Red, 1), 0, picBoxRight.Height / 2, picBoxRight.Width, picBoxRight.Height / 2);
                    picLeft.DrawLine(new Pen(Color.Red, 1), 0, picBoxLeft.Height / 2, picBoxLeft.Width, picBoxLeft.Height / 2);
                    int xr = 0, xl = 0;
                    short maxShort = short.MaxValue;

                    for (uint x = 0; x < dataArray.Length - 1; x += 2)
                    {
                        if (xr + 1 > picBoxRight.Width || xl + 1 > picBoxLeft.Width) break;
                        //Fill with a simple sine wave at max amplitude
                        for (int channel = 0; channel < WChannels; channel++)
                        {
                            if (channel == 0)
                            {
                                var y = picBoxRight.Height * dataArray[x + channel] / maxShort / 2 + picBoxRight.Height / 2;
                                picRight.DrawLine(new Pen(Color.YellowGreen, 1), xr, y, xr + 1, y);
                                xr++;
                            }
                            else
                            {
                                var y = picBoxLeft.Height * dataArray[x + channel] / maxShort / 2 + picBoxLeft.Height / 2;
                                picLeft.DrawLine(new Pen(Color.YellowGreen, 1), xl, y, xl + 1, y);
                                xl++;
                            }
                        }
                    }
                }
            }
        }

        private void btnGenerateWave_Click(object sender, EventArgs e)
        {
            ReGenerat();
        }

        private void PlayBtn_Click_1(object sender, EventArgs e)
        {
            PlayFile();
        }

        private void HzScBar_MouseLeave(object sender, EventArgs e)
        {
            if (oldHScrollBarValue != HzScBar.Value)
            {
                ReGenerat();
                oldHScrollBarValue = HzScBar.Value;
            }
        }

        private void HzScBar_ValueChanged(object sender, EventArgs e)
        {
            HzTextBox.Text = HzScBar.Value.ToString();
        }

        private void VolumhScBar_ValueChanged(object sender, EventArgs e)
        {
            VolumLabel.Text = VolumhScBar.Value + " %";
        }

        private void HzTextBox_TextChanged(object sender, EventArgs e)
        {
            int HzTextBoxVal = 0;
            Int32.TryParse(HzTextBox.Text, out HzTextBoxVal);
            if (HzTextBoxVal < 10) HzTextBoxVal = 10;
            if (HzTextBoxVal > 17000) HzTextBoxVal = 17000;

            HzScBar.Value = HzTextBoxVal;
        }

        private void DurationTextBox_TextChanged(object sender, EventArgs e)
        {
            uint DurationTextBoxVal = 1;
            uint.TryParse(DurationTextBox.Text, out DurationTextBoxVal);
            if (DurationTextBoxVal < 1) DurationTextBoxVal = 1;
            if (DurationTextBoxVal > 1200) DurationTextBoxVal = 1200;
            DurationTextBox.Text = DurationTextBoxVal.ToString();
        }
    }
}
