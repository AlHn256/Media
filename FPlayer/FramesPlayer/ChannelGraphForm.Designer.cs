namespace FramesPlayer
{
    partial class ChannelGraphForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.nudCahannel = new System.Windows.Forms.NumericUpDown();
            this.bSelect = new System.Windows.Forms.Button();
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            ((System.ComponentModel.ISupportInitialize)(this.nudCahannel)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Канал";
            // 
            // nudCahannel
            // 
            this.nudCahannel.Location = new System.Drawing.Point(123, 12);
            this.nudCahannel.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudCahannel.Name = "nudCahannel";
            this.nudCahannel.Size = new System.Drawing.Size(120, 20);
            this.nudCahannel.TabIndex = 2;
            // 
            // bSelect
            // 
            this.bSelect.Location = new System.Drawing.Point(277, 11);
            this.bSelect.Name = "bSelect";
            this.bSelect.Size = new System.Drawing.Size(95, 23);
            this.bSelect.TabIndex = 3;
            this.bSelect.Text = "Выбрать";
            this.bSelect.UseVisualStyleBackColor = true;
            this.bSelect.Click += new System.EventHandler(this.bSelect_Click);
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.Location = new System.Drawing.Point(12, 56);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.ScrollGrace = 0D;
            this.zedGraphControl1.ScrollMaxX = 0D;
            this.zedGraphControl1.ScrollMaxY = 0D;
            this.zedGraphControl1.ScrollMaxY2 = 0D;
            this.zedGraphControl1.ScrollMinX = 0D;
            this.zedGraphControl1.ScrollMinY = 0D;
            this.zedGraphControl1.ScrollMinY2 = 0D;
            this.zedGraphControl1.Size = new System.Drawing.Size(584, 203);
            this.zedGraphControl1.TabIndex = 4;
            // 
            // ChannelGraphForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 528);
            this.Controls.Add(this.zedGraphControl1);
            this.Controls.Add(this.bSelect);
            this.Controls.Add(this.nudCahannel);
            this.Controls.Add(this.label1);
            this.Name = "ChannelGraphForm";
            this.Text = "ChannelGraphForm";
            this.Load += new System.EventHandler(this.ChannelGraphForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudCahannel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudCahannel;
        private System.Windows.Forms.Button bSelect;
        private ZedGraph.ZedGraphControl zedGraphControl1;

    }
}