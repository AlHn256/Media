namespace FramesPlayer
{
    partial class SimpleGraphViewer
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
            this.components = new System.ComponentModel.Container();
            this.gbName = new System.Windows.Forms.GroupBox();
            this.zgcGraph = new ZedGraph.ZedGraphControl();
            this.gbName.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbName
            // 
            this.gbName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbName.Controls.Add(this.zgcGraph);
            this.gbName.Location = new System.Drawing.Point(4, 4);
            this.gbName.Name = "gbName";
            this.gbName.Size = new System.Drawing.Size(332, 245);
            this.gbName.TabIndex = 0;
            this.gbName.TabStop = false;
            this.gbName.Text = "groupBox1";
            // 
            // zgcGraph
            // 
            this.zgcGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.zgcGraph.Location = new System.Drawing.Point(6, 19);
            this.zgcGraph.Name = "zgcGraph";
            this.zgcGraph.ScrollGrace = 0D;
            this.zgcGraph.ScrollMaxX = 0D;
            this.zgcGraph.ScrollMaxY = 0D;
            this.zgcGraph.ScrollMaxY2 = 0D;
            this.zgcGraph.ScrollMinX = 0D;
            this.zgcGraph.ScrollMinY = 0D;
            this.zgcGraph.ScrollMinY2 = 0D;
            this.zgcGraph.Size = new System.Drawing.Size(320, 220);
            this.zgcGraph.TabIndex = 0;
            // 
            // SimpleGraphViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbName);
            this.Name = "SimpleGraphViewer";
            this.Size = new System.Drawing.Size(350, 249);
            this.Load += new System.EventHandler(this.SimpleGraphViewer_Load);
            this.gbName.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbName;
        private ZedGraph.ZedGraphControl zgcGraph;
    }
}
