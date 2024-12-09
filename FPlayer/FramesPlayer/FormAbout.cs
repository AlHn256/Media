using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FramesPlayer
{
    public partial class FormAbout : Form
    {
        public FormAbout()
        {
            InitializeComponent();
        }

        private void FormAbout_Shown(object sender, EventArgs e)
        {
            labelVersion.Text = "Версия " + Application.ProductVersion;
        }
    }
}
