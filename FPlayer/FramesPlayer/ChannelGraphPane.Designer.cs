namespace FramesPlayer
{
    partial class ChannelGraphPane
    {
        /// <summary> 
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.pGraph = new System.Windows.Forms.Panel();
            this.pCommand = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.cbChannels = new System.Windows.Forms.ComboBox();
            this.bSelectData = new System.Windows.Forms.Button();
            this.ftgcGraph = new FramesPlayer.FrameTimeGraphControl();
            this.pGraph.SuspendLayout();
            this.pCommand.SuspendLayout();
            this.SuspendLayout();
            // 
            // pGraph
            // 
            this.pGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pGraph.Controls.Add(this.ftgcGraph);
            this.pGraph.Location = new System.Drawing.Point(4, 51);
            this.pGraph.Name = "pGraph";
            this.pGraph.Size = new System.Drawing.Size(290, 225);
            this.pGraph.TabIndex = 1;
            // 
            // pCommand
            // 
            this.pCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pCommand.Controls.Add(this.label1);
            this.pCommand.Controls.Add(this.cbChannels);
            this.pCommand.Location = new System.Drawing.Point(4, 4);
            this.pCommand.Name = "pCommand";
            this.pCommand.Size = new System.Drawing.Size(290, 41);
            this.pCommand.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Выберите канал";
            // 
            // cbChannels
            // 
            this.cbChannels.FormattingEnabled = true;
            this.cbChannels.Location = new System.Drawing.Point(111, 9);
            this.cbChannels.Name = "cbChannels";
            this.cbChannels.Size = new System.Drawing.Size(146, 21);
            this.cbChannels.TabIndex = 0;
            this.cbChannels.SelectedIndexChanged += new System.EventHandler(this.cbChannels_SelectedIndexChanged);
            this.cbChannels.Enter += new System.EventHandler(this.cbChannels_Enter);
            // 
            // bSelectData
            // 
            this.bSelectData.Location = new System.Drawing.Point(267, 11);
            this.bSelectData.Name = "bSelectData";
            this.bSelectData.Size = new System.Drawing.Size(24, 23);
            this.bSelectData.TabIndex = 2;
            this.bSelectData.Text = "...";
            this.bSelectData.UseVisualStyleBackColor = true;
            this.bSelectData.Click += new System.EventHandler(this.bSelectData_Click);
            // 
            // ftgcGraph
            // 
            this.ftgcGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ftgcGraph.ChannelID = 0;
            this.ftgcGraph.Location = new System.Drawing.Point(4, 4);
            this.ftgcGraph.Name = "ftgcGraph";
            this.ftgcGraph.Size = new System.Drawing.Size(283, 226);
            this.ftgcGraph.TabIndex = 0;
            // 
            // ChannelGraphPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bSelectData);
            this.Controls.Add(this.pCommand);
            this.Controls.Add(this.pGraph);
            this.Name = "ChannelGraphPane";
            this.Size = new System.Drawing.Size(297, 279);
            this.pGraph.ResumeLayout(false);
            this.pCommand.ResumeLayout(false);
            this.pCommand.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pGraph;
        private System.Windows.Forms.Panel pCommand;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbChannels;
        private FrameTimeGraphControl ftgcGraph;
        private System.Windows.Forms.Button bSelectData;
    }
}
