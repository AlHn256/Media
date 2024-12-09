using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FramesPlayer.ExportConfiguration;

namespace FramesPlayer
{

    public partial class FormSelectWagons : Form
    {

        public FormSelectWagons()
        {
            InitializeComponent();
        }

        bool CheckSelectedValue(int wagonID)
        {
            if (SettingContainer.SelectedWagons.Count == 0)
            {
                return false;
            }
            return SettingContainer.SelectedWagons.Any(x=>x.WagId==wagonID); 
        }

        void BindDataToGrid()
        {
            gwWagonData.Rows.Clear();
            foreach (var dataItem in SettingContainer.WagonList.WagonDataList)
            {
                gwWagonData.Rows.Add(dataItem.WagId,
                                     dataItem.Sn,
                                     dataItem.SnSost,
                                     dataItem.InvNumber,
                                     dataItem.InvNumByNL,
                                     CheckSelectedValue(dataItem.WagId));
            }
        }

        void SelectWagonsForm_Load(object sender, EventArgs e)
        {
            BindDataToGrid();
        }

        void SetGridCheckBoxCheckedState(bool state)
        {
            foreach(DataGridViewRow row in gwWagonData.Rows)
            {
                IEnumerable<DataGridViewCheckBoxCell> checkBoxes = row.Cells.OfType<DataGridViewCheckBoxCell>();
                foreach (DataGridViewCheckBoxCell checkBox in checkBoxes)
                {
                    checkBox.Value = state ? checkBox.TrueValue : checkBox.FalseValue;
                }
            }
        }

        void SelectAllButton_Click(object sender, EventArgs e)
        {
            SetGridCheckBoxCheckedState(true);
        }

        void DeselectAllButton_Click(object sender, EventArgs e)
        {
            SetGridCheckBoxCheckedState(false);
        }

        List<int> GetSelectedWagonIDListFromGrid()
        {
            List<int> result = new List<int>();
            foreach (DataGridViewRow row in gwWagonData.Rows)
            {
                IEnumerable<DataGridViewCheckBoxCell> checkBoxes = row.Cells.OfType<DataGridViewCheckBoxCell>();
                foreach (DataGridViewCheckBoxCell checkBox in checkBoxes)
                {
                    if (checkBox.Value == checkBox.TrueValue)
                    {
                        result.Add(Convert.ToInt32(row.Cells[0].Value));
                    }
                }
            }
            return result;
        }
        
        void button1_Click(object sender, EventArgs e)
        {
            List<int> selectedIDList = GetSelectedWagonIDListFromGrid();
            SettingContainer.SelectedWagons = SettingContainer.WagonList.WagonDataList.Where(x => selectedIDList.Contains(x.WagId)).ToList();
            this.Close();
        }

        void CloseFormButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
