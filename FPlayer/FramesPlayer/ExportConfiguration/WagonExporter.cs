using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FramesPlayer.ExportConfiguration
{
    public class WagonExporter : IDisposable
    {

        public string ExportFileName { get; set; }
        
        StreamWriter _streamWriter;

        public WagonExporter(string filename)
        {
            try
            {
                ExportFileName = filename;
                _streamWriter = File.AppendText(filename);
            }
            catch { }
        }

        public void WriteHeaderRow()
        {
            try { _streamWriter.WriteLine((new WagonDataShort()).MakeHeaderString()); } catch { }
        }
        
        public void WriteWagonDataRow(WagonDataShort wagonData, string filePath)
        {
            try { _streamWriter.WriteLine(string.Format("{0} {1}", wagonData.ToString(), filePath)); } catch { }
        }

        public void WriteWagonDataRow(WagonDataShort wagonData)
        {
            try { _streamWriter.WriteLine(wagonData.ToString()); } catch { }
        }

        #region Члены IDisposable

        public void Dispose(bool disposed)
        {
            try
            {
                _streamWriter.Close();
                _streamWriter.Dispose();
            }
            catch { }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

    }
}
