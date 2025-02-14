using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;
using AlfaPribor.VideoStorage2;

namespace FramesPlayer
{
    public partial class FormGraph : Form
    {

        private IVideoIndex _videoIndex;
        private int _channelID;
        
        public void SetNewChannel(int id)
        {
            _channelID = id;
            //GetFileIndexes();
            RunDrawGraph();
        }

        public FormGraph()
        {
            InitializeComponent();
        }

        public FormGraph(IVideoIndex index)
            : this()
        {
            _videoIndex = index;
            DrawStartStreamGraphic(index);
        }

        private void DrawStartStreamGraphic(IVideoIndex index)
        {
            try
            {
                int value = index.StreamInfoList[0].Id;
                nudChanelValue.Value = value;

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
                pane.Title.Text = "Межкадровое время";
                pane.XAxis.Title.Text = "Индекс кадра";
                pane.YAxis.Title.Text = "Время между кадрами";
  

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
                //получение данных
                IVideoIndex index = _videoIndex;
              
                int startTime = index.GetStartTime(100);
                int lastTime = index.GetFinishTime(100);
                List<double> timeIntervals = new List<double>();
                List<double> timeIndexes = new List<double>();
                int timeDelta = 10;
                int oldTime = startTime - timeDelta;
                int frameCounter = 0;
                PointPairList graphData = new PointPairList();
                for (int i = startTime; i <= lastTime; i += 10)
                {
                    int cadrTime = index.GetFrameTime(_channelID, i);
                    if (cadrTime != oldTime)
                    {
                        //обнаружен новый фрейм
                        int delta = cadrTime-oldTime;
                        graphData.Add(new PointPair(frameCounter, delta));
                        oldTime = cadrTime;
                        timeIntervals.Add(oldTime);
                        timeIndexes.Add(frameCounter++);
                    }
                }
                DrawData(graphData);
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
