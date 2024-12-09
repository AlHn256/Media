using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using AlfaPribor.VideoStorage2;
using System.Threading;

namespace FramesPlayer
{
    public partial class FrameTimeGraphControl : UserControl
    {

        public delegate void GetTimeDataCompletedEventHandler(object sender, GetTimeDataCompletedEventArgs e);
        public event GetTimeDataCompletedEventHandler GetTimeDataCompleted;

        protected void RaiseGetTimeDataCompleted(object sender, GetTimeDataCompletedEventArgs e)
        {
            if (GetTimeDataCompleted != null)
                GetTimeDataCompleted(sender, e);
        }

        #region Variables

        private int _channelID;
        private GroupBox _gbName;
        private ZedGraphControl _zedGraph;
        public int ChannelID { get; set; }
        private IVideoIndex _currentIndex;
        Thread _thread;
        List<double> _indexes = new List<double>();
        List<double> _values = new List<double>();
        //Блокировка объекта VideoStorage на случай использования объекта в нескольких потоках 
        private object _lockObject = new object();
        //Блокировка изменения глобальных переменных, на случай перерисовки
        private object _dataLocker = new object();

        #endregion

        public FrameTimeGraphControl()
        {
            InitializeComponent();
        }

        public FrameTimeGraphControl(string name, int channelID, IVideoIndex storrage)
            : this()
        {
            InitData(channelID, storrage);
        }

        public void ReInit(int channelID, IVideoIndex storrage)
        {
            InitData(channelID, storrage);
            RunThread();
        }

        public void RunThread()
        {
            //Запуск потока расчёта
            _thread = new Thread(GetFileIndexes);
            _thread.Priority = ThreadPriority.Normal;
            _thread.Start();
        }

        private void InitData(int channelID, IVideoIndex storrage)
        {
            _currentIndex = storrage;
            ChannelID = channelID;
            this.Controls.Clear();
            //Создание компонентов
            CreateControls(channelID);
            //Подпись на событие окончание расчёта
            this.GetTimeDataCompleted += new GetTimeDataCompletedEventHandler(GraphControl_GetTimeDataCompleted);
        }

        private void CreateControls(int channelID)
        {
             //Создание GroupBox

            GroupBox gbName = new GroupBox{Name=string.Format("gbChannel_{0}",channelID), Text=string.Format("Канал {0}",channelID )};
             gbName.Width = this.Width-10;
            gbName.Height = this.Height-10;

            ZedGraphControl zedGraph = new ZedGraphControl();
            zedGraph.Name = string.Format("zgControl_Channel_{0}",channelID);
            zedGraph.Size = new Size(gbName.Width-10, gbName.Height-10);
            zedGraph.Location = new Point(5,5);
            zedGraph.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            zedGraph.BackColor = Color.Red;

            gbName.Controls.Add(zedGraph);
            
            this.Controls.Add(gbName);
            _gbName = gbName;
            _zedGraph = zedGraph;
            //Создание компонента
            Application.DoEvents();
        }

        void GraphControl_GetTimeDataCompleted(object sender, GetTimeDataCompletedEventArgs e)
        {
            lock (_dataLocker)
            {
                _indexes = e.Indexes;
                _values = e.Values;
            }
            if (!_zedGraph.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() =>
                {

                    DrawData();
                }));
            }
            else
            {
                DrawData();
            }
        }

        private void GetFileIndexes()
        {
            try
            {
                //получение данных
                IVideoIndex index = _currentIndex;
                int partitionID = 0;
                ////lock (_lockObject)
                ////{
                //    partitionID = _currentStorage.Info.Partitions[ChannelID].Id;
                //    IVideoReader reader = _currentStorage.GetReader(partitionID.ToString());
                //    index = reader.VideoIndex;
                ////}

                int startTime = index.GetStartTime(100);
                int lastTime = index.GetFinishTime(100);
                List<double> timeIntervals = new List<double>();
                List<double> timeIndexes = new List<double>();
                int timeDelta = 10;
                int oldTime = startTime - timeDelta;
                int frameCounter = 0;
                for (int i = startTime; i <= lastTime; i += 10)
                {
                    int cadrTime = index.GetFrameTime(ChannelID, i);
                    if (cadrTime != oldTime)
                    {
                        //обнаружен новый фрейм
                        oldTime = cadrTime;
                        timeIntervals.Add(oldTime);
                        timeIndexes.Add(frameCounter++);
                    }
                }
                RaiseGetTimeDataCompleted(this, new GetTimeDataCompletedEventArgs { ChannelID = ChannelID, Indexes = timeIndexes, Values = timeIntervals });
            }//В случае ошибки  не будет событие и просто ничего не отрисуется.
            catch { }
        }

        private void DrawData()
        {
            _zedGraph.GraphPane.AddCurve("Канал " + ChannelID.ToString(), _indexes.ToArray(), _values.ToArray(), Color.Red, SymbolType.Square);
            _zedGraph.AxisChange();
            _zedGraph.Invalidate();
        }

        private void FrameTimeGraphControl_Load(object sender, EventArgs e)
        {
         //   RunThread();
        }
    }

    public class GetTimeDataCompletedEventArgs{
        public List<double> Indexes{get;set;}
        public List<double> Values{get;set;}
        public int ChannelID{get;set;}
        
        public GetTimeDataCompletedEventArgs(){
            ChannelID=0;
            Indexes = new List<double>();
            Values=new List<double>();
        }
    }
}
