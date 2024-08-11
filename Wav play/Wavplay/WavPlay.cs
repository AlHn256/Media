using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using Wavplay.Models;
using Wavplay.WaveGenerator;
using System.Media;

namespace Wavplay
{

    public partial class WavPlay : Form
    {
        private int delta = 500, FScan = 0, TScan = 0, FX, FlxUp, TlxUp, FrxUp, TrxUp, XScrBarAddition = -1, BiaLenght =-1;
        private Image MiniBkGr = null, UpBkGr = null;
        private string WavPlayPath = @"C:\CSH\Wav play\test2.wav";
        private bool FlPbIn = false;
        private List<string> FileList = new List<string>();
        private List<WavList> DataList = new List<WavList>();
        private List<ScanList> ScanList = new List<ScanList>();
        private HzConverter hzConverter = new HzConverter();
        private WaveGenerator.WaveGenerator waveGenerator = new WaveGenerator.WaveGenerator();
        private FileEdit fileEdit = new FileEdit();

        public WavPlay()
        {
            InitializeComponent();
            this.MouseWheel += new MouseEventHandler(picBoxDn_MouseWheel);
            ALoad();
            FileSearch();
            ShowPositionInfo();
            NextBtn.Enabled = false;
        }

        public void ALoad()
        {
            string[] loadData = fileEdit.AutoLoade().Split('\r').ToArray();
            if (loadData.Count() > 3) readtextbox.Text = loadData[0];
            Start();
            //if (loadData.Count() > 3)
            //{
            //    int hScrollBar = 0;
            //    Int32.TryParse(loadData[1], out hScrollBar);
            //    Int32.TryParse(loadData[2], out delta);
            //    hScrollBar1.Value = hScrollBar;
            //}
        }

        public void FileSearch()
        {
            if (!string.IsNullOrEmpty(readtextbox.Text))
            {
                string test = AppDomain.CurrentDomain.BaseDirectory;
                string dir = Path.GetDirectoryName(readtextbox.Text);
                if (Directory.Exists(dir))FileList = fileEdit.SearchFiles(dir, new string[] { "*.wav" }).Select(x => x.FullName).ToList();
                else
                {
                    
                }
            }
        }

        public void NHzShow(int Fl)
        {
            if (Fl == 1) RTB.Text += hzConverter.NHzShow();
            else RTB.Text = hzConverter.NHzShow();
        }

        void picBoxDn_MouseWheel(object sender, MouseEventArgs e)
        {
            int save = delta, hscb = hScrollBar1.Value;
            if (FlPbIn == true && e.Delta < 0)
            {
                double d = delta, f = d / picBoxUp.Width;
                hscb = (int)(f * FX) + hScrollBar1.Value;
            }

            if (e.Delta > 0) delta = delta * 2;
            else delta = delta / 2;
            if (delta > DataList.Count) delta = DataList.Count;
            if (delta < 10) delta = 10;

            if (hscb > hScrollBar1.Maximum) hscb = hScrollBar1.Maximum - delta - 100;

            if (save != delta)
            {
                hScrollBar1.Value = hscb + 1;
            }
        }
        private void picBoxUp_MouseDown(object sender, MouseEventArgs e) { FlxUp = e.X; FrxUp = e.X; }
        private void picBoxUp_MouseMove(object sender, MouseEventArgs e)
        {
            FX = e.X;
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                TlxUp = e.X;
                TrxUp = TlxUp;
                picBoxUp.Image = new Bitmap(picBoxUp.Width, picBoxUp.Height);
                using (Graphics g = Graphics.FromImage(picBoxUp.Image))
                {
                    double d = delta, f = picBoxUp.Width / d;
                    FlxUp = (int)(Math.Round(FlxUp / f) * f);
                    FrxUp = (int)(Math.Round(FrxUp / f) * f);
                    TlxUp = (int)(Math.Round(TlxUp / f) * f);
                    TrxUp = (int)(Math.Round(TrxUp / f) * f);

                    g.DrawImage(UpBkGr, 0, 0, new Rectangle(0, 0, picBoxUp.Width, picBoxUp.Height), GraphicsUnit.Pixel);

                    if (e.Button == MouseButtons.Left)
                    {
                        g.DrawLine(new Pen(Color.Red, 2), FlxUp, 0, FlxUp, picBoxUp.Height);
                        if (FlxUp != TlxUp)
                        {
                            g.DrawLine(new Pen(Color.Red, 2), TlxUp, 0, TlxUp, picBoxUp.Height);
                            g.DrawLine(new Pen(Color.Red, 4), FlxUp, 0, TlxUp, 0);
                            g.DrawLine(new Pen(Color.Red, 2), FlxUp, picBoxUp.Height - 1, TlxUp, picBoxUp.Height - 1);
                        }
                    }
                    else
                    {
                        g.DrawLine(new Pen(Color.Green, 2), FlxUp, 0, FlxUp, picBoxUp.Height);
                        if (FrxUp != TrxUp)
                        {
                            g.DrawLine(new Pen(Color.Green, 2), TrxUp, 0, TrxUp, picBoxUp.Height);
                            g.DrawLine(new Pen(Color.Green, 4), FrxUp, 0, TrxUp, 0);
                            g.DrawLine(new Pen(Color.Green, 2), FrxUp, picBoxUp.Height - 1, TrxUp, picBoxUp.Height - 1);
                        }
                    }
                }
            }
        }
        private void picBoxUp_MouseClick(object sender, MouseEventArgs e)
        {
            double d = delta, f = picBoxUp.Width / d;
            TlxUp = e.X;
            if (TlxUp < FlxUp)
            {
                int tmp = FlxUp;
                FlxUp = TlxUp;
                TlxUp = tmp;
            }

            FlxUp = (int)(Math.Round(FlxUp / f) * f);
            TlxUp = (int)(Math.Round(TlxUp / f) * f);
            FScan = (int)Math.Round(FlxUp / f) + hScrollBar1.Value;
            TScan = (int)Math.Round(TlxUp / f) + hScrollBar1.Value;
            if (TScan > hScrollBar1.Maximum) TScan = hScrollBar1.Maximum;
            if (FScan > hScrollBar1.Maximum) FScan = hScrollBar1.Maximum;

            if (e.Button == MouseButtons.Right && delta != 10)
            {
                delta = (int)((TlxUp - FlxUp) / f);
                if (delta > hScrollBar1.Maximum) delta = hScrollBar1.Maximum;
                if (delta < 10) delta = 10;
                hScrollBar1.Value = FScan;
                hScrollBar1.Maximum = DataList.Count - delta;
            }

            if (e.Button == MouseButtons.Left)
            {
                picBoxUp.Image = new Bitmap(picBoxUp.Width, picBoxUp.Height);
                using (Graphics g = Graphics.FromImage(picBoxUp.Image))
                {
                    g.DrawImage(UpBkGr, 0, 0, new Rectangle(0, 0, picBoxUp.Width, picBoxUp.Height), GraphicsUnit.Pixel);
                    g.DrawLine(new Pen(Color.Red, 1), FlxUp, 0, FlxUp, picBoxUp.Height);
                    if (FlxUp != TlxUp)
                    {
                        g.DrawLine(new Pen(Color.Red, 1), TlxUp, 0, TlxUp, picBoxUp.Height);
                        g.DrawLine(new Pen(Color.Red, 2), FlxUp, 0, TlxUp, 0);
                        g.DrawLine(new Pen(Color.Red, 2), FlxUp, picBoxUp.Height - 1, TlxUp, picBoxUp.Height - 1);
                    }
                }

            }
            ShowPositionInfo();
            //RTB.Text = "Delta - " + delta + " \\ " + DataList.Count + "\nhScrollBar1.Value " + hScrollBar1.Value + " \\ " + hScrollBar1.Maximum
            //    + "\nFlxUp " + FlxUp + " TlxUp " + TlxUp + " f " +f 
            //    + "\nFScan " + FScan + " TScan " + TScan;
        }

