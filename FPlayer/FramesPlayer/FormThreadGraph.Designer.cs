namespace FramesPlayer
{
    partial class FormThreadGraph
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormThreadGraph));
            this.zgcTest = new ZedGraph.ZedGraphControl();
            this.label1 = new System.Windows.Forms.Label();
            this.nudChanelValue = new System.Windows.Forms.NumericUpDown();
            this.bRefresh = new System.Windows.Forms.Button();
            this.bwDrawGraphic = new System.ComponentModel.BackgroundWorker();
            this.DelayTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.nudChanelValue)).BeginInit();
            this.SuspendLayout();
            // 
            // zgcTest
            // 
            this.zgcTest.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.zgcTest.Location = new System.Drawing.Point(3, 37);
            this.zgcTest.Name = "zgcTest";
            this.zgcTest.ScrollGrace = 0D;
            this.zgcTest.ScrollMaxX = 0D;
            this.zgcTest.ScrollMaxY = 0D;
            this.zgcTest.ScrollMaxY2 = 0D;
            this.zgcTest.ScrollMinX = 0D;
            this.zgcTest.ScrollMinY = 0D;
            this.zgcTest.ScrollMinY2 = 0D;
            this.zgcTest.Size = new System.Drawing.Size(324, 277);
            this.zgcTest.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Канал   №";
            // 
            // nudChanelValue
            // 
            this.nudChanelValue.Location = new System.Drawing.Point(94, 10);
            this.nudChanelValue.Name = "nudChanelValue";
            this.nudChanelValue.Size = new System.Drawing.Size(86, 20);
            this.nudChanelValue.TabIndex = 2;
            // 
            // bRefresh
            // 
            this.bRefresh.Location = new System.Drawing.Point(186, 7);
            this.bRefresh.Name = "bRefresh";
            this.bRefresh.Size = new System.Drawing.Size(75, 23);
            this.bRefresh.TabIndex = 3;
            this.bRefresh.Text = "Обновить";
            this.bRefresh.UseVisualStyleBackColor = true;
            this.bRefresh.Click += new System.EventHandler(this.bRefresh_Click);
            // 
            // bwDrawGraphic
            // 
            this.bwDrawGraphic.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwDrawGraphic_DoWork);
            // 
            // DelayTimer
            // 
            this.DelayTimer.Tick += new System.EventHandler(this.DelayTimer_Tick);
            // 
            // FormThreadGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(339, 326);
            this.Controls.Add(this.bRefresh);
            this.Controls.Add(this.nudChanelValue);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.zgcTest);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormThreadGraph";
            this.Text = "График скорости движения состава";
            this.Load += new System.EventHandler(this.FormGraph_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudChanelValue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraph.ZedGraphControl zgcTest;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudChanelValue;
        private System.Windows.Forms.Button bRefresh;
        private System.ComponentModel.BackgroundWorker bwDrawGraphic;
        private System.Windows.Forms.Timer DelayTimer;
    }
}