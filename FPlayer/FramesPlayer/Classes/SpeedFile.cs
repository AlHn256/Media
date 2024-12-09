using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace FramesPlayer.Classes
{
    public class SpeedInfo
    {
        public int Channel { get; set; }
        public long TimeStamp { get; set; }
        public double Direction { get; set; }
        public double Speed { get; set; }

        public SpeedInfo() { }
    }

    public class TrainSpeedData
    {
        public int ID { get; set; }
        public List<SpeedInfo> Data { get; set; }

        public TrainSpeedData()
        {
            ID = 0;
            Data = new List<SpeedInfo>();
        }

        public List<double> GetSpeeds()
        {
            List<double> result = new List<double>();
            for (int i = 0; i < Data.Count; i++)
                result.Add(Data[i].Speed);
            return result;
        }

        public List<double> GetDirections()
        {
            List<double> result = new List<double>();
            for (int i = 0; i < Data.Count; i++)
                result.Add(Data[i].Direction);
            return result;
        }
    }

    public class SpeedFile : IDisposable
    {
        private string _fileName = "speed";
        private string FileName { get { return _fileName; } set { _fileName = value; } }
        private int _trainID;
        private StreamWriter _writer;
        private int _frameIndex;
        bool _disposed;
        public string CatalogName { get; set; }


        public void WriteInfo(int trainID, int channelID, long timestamp, double speedValue, double direction)
        {
            if (_trainID != trainID)
            {
                _trainID = trainID;
                if (_writer != null)
                {
                    _writer.Flush();
                    _writer.Close();
                    _frameIndex = 0;
                }
                _writer = File.AppendText(GetFileName(trainID));
            }

            _writer.WriteLine(string.Format("{0} {1} {2}", channelID, timestamp, speedValue * direction));
        }

        private string GetFileName(int trainID)
        {
            return (CatalogName.EndsWith("\\") ? CatalogName : CatalogName += "\\") + string.Format("{0}.speed", trainID);
        }

        private SpeedInfo GetSpeedFromString(string speed)
        {
            SpeedInfo item = new SpeedInfo();
            try
            {
                string[] data = speed.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                item.Channel = Convert.ToInt32(data[0]);
                item.TimeStamp = Convert.ToInt32(data[1]);
                item.Speed = Convert.ToDouble(data[2]);
                item.Direction = item.Speed >= 0 ? (item.Speed == 0 ? 0 : 1) : -1;
            }
            catch { }
            return item;
        }

        public TrainSpeedData GetTrainData(int trainID)
        {
            TrainSpeedData item = new TrainSpeedData();
            item.ID = trainID;
            try
            {
                using (var reader = File.OpenText(GetFileName(trainID)))
                {
                    string row = null;
                    while ((row = reader.ReadLine()) != null)
                    {
                        item.Data.Add(GetSpeedFromString(row));
                    }
                }
            }
            catch { }
            return item;
        }

        public List<TrainSpeedData> GetAllTrainData()
        {
            List<TrainSpeedData> result = new List<TrainSpeedData>();
            string[] fileNames = Directory.GetFiles(CatalogName, "*.speed");
            foreach (string fileName in fileNames)
            {
                try
                {
                    result.Add(GetTrainData(Convert.ToInt32(Path.GetFileNameWithoutExtension(fileName))));
                }
                catch { }
            }
            return result;
        }

        public SpeedFile()
        {
            CatalogName = Application.StartupPath;
        }

        protected void Dispose(bool mode)
        {
            if (!_disposed)
            {

                if (_writer != null)
                {
                    _writer.Flush();
                    _writer.Close();
                    _writer = null;
                }
                _writer = null;
                _disposed = true;
                if (mode)
                    GC.SuppressFinalize(this);
            }
        }

        public void Dispose()
        {
            Dispose(false);
        }
    }
}