        private void UpDn_Click(object sender, EventArgs e)
        {
            if (FileList.Count != 0)
            {
                int N = -1;
                for (int i = 0; i < FileList.Count; i++) if (readtextbox.Text == FileList[i]) { N = i; break; }
                if (N == -1 || N == 0) readtextbox.Text = FileList[FileList.Count - 1];
                else
                {
                    if (((Button)sender).Name=="Dn")N++;
                    else N--;
                    readtextbox.Text = FileList[N];
                }
                Start();
                Fileinfo(readtextbox.Text);
            }
            else RTB.Text = "FileList.Count = 0";
        }

        private short[] GenerateData(int multiple = 1)
        {
            if (FScan < TScan)
            {
                short[] shortArr = new short[(TScan - FScan) * 2 * multiple];

                RTB.Text = "Delta - " + delta + " \\ " + DataList.Count + "\nhScrollBar1.Value " + hScrollBar1.Value + " \\ " + hScrollBar1.Maximum
                    + "\nFlxUp " + FlxUp + " TlxUp " + TlxUp + " f "
                    + "\nFScan " + FScan + " TScan " + TScan
                    + "\nShortArr.Count() " + shortArr.Count() + " multiple "+ multiple;

                int y = 0;

                for (int j = 0; j < multiple; j++)
                {
                    for (int i = FScan; i < TScan - 1; i++)
                    {
                        shortArr[y++] = DataList[i].UpLf;
                        shortArr[y++] = DataList[i].DnRt;
                    }
                }

                return shortArr;
            }
            else return new short[0];
        }

        private void Play(string file)
        {
            if (File.Exists(file))
            {
                SoundPlayer player = new SoundPlayer(file);
                player.Play();
            }
        }

        private void PlayBtn_Click(object sender, EventArgs e)
        {
            waveGenerator = new WaveGenerator.WaveGenerator(GenerateData());
            waveGenerator.Save(WavPlayPath);
            Play(WavPlayPath);
        }

        private void PlayX10Btn_Click(object sender, EventArgs e)
        {
            waveGenerator = new WaveGenerator.WaveGenerator(GenerateData(10));
            waveGenerator.Save(WavPlayPath);
            Play(WavPlayPath);
        }

        private void PlayX100Btn_Click(object sender, EventArgs e)
        {
            waveGenerator = new WaveGenerator.WaveGenerator(GenerateData(100));
            waveGenerator.Save(WavPlayPath);
            Play(WavPlayPath);
        }
        private void Play_Click(object sender, EventArgs e)
        {
            Play(readtextbox.Text);
        }

