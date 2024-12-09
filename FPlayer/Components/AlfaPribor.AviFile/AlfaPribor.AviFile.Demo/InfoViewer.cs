using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AlfaPribor.AviFile;

namespace AlfaPribor.AviFile.Demo
{
    public partial class formInfoViewer : Form
    {
        public formInfoViewer()
        {
            InitializeComponent();
        }

        public IList<AviStreamInfo> Data
        {
            get
            {
                return (IList<AviStreamInfo>)BindingSource.DataSource;
            }
            set
            {
                BindingSource.DataSource = value;
            }
        }

        public bool AllowCheck
        {
            get
            {
                return dataGridView.Columns.GetFirstColumn(DataGridViewElementStates.None).Visible;
            }
            set
            {
                dataGridView.Columns.GetFirstColumn(DataGridViewElementStates.None).Visible = value;
            }
        }

        public IList<int> CheckedStreamIndexes
        {
            get
            {
                if (!AllowCheck)
                {
                    return new List<int>();
                }
                else
                {
                    List<int> result = new List<int>();
                    foreach (DataGridViewRow row in dataGridView.Rows)
	                {
                        if ((bool)row.Cells[0].Value)
                        {
                            result.Add(row.Index);
                        }
	                }
                    return result;
                }
            }
        }

        private void dataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.Data[e.RowIndex].Selected = !this.Data[e.RowIndex].Selected;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView.IsCurrentCellDirty)
            {
                dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
    }

    public class AviStreamInfo
    {
        public AviStreamInfo()
        {
        }

        public AviStreamInfo(Avi.AVISTREAMINFO info)
        {
            fccTypeStr = Avi.GetFourCCstr(info.fccType);
            string desc = new string(info.szName);
            Description = desc.Remove(desc.IndexOf('\0'));
            fccHandlerStr = Avi.GetFourCCstr(info.fccHandler);
            Length = (int)info.dwLength;
            FrameRate = (int)((double)info.dwScale / (double)info.dwRate * 1000.0);
        }

        public string fccTypeStr { get; set; }

        public string Description { get; set; }

        public string fccHandlerStr { get; set; }

        public int Length { get; set; }

        public int FrameRate { get; set; }

        public bool Selected { get; set; }
    }
}
