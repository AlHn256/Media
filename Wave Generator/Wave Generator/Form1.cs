using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace Wave_Generator
{
    public partial class Form1 : Form
    {
        const int SAMPLE_RATE = 44100;
        const short BITS_PER_SAMPLE = 16;
        string filePath = @"E:\C#\test.wav";
        int oldHScrollBarValue = 440;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReGenerat();
        }

        private void ReGenerat()
        {
            WaveGenerator waveGenerator = new WaveGenerator(HzScBar.Value, WaveExampleType.ExampleSineWave, VolumhScBar.Value);
            waveGenerator.Save(filePath);
            ShpowPic(waveGenerator);
            if (AutoPlayChkBox.Checked)PlayFile();
        }

        private void PlayFile()
        {if (File.Exists(filePath))
            {
                SoundPlayer player = new SoundPlayer(filePath);
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
                    int xr = 0,xl=0;
                    short maxShort = short.MaxValue;

                    for (uint x = 0; x < dataArray.Length - 1; x += 2)
                    {
                        if (xr + 1 > picBoxRight.Width || xl + 1 > picBoxLeft.Width) break;
                        //Fill with a simple sine wave at max amplitude
                        for (int channel = 0; channel < WChannels; channel++)
                        {
                            if (channel == 0)
                            {
                                var y = picBoxRight.Height * dataArray[x+ channel] / maxShort / 2 + picBoxRight.Height / 2;
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

        struct WavHeader
        {
            public byte[] riffID;
            public uint size;
            public byte[] wavID;
            public byte[] fmtID;
            public uint fmtSize;
            public ushort format;
            public ushort channels;
            public uint sampleRate;
            public uint bytePerSec;
            public ushort blockSize;
            public ushort bit;
            public byte[] dataID;
            public uint dataSize;
        }

        private WavHeader Header = new WavHeader();

        private void ReadBut_Click(object sender, EventArgs e)
        {
            
            List<Int16> lDataList = new List<Int16>();
            int N = 0;
            //string filePath = @"C:\C#\2.wav";
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                try
                {
                    Header.riffID = br.ReadBytes(4);//1
                    Header.size = br.ReadUInt32();//2
                    Header.wavID = br.ReadBytes(4);//3
                    Header.fmtID = br.ReadBytes(4);//4
                    Header.fmtSize = br.ReadUInt32();//5
                    Header.format = br.ReadUInt16();//6
                    Header.channels = br.ReadUInt16();//7
                    Header.sampleRate = br.ReadUInt32();//8
                    Header.bytePerSec = br.ReadUInt32();//9
                    Header.blockSize = br.ReadUInt16();//10
                    Header.bit = br.ReadUInt16();//11
                    Header.dataID = br.ReadBytes(6);//12
                    Header.dataSize = br.ReadUInt32();//13

                    int n = (int)(Header.dataSize / Header.channels * 8 / Header.bit);
                    
                    RTB.Text += "\nHeader.riffID -" + Header.riffID.ToString();
                    RTB.Text += "\nHeader.size -"+ Header.size.ToString();
                    RTB.Text += "\nHeader.wavID -"+ Header.wavID.ToString();
                    RTB.Text += "\nHeader.fmtID -"+ Header.fmtID.ToString();
                    RTB.Text += "\nHeader.fmtSize -"+ Header.fmtSize;
                    RTB.Text += "\nHeader.format -"+ Header.format;
                    RTB.Text += "\nHeader.channels -"+ Header.channels;
                    RTB.Text += "\nHeader.sampleRate -"+ Header.sampleRate;
                    RTB.Text += "\nHeader.bytePerSec -"+ Header.bytePerSec;
                    RTB.Text += "\nHeader.blockSize -"+ Header.blockSize;
                    RTB.Text += "\nHeader.bit -"+ Header.bit;
                    RTB.Text += "\nHeader.dataID -"+ Header.dataID;
                    RTB.Text += "\nHeader.dataSize -"+ Header.dataSize;
                    RTB.Text += "\nHeader.dataSize -"+ Header.dataSize;
                    RTB.Text += "\nHeader.channels -"+ Header.channels;
                    RTB.Text += "\nHeader.bit -"+ Header.bit;
                    RTB.Text += "\nN -"+ n;

                }
                finally
                {
                    if (br != null)
                    {
                        br.Close();
                    }
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }
            }
            SoundPlayer player = new SoundPlayer(filePath);
            player.Play();
            RTB.Text += "\n Открыт";
        }

        private void hScrollBar1_MouseLeave(object sender, EventArgs e)
        {
            if(oldHScrollBarValue!= HzScBar.Value)
            {
                ReGenerat();
                oldHScrollBarValue = HzScBar.Value;
            }
        }

        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            HzLabel.Text = HzScBar.Value +" Hz";
        }

        private void PlayBtn_Click(object sender, EventArgs e)
        {
            PlayFile();
        }

        private void VolumhScBar_ValueChanged(object sender, EventArgs e)
        {
            VolumLabel.Text = VolumhScBar.Value + " %";
        }
    }
}