        public void Start()
        {
            if (File.Exists(readtextbox.Text))
            {
                int i = 0;
                using (FileStream fs = new FileStream(readtextbox.Text, FileMode.Open, FileAccess.Read))
                {
                    long N = 0;
                    short shl = 0, shr = 0;
                    DataList.Clear();

                    var header = new WavHeader();
                    int headerSize = Marshal.SizeOf(header);
                    byte[] buffer = new byte[headerSize];

                    fs.Read(buffer, 0, headerSize);
                    var headerPtr = Marshal.AllocHGlobal(headerSize);
                    Marshal.Copy(buffer, 0, headerPtr, headerSize);
                    Marshal.PtrToStructure(headerPtr, header);

                    N = fs.Length;
                    N = (N - headerSize) / 2;
                    byte[] bt = new byte[2];
                    for (i = 0; i < N / header.NumChannels; i++)
                    {
                        fs.Read(bt, 0, 2);
                        shl = BitConverter.ToInt16(bt, 0);
                        if (header.NumChannels == 2)
                        {
                            fs.Read(bt, 0, 2);
                            shr = BitConverter.ToInt16(bt, 0);
                        }
                        DataList.Add(new WavList(shl, shr));
                    }
                    if (fs != null) fs.Close();
                }

                if (DataList.Count != 0)
                    {
                        MiniBkGr = new Bitmap(minipictureBox.Width, minipictureBox.Height);
                        using (Graphics g = Graphics.FromImage(MiniBkGr))
                        {
                            float Y = minipictureBox.Height / 2;
                            g.Clear(Color.Black);

                            float d = DataList.Count;
                            float f = minipictureBox.Width / d;
                            if (DataList.Count > 100000)
                            {
                                int it = DataList.Count / 10000;
                                for (i = 0; i < DataList.Count; i++)
                                {
                                    i += it;
                                    if (i >= DataList.Count) i = DataList.Count - 1;
                                    Y = minipictureBox.Height / 2 - (float)DataList[i].UpLf / 500;
                                    g.DrawLine(new Pen(Color.Orange, 1), i * f, Y, i * f, minipictureBox.Height / 2);
                                }
                            }
                            else for (i = 0; i < DataList.Count; i++)
                                {
                                    Y = minipictureBox.Height / 2 - (float)DataList[i].UpLf / 500;
                                    g.DrawLine(new Pen(Color.Orange, 1), i * f, Y, i * f, minipictureBox.Height / 2);
                                }
                            g.DrawLine(new Pen(Color.Red, 1), 0, minipictureBox.Height / 2, minipictureBox.Width, minipictureBox.Height / 2);
                        }
                    }

                    hScrollBar1.Minimum = 0;
                    hScrollBar1.Maximum = DataList.Count;
                    if (DataList.Count > 1000)
                    {
                        delta = DataList.Count/10;
                        hScrollBar1.Maximum = DataList.Count- delta;
                        hScrollBar1.Value = 1;
                    }
                    else
                    {
                        delta = DataList.Count;
                        hScrollBar1.Value = 2;
                    }
                Redrawe();
                RedraweMiniPB();
            }
            else RTB.Text = "Нет файла " + readtextbox.Text + "\n";
        }
        
        public void Fileinfo(string file)
        {
            var header = new WavHeader();
            // Размер заголовка
            var headerSize = Marshal.SizeOf(header);
            var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            var buffer = new byte[headerSize];
            fileStream.Read(buffer, 0, headerSize);
            // Чтобы не считывать каждое значение заголовка по отдельности,
            // воспользуемся выделением unmanaged блока памяти
            var headerPtr = Marshal.AllocHGlobal(headerSize);
            // Копируем считанные байты из файла в выделенный блок памяти
            Marshal.Copy(buffer, 0, headerPtr, headerSize);
            // Преобразовываем указатель на блок памяти к нашей структуре
            Marshal.PtrToStructure(headerPtr, header);

            RTB.Text = "File: " + file + "\n\n";
            RTB.Text += "File Size " + fileStream.Length + " Bt " + fileStream.Length/1024 + " Kb\n";
            RTB.Text += "DataList.Count " + DataList.Count + "\n";
            RTB.Text += "hScrollBar1.Value " + hScrollBar1.Value + "\n";
            RTB.Text += "hScrollBar1.Maximum " + hScrollBar1.Maximum + "\n";
            RTB.Text += "vScrollBar1.Value " + vScrollBar.Value + "\n";
            int x = (int)DataList.Count - (int)header.Subchunk2Size / header.BlockAlign;
            RTB.Text += "DataList.Count-Subchunk2Size/BlockAlign " + x + "\n";

            RTB.Text += "delta " + delta + "\n\n";

            RTB.Text += "Sample rate: " + header.SampleRate + "\n";
            RTB.Text += "Channels: " + header.NumChannels + "\n";
            RTB.Text += "Bits per sample: " + header.BitsPerSample + "\n";
            RTB.Text += "Header Size " + Marshal.SizeOf(header) + " Bt\n";
            RTB.Text += "Header Size " + headerSize + " Bt\n\n";

            RTB.Text += "Audio Format " + header.AudioFormat + "\n";
            RTB.Text += "Bits per sample: " + header.BitsPerSample + "\n";
            RTB.Text += "BlockAlign " + header.BlockAlign + "\n";
            RTB.Text += "ByteRate " + header.ByteRate + "\n";
            RTB.Text += "Channels: " + header.NumChannels + "\n";
            RTB.Text += "ChunkId " + header.ChunkId + "\n";
            RTB.Text += "ChunkSize " + header.ChunkSize + "\n";
            RTB.Text += "Format " + header.Format + "\n";
            RTB.Text += "NumChannels " + header.NumChannels + "\n";
            RTB.Text += "SampleRate " + header.SampleRate + "\n";
            RTB.Text += "Subchunk1Id " + header.Subchunk1Id + "\n";
            RTB.Text += "Subchunk1Size " + header.Subchunk1Size + "\n";
            RTB.Text += "Subchunk2Id " + header.Subchunk2Id + "\n";
            RTB.Text += "Subchunk2Size " + header.Subchunk2Size + "\n";

            // Посчитаем длительность воспроизведения в секундах
            var durationSeconds =  header.Subchunk2Size / (header.BitsPerSample / 8.0) / header.NumChannels / header.SampleRate;
            var durationMinutes = (int)Math.Floor(durationSeconds / 60);
            durationSeconds = durationSeconds - (durationMinutes * 60);

            RTB.Text += "Duration: " + durationMinutes + ":" + durationSeconds+"\n";
            Marshal.FreeHGlobal(headerPtr);
        }

