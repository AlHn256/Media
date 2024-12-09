using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AlfaPribor.VideoStorage;
using ZedGraph;

namespace FramesPlayer
{
    public partial class ChannelGraphForm : Form
    {
        public ChannelGraphForm()
        {
            InitializeComponent();
        }
        List<IVideoIndex> _indexes;

        public ChannelGraphForm(List<IVideoIndex> indexes, string fileName):this()
        {
            InitializeComponent();
          //  SetStorrage(storage);
            _indexes = new List<IVideoIndex>();
            if(indexes.Count>0){
                _indexes.AddRange(indexes.ToArray());
            }
            nudCahannel.Maximum = _indexes.Count;
        }

        private void ChannelGraphForm_Load(object sender, EventArgs e)
        {
           // SetStorrage(_storrage);
        }

        private void SetGrahicDataSource(ZedGraphControl control,
                   PointPairList points,
                   string paneTitle,
                   string xAxisName,
                   string yAxisName,
                   string curveName,
                   Color color
           )
        {
            control.Invoke((MethodInvoker)(() =>
            {
                control.IsShowPointValues = true;
               // control.PointDateFormat = "dd-MMM HH:mm:ss";

                control.IsShowHScrollBar = true;
                control.IsShowVScrollBar = true;
                control.IsAutoScrollRange = true;
                ////Масштабирование с корректным скролом
                //control.ScrollGrace = 1;
                GraphPane pane = control.GraphPane;

                ClearGraphPane(pane);

                pane.Title.Text = paneTitle;
                //pane.XAxis.Title.Text = xAxisName;
                //pane.XAxis.Type = AxisType.Date;
                //pane.XAxis.Scale.Format = "dd-MMM HH:mm:ss";
                //pane.XAxis.Scale.MajorUnit = DateUnit.Minute;
                pane.IsBoundedRanges = true;
                pane.Y2Axis.Title.Text = yAxisName;

                LineItem curve = pane.AddCurve(curveName, points, color);
                curve.Symbol.Type = SymbolType.Diamond;

                // control.SetScrollRangeFromData();
                control.AxisChange();
                control.Invalidate();
            }));
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


        private void DrawData(ZedGraphControl zg,  PointPairList list)
        {
            GraphPane pane = zg.GraphPane;
            pane.CurveList.Clear();
            //SetGrahicDataSource(zgTest, list, "Test", "test", "XXX", "Channel", Color.Red);
            LineItem myCurve = pane.AddCurve("Sinc", list, Color.Blue, SymbolType.None);
            zg.AxisChange();
            zg.Invalidate();
        }

        private void DrawData(PointPairList list)
        {
            GraphPane pane = zedGraphControl1.GraphPane;

            pane.CurveList.Clear();
            //SetGrahicDataSource(zgTest, list, "Test", "test", "XXX", "Channel", Color.Red);
            LineItem myCurve = pane.AddCurve("Sinc", list, Color.Blue, SymbolType.None);
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }


        private void bSelect_Click(object sender, EventArgs e)
        {
            try
            {
              //  int selectedIndex = (int)nudCahannel.Value;
              //  int channelID=_indexes.First().StreamInfoList[selectedIndex].Id;
              // // FrameTimeGraphControl ftc = new FrameTimeGraphControl(channelID.ToString(), channelID, _indexes.First());
              // //// ftc.Location = new Point(pGraph.Location.X, pGraph.Location.Y);
              // // ftc.BackColor = Color.Red;
              // // ftc.Size = pGraph.Size;
              // //pGraph.Controls.Add(ftc);
              // //ftc.BringToFront();
              // //pGraph.Invalidate();
              // //Application.DoEvents();
              // // ftc.RunThread();
              // // ftc.ReInit((int)nudCahannel.Value,_storrage);
              ////  ftgcGraph.ReInit(channelID, _indexes.First());

              //  PointPairList list = new PointPairList();
              //  for (int i = 0; i < 100; i++)
              //  {
              //      list.Add(new PointPair(i,i*Math.Sin(i)));
              //  }
              //  DrawData(zgTest, list);

                PointPairList list = new PointPairList();
                for (int i = 0; i < 100; i++)
                {
                    list.Add(new PointPair(i, (i)*Math.Cos(i)));
                }
                DrawData(list);
            } 
            catch { MessageBox.Show("Не удалось построить график по данному каналу"); }
        }

        //private void SetStorrage(IVideoStorage storageObj)
        //{
        //    _storrage = storageObj;
        //    if (storageObj != null)
        //    {
        //        cgpGraph.Storrage = storageObj;
        //        cgpGraph.RefreshData();
        //    }
        //}
    }
}
