using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using AlfaPribor.VideoStorage2;
using FramesPlayer.Classes;
using System.IO;

namespace FramesPlayer
{
    public partial class FormThreadGraph : Form
    {

        private int _channelID;
        private string _videoFileName;
        
        
        public void SetNewChannel(int id)
        {
            _channelID = id;
            //GetFileIndexes();
            RunDrawGraph();
        }


        public FormThreadGraph()
        {
            InitializeComponent();
        }

        public FormThreadGraph(string videoFileName)
            : this()
        {
            _videoFileName = videoFileName;
            DrawStartStreamGraphic();
        }

        private void DrawStartStreamGraphic()
        {
            try
            {
                DelayTimer.Interval = 1000;
                DelayTimer.Enabled = true;
                DelayTimer.Start();
            }
            catch { }
        }

        private void DrawData(PointPairList list)
        {
            this.Invoke((MethodInvoker)(() =>
            {
                GraphPane pane = zgcTest.GraphPane;
                pane.CurveList.Clear();
                //SetGrahicDataSource(zgTest, list, "Test", "test", "XXX", "Channel", Color.Red);
                LineItem myCurve = pane.AddCurve("Sinc", list, Color.Blue, SymbolType.None);
                //Scroll for Zoom
                zgcTest.IsShowHScrollBar = true;
                zgcTest.IsShowVScrollBar = true;
                pane.Title.Text = "Изменение скорости состава";
                pane.XAxis.Title.Text = "Индекс кадра";
                pane.YAxis.Title.Text = "Скорость км/ч";
  

                zgcTest.AxisChange();
                zgcTest.Invalidate();
            }));
        }

        private void FormGraph_Load(object sender, EventArgs e)
        {
            PointPairList list = new PointPairList();
            for (int i = 0; i < 100; i++)
            {
                list.Add(new PointPair(i, Math.Cos(i)));
            }
            DrawData(list);
        }

        private void GetFileIndexes()
        {
            try
            {
                int trainID =0;
                int.TryParse(Path.GetFileNameWithoutExtension(_videoFileName),out trainID);
                string directoryName = Path.GetDirectoryName(_videoFileName);
                using (SpeedFile file = new SpeedFile { CatalogName = directoryName })
                {
                    var data = file.GetTrainData(trainID);
                    PointPairList graphData = new PointPairList();
                    for (int i = 0; i < data.Data.Count; i++)
                    {
                        if(data.Data[i].Channel==_channelID)
                             graphData.Add(new PointPair(i,data.Data[i].Speed));
                    }
                    DrawData(graphData);
                }
            }//В случае ошибки  не будет событие и просто ничего не отрисуется.
            catch { }
        }

        private void bRefresh_Click(object sender, EventArgs e)
        {
            try{

                SetNewChannel((int)nudChanelValue.Value);
            }catch{MessageBox.Show("Can not draw graphic.");}
        }

        private void RunDrawGraph()
        {
            if (!bwDrawGraphic.IsBusy)
            {
                bwDrawGraphic.RunWorkerAsync();
            }
        }

        private void bwDrawGraphic_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                GetFileIndexes();
            }
            catch { }
        }

        private void DelayTimer_Tick(object sender, EventArgs e)
        {
            DelayTimer.Stop();
            RunDrawGraph();
        }
    }
}