        private void ReedFile_Click(object sender, EventArgs e)
        {
            if (File.Exists(readtextbox.Text))
            {
                int i, x, y;
                FileStream fs = new FileStream(readtextbox.Text, FileMode.Open, FileAccess.Read);
                string txt = "";
                byte[] arrfile = new byte[fs.Length - 44];
                
                fs.Read(arrfile, 0, arrfile.Length);
                fs.Close();

                Fileinfo(readtextbox.Text);
                RTB.Text += "\n" + arrfile.Length + "\n\n";

                for (y = 0; y < arrfile.Length; y++)
                {
                    i = y;

                    if (i == 43) txt += "\n\n\n";

                    x = Convert.ToInt32(arrfile[i]);
                    i++;
                    txt += x + " ";
                    if (x < 10) txt += " ";
                    if (x < 100) txt += " ";

                    if (i != 0 && i != 1)
                    {
                        if (i == i / 8 * 8) txt += "  ";
                        if (i == i / 64 * 64) txt += "\n";
                    }
                }
                RTB.Text += txt + "\n" + y;
            }
            else RTB.Text = "Файла "+ readtextbox.Text + " нет!!!";
        }

        private void St1_Click(object sender, EventArgs e){Start();}
        private void minipictureBox_MouseDown(object sender, MouseEventArgs e){FX = e.X;}
        private void minipictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            double DTX = e.X, DFX = FX;

            if (DTX < 0) DTX = 0;
            if (DTX > minipictureBox.Width) DTX = minipictureBox.Width;

            if (DTX < DFX) { double tX = DFX; DFX = DTX; DTX = tX; }

            if (DTX - DFX != 0){delta = (int)(DataList.Count * (DTX-DFX+1) / minipictureBox.Width);}
            hScrollBar1.Value = (int)(DFX * DataList.Count / minipictureBox.Width) ;

