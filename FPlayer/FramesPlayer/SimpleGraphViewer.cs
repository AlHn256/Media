using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AlfaPribor.VideoStorage2;
using System.Threading;
using ZedGraph;

namespace FramesPlayer
{
    public partial class SimpleGraphViewer : UserControl
    {
       
        public int ChannelID { get; set; }
        public IVideoIndex Index { get; set; }
        Thread _thread;

        public SimpleGraphViewer()
        {
            InitializeComponent();
        }

        public SimpleGraphViewer(string name, int channelID, IVideoIndex storrage):this()
        {
            ChannelID = channelID;
            Index = storrage;
        }

        private void DrawResult( List<double> x, List<double> y){
            if (zgcGraph.InvokeRequired)
                zgcGraph.Invoke((MethodInvoker)(() => { DrawGraphics(x, y); }));
            else
                DrawGraphics(x,y);
        }

        private void DrawGraphics(List<double> x, List<double> y)
        {

            GraphPane pane = zgcGraph.GraphPane;
            ClearGraphPane(pane);

            zgcGraph.IsShowHScrollBar = true;
            zgcGraph.IsShowVScrollBar = true;
            zgcGraph.IsAutoScrollRange = true;

            pane.Title.Text = string.Format("Канал {0}",ChannelID);
            pane.XAxis.Title.Text = "№ кадра";
            pane.XAxis.Type = AxisType.Log;
            pane.IsBoundedRanges = true;
            pane.Y2Axis.Title.Text = "Разница";

            PointPairList data = new PointPairList();
            for(int i=0;i<Math.Min(x.Count,y.Count);i++){
                data.Add(new PointPair(x[i],y[i]));
            }


             LineItem curve = pane.AddCurve("CH_ " + ChannelID.ToString(), data, Color.Red);
            curve.Symbol.Type = SymbolType.Diamond;
            zgcGraph.AxisChange();
            zgcGraph.Invalidate();
            Application.DoEvents();
        }

        private static void ClearGraphPane(GraphPane pane)
        {
            pane.Title.Text = string.Empty;
            pane.XAxis.Title.Text = string.Empty;
            while (pane.CurveList.Count > 0)
            {
                pane.CurveList.RemoveAt(0);
            }
        }

        private void GetFileIndexes()
        {
            try
            {
                int startTime = Index.GetStartTime(100);
                int lastTime = Index.GetFinishTime(100);
                List<double> timeIntervals = new List<double>();
                List<double> timeIndexes = new List<double>();
                int timeDelta = 10;
                int oldTime = startTime - timeDelta;
                int frameCounter = 0;
                for (int i = startTime; i <= lastTime; i += 10)
                {
                    int cadrTime = Index.GetFrameTime(ChannelID, i);
                    if (cadrTime != oldTime)
                    {
                        //обнаружен новый фрейм
                        int delta = cadrTime - oldTime;  
                        oldTime = cadrTime;
                        timeIntervals.Add(delta);
                        timeIndexes.Add(frameCounter++);
                    }
                }
                DrawResult(timeIndexes, timeIntervals);
            }//В случае ошибки  не будет событие и просто ничего не отрисуется.
            catch { MessageBox.Show("Can not draw item."); }
        }

        private void SimpleGraphViewer_Load(object sender, EventArgs e)
        {
            BindData();
        }

        private void BindData()
        {
            if (Index != null && ChannelID > -1)
            {
                _thread = new Thread(GetFileIndexes);
                _thread.Priority = ThreadPriority.Normal;
                _thread.Start();
            }
        }

        public void ReInit(string name, int channelID, IVideoIndex storrage)
        {
            ChannelID = channelID;
            Index = storrage;
            gbName.Text = "График для камеры " + channelID.ToString();
            BindData();
            Application.DoEvents();
        }

    }
}
