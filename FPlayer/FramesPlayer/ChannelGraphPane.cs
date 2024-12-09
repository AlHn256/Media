using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AlfaPribor.VideoStorage;

namespace FramesPlayer
{
    public partial class ChannelGraphPane : UserControl
    {
        public ChannelGraphPane()
        {
            InitializeComponent();
        }
        private IVideoStorage _storrage;
        public IVideoStorage Storrage {
            get
            {
                if (_storrage == null)
                    _storrage = new VideoStorage();
                return _storrage;
            }
            set
            {
                if (value != null)
                {
                    _storrage = value;
                }
            }
        
        }

        private void cbChannels_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSelectionChannel();
        }

        private void ShowSelectionChannel()
        {
            try
            {
               // ftgcGraph.ReInit(Convert.ToInt32(cbChannels.Text), Storrage);
            }
            catch
            {
                MessageBox.Show("Не удалось отобразить график");
            }
        }
        public void RefreshData()
        {
          ////  List<int> channels = new List<int>();
          //  if (Storrage != null)
          //  {
          //      foreach (var part in Storrage.Info.Partitions)
          //      {
          //          channels.Add(part.Id);
          //      }
          //      cbChannels.DataSource = channels;
          //      cbChannels.Refresh();
          //  }
        }

        private void cbChannels_Enter(object sender, EventArgs e)
        {
            ShowSelectionChannel();
        }

        private void bSelectData_Click(object sender, EventArgs e)
        {
            ShowSelectionChannel();
        }
    }
}