            RTB.Text = "DFX " + DFX + " DTX " + DTX + "\nDataList.Count " + DataList.Count + " Delta " + delta;
            RTB.Text += "\nMaximum " + hScrollBar1.Maximum + " hScrollBar1.Value " + hScrollBar1.Value ;
        }
        private void minipictureBox_DoubleClick(object sender, EventArgs e)
        {
            FX = 0;
            delta = DataList.Count;
            hScrollBar1.Value = 0;

            RTB.Text = "DataList.Count " + DataList.Count + " Delta " + delta;
            RTB.Text += "\nMaximum " + hScrollBar1.Maximum + " hScrollBar1.Value " + hScrollBar1.Value;
        }

        private void Redrawe()
        {
            if (DataList.Count != 0)
            {
                int X = 0, It = 5;
                float Yu = picBoxUp.Height / 2, Yu2 = picBoxUp.Height / 2, Yd = picBoxDn.Height / 2;
                picBoxUp.Image = new Bitmap(picBoxUp.Width, picBoxUp.Height);
                using (Graphics Up = Graphics.FromImage(picBoxUp.Image))
                {
                    picBoxDn.Image = new Bitmap(picBoxDn.Width, picBoxDn.Height);
                    using (Graphics Dn = Graphics.FromImage(picBoxDn.Image))
                    {
                        Up.Clear(Color.Black);
                        Up.DrawLine(new Pen(Color.Red, 3), 0, picBoxUp.Height / 2, picBoxUp.Width, picBoxUp.Height / 2);
                        Dn.Clear(Color.Black);
                        Dn.DrawLine(new Pen(Color.Red, 3), 0, picBoxDn.Height / 2, picBoxDn.Width, picBoxDn.Height / 2);

                        float d = delta, f = picBoxUp.Width / d;

                        if (delta > 100000)
                        {
                            It = delta / 10000;
                            for (int i = hScrollBar1.Value; i < hScrollBar1.Value + delta + 5; i++)
                            {
                                i += It;
                                X = i - hScrollBar1.Value;
                                if (i < DataList.Count)
                                {
                                    Yu = picBoxUp.Height / 2 - (float)DataList[i].UpLf / vScrollBar.Value;
                                    Yd = picBoxDn.Height / 2 - (float)DataList[i].DnRt / vScrollBar.Value;
                                }
                                else
                                {
                                    Yu = picBoxUp.Height / 2;
                                    Yd = picBoxUp.Height / 2;
                                }

                                if (checkBox.Checked == true)
                                {
                                    Up.DrawLine(new Pen(Color.Yellow, 1), X * f, Yu, (X + 1) * f, Yu);
                                    Up.DrawLine(new Pen(Color.Yellow, 1), X * f, Yu2, X * f, Yu);
                                }
                                else Up.DrawLine(new Pen(Color.Yellow, 1), (X - 1) * f, Yu2, X * f, Yu);
                                Yu2 = Yu;

                                Dn.DrawLine(new Pen(Color.Orange, 1), (X - 1) * f, Yd, X * f, Yd);
                            }
                        }
                        else
                        {
                            for (int i = hScrollBar1.Value; i < hScrollBar1.Value + delta + 5; i++)
                            {
                                X = i - hScrollBar1.Value;
                                if (i < DataList.Count)
                                {
                                    Yu = picBoxUp.Height / 2 - (float)DataList[i].UpLf / vScrollBar.Value;
                                    //richTextBox1.Text += "\n" + i + "  " + DataList[i].UpLf;
                                    Yd = picBoxDn.Height / 2 - (float)DataList[i].DnRt / vScrollBar.Value;
                                }
                                else
                                {
                                    Yu = picBoxUp.Height / 2;
                                    Yd = picBoxUp.Height / 2;
                                }

                                if (checkBox.Checked == true)
                                {
                                    Up.DrawLine(new Pen(Color.Yellow, 1), X * f, Yu, (X + 1) * f, Yu);
                                    Up.DrawLine(new Pen(Color.Yellow, 1), X * f, Yu2, X * f, Yu);
                                }
                                else Up.DrawLine(new Pen(Color.Yellow, 1), (X - 1) * f, Yu2, X * f, Yu);
                                Yu2 = Yu;

                                Dn.DrawLine(new Pen(Color.Orange, 1), (X - 1) * f, Yd, X * f, Yd);
                            }
                        }

                    }
                }
                UpBkGr = new Bitmap(picBoxUp.Width, picBoxUp.Height);
                using (Graphics g = Graphics.FromImage(UpBkGr)) g.DrawImage(picBoxUp.Image, 0, 0, new Rectangle(0, 0, picBoxUp.Width, picBoxUp.Height), GraphicsUnit.Pixel);
            }
            else RTB.Text = "\nRedrawe Проблемки DataList.Count " + DataList.Count + " hScrollBar1.Value " + hScrollBar1.Value + " delta " + delta;
        }
        private void RedraweMiniPB()
        {
            minipictureBox.Image = new Bitmap(minipictureBox.Width, minipictureBox.Height);
            float d = DataList.Count;
            float f = hScrollBar1.Value / d;
            int FX = (int)(f * minipictureBox.Width) + 1,TX=0;

            using (Graphics g = Graphics.FromImage(minipictureBox.Image))
            {
                if (MiniBkGr != null) g.DrawImage(MiniBkGr, 0, 0, new Rectangle(0, 0, minipictureBox.Width, minipictureBox.Height), GraphicsUnit.Pixel);
                f = minipictureBox.Width * delta / d;

                if ((int)f <2) g.DrawLine(new Pen(Color.Red, 1), FX, 0, FX, minipictureBox.Height);
                else
                {
                    TX = FX + (int)f;
                    g.DrawLine(new Pen(Color.Red, 2), FX , 0, FX , minipictureBox.Height);
                    g.DrawLine(new Pen(Color.Red, 2), TX-1 , 0, TX -1, minipictureBox.Height);
                }
            }
        }

        private string GetHzFromScanList()
        {
            return hzConverter.GetHzFromScanList(ScanList);
        }
        private void ScanDrawe()
        {
            if (ScanList.Count != 0)
            {
                float Yd2 = picBoxDn.Height / 2, Yd = picBoxDn.Height / 2;
                picBoxDn.Image = new Bitmap(picBoxDn.Width, picBoxDn.Height);
                using (Graphics Dn = Graphics.FromImage(picBoxDn.Image))
                {
                    Dn.Clear(Color.Black);
                    Dn.DrawLine(new Pen(Color.Red, 3), 0, picBoxDn.Height / 2, picBoxDn.Width, picBoxDn.Height / 2);

                    float d = TScan - FScan;
                    float f = picBoxDn.Width / d;
                    int i, To = hScrollBar1.Value + delta;
                    for (i = 0; i < ScanList.Count; i++)
                    {
                        Yd = picBoxDn.Height / 2 - (float)ScanList[i].El / vScrollBar.Value;
                        Dn.DrawLine(new Pen(Color.Yellow, 1), i * f, Yd, (i + 1) * f, Yd);
                        Dn.DrawLine(new Pen(Color.Yellow, 1), i * f, Yd2, i * f, Yd);
                        if (ScanList[i].Fl1 == true) Dn.DrawLine(new Pen(Color.Red, 1), i * f, 0, i * f, Yd);
                        if (ScanList[i].Fl2 == true) Dn.DrawLine(new Pen(Color.Green, 1), i * f, picBoxDn.Height, i * f, Yd);
                        Yd2 = Yd;
                    }
                }
            }
        }
        private int ScanTest()
        {
            Scaner scaner = new Scaner(ScanList);
            return scaner.ScanTest();
        }
        private void Fscan()
        {
            Scaner scaner = new Scaner(DataList, FScan, TScan);
            //ScanList = scaner.GetScanListFromData(FScan, TScan, hScrollBar1.Value, delta);
            ScanList = scaner.Fscan();
            ScanDrawe();
        }
        private void ScanMetod1(int Par)
        {
            ScanList = new Scaner(ScanList).ScanMetod1(Par);
            ScanDrawe();
        }
        private void ScanMetod2(int Rad)
        {
            ScanList = new Scaner(ScanList).ScanMetod2(Rad);
            ScanDrawe();
        }
        private double ScanMetod3()
        {
            Scaner scaner = new Scaner(ScanList);
            double SrRad = scaner.ScanMetod3();
            double Hz = RadToHz(SrRad);
            RTB.Text = "\nSrRad - "+ SrRad+ "\nHz - "+ Hz+"\n"+scaner.ScanStatListShow();
            
            return SrRad;
        }
        //public void ScanStatListShow(int Fl)
        //{

        //    new Scaner(ScanList).ScanStatListShow();

        //    if (ScanStat.Count() != 0)
        //    {
        //        string txt = "";
        //        for (int i = 0; i < ScanStat.Count(); i++)
        //        {
        //            txt += "\n<<" + i + ">>\n |Count " + ScanStat[i].Count + " |ListCount " + ScanStat[i].ListCount() + " |SrRad " + ScanStat[i].SrRad + "|\n";
        //            for (int j = 0; j < ScanStat[i].Count; j++) txt += "|" + ScanStat[i].GetListElm(j);
        //            txt += "|";
        //        }
        //        if (Fl == 1) richTextBox1.Text += txt;
        //        else richTextBox1.Text = txt;
        //    }
        //}
        private void ScanTxt(int Fl)
        {
            if (Fl == 1) RTB.Text = new Scaner(ScanList).ScanTxt();
            else RTB.Text += new Scaner(ScanList).ScanTxt();
        }
        private void FullScan_Click(object sender, EventArgs e)
        {
            Fscan();
            ScanMetod1(0);
            ScanMetod2(0);
            //ScanTxt(1);
            double SrRad = ScanMetod3();
            
            //...richTextBox1.txt += "\n SrRad " + SrRad;

            //ScanStatListShow(1);
            //txt += "\n" + txt + "\n++++++++++++++++++++++";
            //txt += "\n\n SrRad " + SrRad + "\n Hz " + Hz + "\n" + HzInfo(Hz) + "\n\n++++++++++++++++++++++";
            //richTextBox1.Text = txt;

            //Metods sdf = new Metods();
            //sdf.SetList(ScanList);
            //richTextBox1.Text += "\n\nHz " + sdf.Hz();
            //ScanList.Clear();
            //richTextBox1.Text += "\nHz " + sdf.Hz();
            //ScanList = new List<WavPlay._ScanList>(sdf.GetList());
            //richTextBox1.Text += "\nScanList " + ScanList.Count + " " + sdf.ListCount();
        }
        
        //private void WavGen(bool ChengXScrBar = false)
        private double WavGen()
        {
            double Hz = GetHzFrTextBox();
            int scanDelta = Math.Abs(TScan - FScan);

            Scaner scaner = new Scaner(DataList, FScan, TScan);
            //if (ChengXScrBar)
            //{
            //    BiaLenght = (int)((double)picBoxUp.Width * 44100 / scanDelta / Hz);
            //    XScrBarAddition = scaner.GetStartUpPoint() * picBoxUp.Width / scanDelta;
            //    if (XScrBarAddition > 0)
            //    {
            //        BiasHzScrollBar.Maximum = (int)((BiaLenght + XScrBarAddition) * 1.5) + 9;
            //        BiasHzScrollBar.Value = (BiasHzScrollBar.Maximum + BiasHzScrollBar.Minimum)/2;
            //    }
            //}

            if (AutoVolumChkBox.Checked)
            {
                var maxVol = scaner.GetMaxVolum();
                VolScBar.Value = 109 - maxVol * 109 / short.MaxValue;
            }

            double time = 1.5*(double)scanDelta / 44100;
            int volum = 109 - VolScBar.Value;
            waveGenerator.GeneratWaveArray(Hz, WaveExampleType.ExampleSineWave, volum, time);
            return DrawComparisonImg();
        }

        private List<IntSumm> SUMMList = new List<IntSumm>();
        private double DrawComparisonImg()
        {          
            double SUMM = 0;
            var dataArray = waveGenerator.GetData().shortArray;
            if (dataArray.Length != 0)
            {
                picBoxDn.Image = new Bitmap(picBoxDn.Width, picBoxDn.Height);
                using (Graphics Comp = Graphics.FromImage(picBoxDn.Image))
                {
                    short maxShort = short.MaxValue;
                    int Ig = 0, Nchannel = SecondChanalChckBox.Checked ? 1 : 0, CDelta = Math.Abs(TScan - FScan);
                    float Y = picBoxDn.Height / 2, Y2 = picBoxDn.Height / 2, Yg = 0;
                    float X = 0, f = (float)picBoxDn.Width / CDelta;

                    Comp.DrawLine(new Pen(Color.Green, 3), 0, Y, picBoxDn.Width, Y);
                    if (BiaLenght > 0) Comp.DrawLine(new Pen(Color.Wheat, 3), XScrBarAddition, Y, XScrBarAddition + BiaLenght, Y);

                    if (FrameOverlayChckBox.Checked && DataList.Count != 0)
                    {
                        Comp.DrawLine(new Pen(Color.OrangeRed, 1), XScrBarAddition, picBoxDn.Height / 2, XScrBarAddition, 0);
                        if (ChekingGrafChkBox.Checked)
                        {
                            for (int I = 0; I < CDelta; I++)
                            {
                                X = I * f;
                                Y = picBoxDn.Height / 2 - picBoxDn.Height * DataList[I + FScan].UpLf / maxShort / 2;
                                Comp.DrawLine(new Pen(Color.Red, 1), X, Y2, X + 1, Y);
                                Y2 = Y;
                                if (X > XScrBarAddition)
                                {
                                    if (X < 900)
                                    {
                                        var itogo = Math.Abs(dataArray[Ig * 2 + Nchannel] - DataList[I + FScan].UpLf);
                                        SUMM += itogo;
                                        Comp.DrawLine(new Pen(Color.DeepPink, 1), X, picBoxDn.Height, X, picBoxDn.Height - (int)(itogo * 50 / maxShort));
                                    }
                                    if (dataArray.Length > (Ig + 2) * 2) Yg = picBoxDn.Height / 2 - picBoxDn.Height * dataArray[Ig++ * 2 + Nchannel] / maxShort / 2;
                                    Comp.DrawLine(new Pen(Color.YellowGreen, 1), X, Yg, ++X, Yg);
                                }
                            }
                        }
                    }
                }
            }
            return SUMM;
        }

        private void HzTextBox_TextChanged(object sender, EventArgs e)
        {
            double HzTextBoxVal = GetHzFrTextBox();
            if (HzTextBoxVal < 10 && HzTextBoxVal > 17000)
            {
                if (HzTextBoxVal < 10) HzTextBoxVal = 10;
                if (HzTextBoxVal > 17000) HzTextBoxVal = 17000;
                HzTextBox.Text = HzTextBoxVal.ToString();
            }

            NtLabel.Text = hzConverter.GetNtFromHz(HzTextBoxVal);
        }

        private double GetHzFrTextBox()
        {
            double HzTextBoxVal = 0;
            double.TryParse(HzTextBox.Text, out HzTextBoxVal);
            return HzTextBoxVal;
        }

        private void HzScrollBar_ValueChanged(object sender, EventArgs e)
        {
            XScrBarAddition = BiasHzScrollBar.Value;
            //PercentBiasHzScrollBarValue = (double)BiasHzScrollBar.Value / (double)(BiasHzScrollBar.Maximum-9) * 100;
            //BiasLabel.Text = BiasHzScrollBar.Value.ToString();
            //MaxBiasLabel.Text = BiasHzScrollBar.Maximum.ToString();
            //BiasPercentLabel.Text = PercentBiasHzScrollBarValue.ToString();
            DrawComparisonImg();
        }
        private void ShowPositionInfo(string AdditionTexToStart = "", string AdditionTexToEnd = "")
        {
            RTB.Text = AdditionTexToStart + "Delta " + delta + " \\ " + DataList.Count + "\nhScrollBar1.Value " + hScrollBar1.Value + " \\ " + hScrollBar1.Maximum
                + "\nFlxUp " + FlxUp + " TlxUp " + TlxUp
                + "\nFScan " + FScan + " TScan " + TScan
                + AdditionTexToEnd;
        }
        
        private List<_NHz> NHzList = HzConverter.NHzList;
        private int Elem = 0;
        private void AutoGenButn_Click(object sender, EventArgs e)
        {
            RTB.Text = string.Empty;
            AutoVolumChkBox.Checked = true;
            SUMMList = new List<IntSumm>();

            if (AutoScanChkBox.Checked) AutoGen();
            else
            {
                Elem = 29;
                HzTextBox.Text = NHzList[Elem].SrHz.ToString();
                RTB.Text += NHzList[Elem].SrHz + " Hz " + NHzList[Elem].Nt + " Nt ";
                WavGen();
                NextBtn.Enabled = true;
            }
        }

        private void AutoGen(int delta=0, double frHz=0, double toHz=0)
        {
            foreach (var elem in HzConverter.NHzList)
            {
                if (elem.GTR > 29 && elem.GTR < 80)
                {
                    HzTextBox.Text = elem.SrHz.ToString();
                    double integrSumm = WavGen();
                    RTB.Text += integrSumm + " - "+ elem.SrHz + " Hz " + elem.Nt + " Nt\n" ;
                    SUMMList.Add(new IntSumm(elem.SrHz, integrSumm));
                }
            }
        }

        private void GraphBtn_Click(object sender, EventArgs e)
        {
            if (SUMMList.Count != 0)
            {
                var minElem = SUMMList.Where(x => x.IntergSumm == SUMMList.Min(y => y.IntergSumm)).FirstOrDefault();
                if(minElem!=null)
                {
                    double max = SUMMList.Max(x => x.IntergSumm);
                    double F = (double)picBoxUp.Height / max / 2;
                    double L = (double)picBoxUp.Width / (SUMMList.Count + 1);
                    double X = L, Y = 0, Yprev = 0;
                    IntSumm intSumm = new IntSumm();

                    picBoxUp.Image = new Bitmap(picBoxUp.Width, picBoxUp.Height);
                    using (Graphics Graph = Graphics.FromImage(picBoxUp.Image))
                    {
                        foreach (var elem in SUMMList)
                        {
                            Y = picBoxUp.Height - elem.IntergSumm * F;
                            if (X != L) Graph.DrawLine(new Pen(Color.White, 2), (int)(X - L), (int)Yprev, (int)X, (int)Y);
                            if (elem.IntergSumm == minElem.IntergSumm)
                            {
                                Graph.DrawLine(new Pen(Color.YellowGreen, 5), (int)X, (int)Y, (int)X + 5, (int)Y);
                                intSumm = elem;
                            }
                            else Graph.DrawLine(new Pen(Color.Red, 5), (int)X, (int)Y, (int)X + 5, (int)Y);
                            X += L;
                            Yprev = Y;
                        }
                    }
                    RTB.Text += "Min " + intSumm.IntergSumm + " - " + intSumm.Hz + " Hz";
                    HzTextBox.Text = minElem.Hz.ToString();
                    WavGen();
                }
            }
            else RTB.Text = "SUMMList пустой!";
        }

        private void picBoxDn_MouseClick(object sender, MouseEventArgs e)
        {
            XScrBarAddition = e.X;
            if (XScrBarAddition > BiasHzScrollBar.Maximum)
            {
                if(XScrBarAddition<10) BiasHzScrollBar.Maximum = XScrBarAddition * 5;
                else BiasHzScrollBar.Maximum = XScrBarAddition * 2;
                BiasHzScrollBar.Minimum = 0;
            }
            BiasHzScrollBar.Value = XScrBarAddition;
        }
        private void NextBtn_Click(object sender, EventArgs e)
        {
            HzTextBox.Text = NHzList[++Elem].SrHz.ToString();
            RTB.Text += NHzList[Elem].SrHz + " Hz " + NHzList[Elem].Nt + " Nt ";
            WavGen();
            
            if (Elem > 80) NextBtn.Enabled = false;
        }

        private void Pl10HzBtn_Click(object sender, EventArgs e){HzTextBox.Text = (GetHzFrTextBox() + 10).ToString();WavGen();}
        private void Pl5HzBtn_Click(object sender, EventArgs e){HzTextBox.Text = (GetHzFrTextBox() +5).ToString();WavGen();}
        private void Pl1HzBtn_Click(object sender, EventArgs e){HzTextBox.Text = (GetHzFrTextBox() + 1).ToString();WavGen();}
        private void Pl01HzBtn_Click(object sender, EventArgs e){HzTextBox.Text = (GetHzFrTextBox() + 0.1).ToString();WavGen();}
        private void Mn01HzBtn_Click(object sender, EventArgs e){HzTextBox.Text = (GetHzFrTextBox() - 0.1).ToString();WavGen();}
        private void Mn1HzBtn_Click(object sender, EventArgs e){HzTextBox.Text = (GetHzFrTextBox() - 1).ToString();WavGen();}
        private void Mn5HzBtn_Click(object sender, EventArgs e){HzTextBox.Text = (GetHzFrTextBox() - 5).ToString();WavGen();}
        private void Mn10HzBtn_Click(object sender, EventArgs e){HzTextBox.Text = (GetHzFrTextBox() - 10).ToString();WavGen();}
        private void G10Btn_Click(object sender, EventArgs e){HzTextBox.Text = hzConverter.GetHzByNt("1.0").ToString();WavGen();}
        private void G15Btn_Click(object sender, EventArgs e){HzTextBox.Text = hzConverter.GetHzByNt("1.5").ToString();WavGen();}
        private void G110Btn_Click(object sender, EventArgs e){HzTextBox.Text = hzConverter.GetHzByNt("1.10").ToString();WavGen();}
        private void G112Btn_Click(object sender, EventArgs e){HzTextBox.Text = hzConverter.GetHzByNt("1.12").ToString();WavGen();}
        private void G20Btn_Click(object sender, EventArgs e){HzTextBox.Text = hzConverter.GetHzByNt("2.0").ToString();WavGen();}
        private void G30Btn_Click(object sender, EventArgs e){HzTextBox.Text = hzConverter.GetHzByNt("3.0").ToString();WavGen();}
        private void G40Btn_Click(object sender, EventArgs e){HzTextBox.Text = hzConverter.GetHzByNt("4.0").ToString();WavGen();}
        private void G50Btn_Click(object sender, EventArgs e){HzTextBox.Text = hzConverter.GetHzByNt("5.0").ToString();WavGen();}
        private void G64Btn_Click(object sender, EventArgs e){HzTextBox.Text = hzConverter.GetHzByNt("6.4").ToString();WavGen();}
        private void G60Btn_Click(object sender, EventArgs e){HzTextBox.Text = hzConverter.GetHzByNt("6.0").ToString();WavGen();}
        private void Gmn65Btn_Click(object sender, EventArgs e){HzTextBox.Text = hzConverter.GetHzByNt("6.-5").ToString();WavGen(); }
        private void G6mn10Btn_Click(object sender, EventArgs e) { HzTextBox.Text = hzConverter.GetHzByNt("6.-10").ToString(); WavGen(); }
        private void WavGenButn_Click(object sender, EventArgs e) { WavGen(); }
        private void Tmp_Click(object sender, EventArgs e){foreach (string str in FileList) RTB.Text += str + "\n";}
        private void Scan_Click(object sender, EventArgs e){Fscan();}
        private void picBoxUp_MouseEnter(object sender, EventArgs e){FlPbIn = true;picBoxUp.Focus();}
        private void picBoxUp_MouseLeave(object sender, EventArgs e){FlPbIn = false;}
        private void ScanText_Click(object sender, EventArgs e) { ScanTxt(1); }
        private void vScrollBar1_ValueChanged(object sender, EventArgs e) { if (DataList.Count != 0)Redrawe(); }
        private void hScrollBar1_ValueChanged(object sender, EventArgs e) { FScan = hScrollBar1.Value; TScan = hScrollBar1.Value +delta; Redrawe(); RedraweMiniPB(); }
        private void Info_Click(object sender, EventArgs e) { Fileinfo(readtextbox.Text); }
        private void checkBox_CheckedChanged(object sender, EventArgs e) { Redrawe(); }
        private void GetNtBtn_Click(object sender, EventArgs e){NtLabel.Text = hzConverter.GetNtFromHz(HzTextBox.Text);}
        private void CrearTextBTN_Click(object sender, EventArgs e){RTB.Text = string.Empty;}
        private void WavGeneratorButtn_Click(object sender, EventArgs e){ new WaveGen().Show(); }
        //private void VolScBar_ValueChanged(object sender, EventArgs e){AutoVolumChkBox.Checked = false;}
        private void ScHz_Click(object sender, EventArgs e) { RTB.Text =  GetHzFromScanList().ToString(); }
        private void ScTest_Click(object sender, EventArgs e) { RTB.Text = "ScanTest " + ScanTest(); }
        private void Mt1_Click(object sender, EventArgs e) { ScanMetod1(10); ScanDrawe(); }
        private void Mt2_Click(object sender, EventArgs e) { ScanMetod2(0); ScanDrawe(); }
        private void Mt3_Click(object sender, EventArgs e) { ScanMetod3(); ScanDrawe(); }
        private void HzShow_Click(object sender, EventArgs e) { NHzShow(1); }
        private void ShowHzmini_Click(object sender, EventArgs e){RTB.Text += hzConverter.NHzShowMini();}
        private double RadToHz(double Rad) { return 44100 / Rad; }
        private void WavPlay_FormClosed(object sender, FormClosedEventArgs e)
        {
            fileEdit.AutoSave(new string[] { readtextbox.Text, hScrollBar1.Value.ToString(), delta.ToString() });
        }
    }
}
