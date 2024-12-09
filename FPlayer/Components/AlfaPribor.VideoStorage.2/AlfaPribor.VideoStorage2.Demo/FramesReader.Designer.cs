namespace AlfaPribor.VideoStorage.Demo
{
    partial class FramesReader
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnIndex = new System.Windows.Forms.Button();
            this.btnReadMany = new System.Windows.Forms.Button();
            this.btnReadNext = new System.Windows.Forms.Button();
            this.btnReadFirst = new System.Windows.Forms.Button();
            this.btnReadOne = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numDeltaTime = new System.Windows.Forms.NumericUpDown();
            this.chbUseDeltaTime = new System.Windows.Forms.CheckBox();
            this.numStreamId = new System.Windows.Forms.NumericUpDown();
            this.numOffset = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numTimeStamp = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnView = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txtbId = new System.Windows.Forms.TextBox();
            this.numPartitionId = new System.Windows.Forms.NumericUpDown();
            this.chbUsePartitionId = new System.Windows.Forms.CheckBox();
            this.btnReadLast = new System.Windows.Forms.Button();
            this.btnReadPrev = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDeltaTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStreamId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeStamp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPartitionId)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnReadPrev);
            this.groupBox1.Controls.Add(this.btnReadLast);
            this.groupBox1.Controls.Add(this.btnIndex);
            this.groupBox1.Controls.Add(this.btnReadMany);
            this.groupBox1.Controls.Add(this.btnReadNext);
            this.groupBox1.Controls.Add(this.btnReadFirst);
            this.groupBox1.Controls.Add(this.btnReadOne);
            this.groupBox1.Location = new System.Drawing.Point(12, 205);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(268, 222);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Действия";
            // 
            // btnIndex
            // 
            this.btnIndex.Location = new System.Drawing.Point(17, 192);
            this.btnIndex.Name = "btnIndex";
            this.btnIndex.Size = new System.Drawing.Size(237, 23);
            this.btnIndex.TabIndex = 4;
            this.btnIndex.Text = "Показать индекс";
            this.btnIndex.UseVisualStyleBackColor = true;
            this.btnIndex.Click += new System.EventHandler(this.btnIndex_Click);
            // 
            // btnReadMany
            // 
            this.btnReadMany.Location = new System.Drawing.Point(17, 48);
            this.btnReadMany.Name = "btnReadMany";
            this.btnReadMany.Size = new System.Drawing.Size(237, 23);
            this.btnReadMany.TabIndex = 3;
            this.btnReadMany.Text = "Читать несколько";
            this.btnReadMany.UseVisualStyleBackColor = true;
            this.btnReadMany.Click += new System.EventHandler(this.btnReadMany_Click);
            // 
            // btnReadNext
            // 
            this.btnReadNext.Enabled = false;
            this.btnReadNext.Location = new System.Drawing.Point(17, 105);
            this.btnReadNext.Name = "btnReadNext";
            this.btnReadNext.Size = new System.Drawing.Size(237, 23);
            this.btnReadNext.TabIndex = 2;
            this.btnReadNext.Text = "Читать следующий";
            this.btnReadNext.UseVisualStyleBackColor = true;
            this.btnReadNext.Click += new System.EventHandler(this.btnReadNext_Click);
            // 
            // btnReadFirst
            // 
            this.btnReadFirst.Location = new System.Drawing.Point(17, 76);
            this.btnReadFirst.Name = "btnReadFirst";
            this.btnReadFirst.Size = new System.Drawing.Size(237, 23);
            this.btnReadFirst.TabIndex = 1;
            this.btnReadFirst.Text = "Читать первый";
            this.btnReadFirst.UseVisualStyleBackColor = true;
            this.btnReadFirst.Click += new System.EventHandler(this.btnReadFirst_Click);
            // 
            // btnReadOne
            // 
            this.btnReadOne.Location = new System.Drawing.Point(17, 19);
            this.btnReadOne.Name = "btnReadOne";
            this.btnReadOne.Size = new System.Drawing.Size(237, 23);
            this.btnReadOne.TabIndex = 0;
            this.btnReadOne.Text = "Читать один";
            this.btnReadOne.UseVisualStyleBackColor = true;
            this.btnReadOne.Click += new System.EventHandler(this.btnReadOne_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numDeltaTime);
            this.groupBox2.Controls.Add(this.chbUseDeltaTime);
            this.groupBox2.Controls.Add(this.numStreamId);
            this.groupBox2.Controls.Add(this.numOffset);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.numTimeStamp);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 68);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(268, 131);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Данные кадра";
            // 
            // numDeltaTime
            // 
            this.numDeltaTime.Location = new System.Drawing.Point(174, 68);
            this.numDeltaTime.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numDeltaTime.Name = "numDeltaTime";
            this.numDeltaTime.Size = new System.Drawing.Size(80, 20);
            this.numDeltaTime.TabIndex = 11;
            // 
            // chbUseDeltaTime
            // 
            this.chbUseDeltaTime.AutoSize = true;
            this.chbUseDeltaTime.Location = new System.Drawing.Point(17, 68);
            this.chbUseDeltaTime.Name = "chbUseDeltaTime";
            this.chbUseDeltaTime.Size = new System.Drawing.Size(133, 17);
            this.chbUseDeltaTime.TabIndex = 10;
            this.chbUseDeltaTime.Text = "Временной интервал";
            this.chbUseDeltaTime.UseVisualStyleBackColor = true;
            // 
            // numStreamId
            // 
            this.numStreamId.Location = new System.Drawing.Point(174, 17);
            this.numStreamId.Name = "numStreamId";
            this.numStreamId.Size = new System.Drawing.Size(80, 20);
            this.numStreamId.TabIndex = 9;
            // 
            // numOffset
            // 
            this.numOffset.Location = new System.Drawing.Point(174, 95);
            this.numOffset.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numOffset.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numOffset.Name = "numOffset";
            this.numOffset.Size = new System.Drawing.Size(80, 20);
            this.numOffset.TabIndex = 6;
            this.numOffset.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(149, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Приращение к следующему";
            // 
            // numTimeStamp
            // 
            this.numTimeStamp.Location = new System.Drawing.Point(174, 42);
            this.numTimeStamp.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numTimeStamp.Name = "numTimeStamp";
            this.numTimeStamp.Size = new System.Drawing.Size(80, 20);
            this.numTimeStamp.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Метка времени";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Идентификатор видеопотока";
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Location = new System.Drawing.Point(205, 433);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Закрыть";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnView
            // 
            this.btnView.Location = new System.Drawing.Point(12, 433);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(75, 23);
            this.btnView.TabIndex = 5;
            this.btnView.Text = "Просмотр";
            this.btnView.UseVisualStyleBackColor = true;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(156, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Идентификатор видеозаписи";
            // 
            // txtbId
            // 
            this.txtbId.Location = new System.Drawing.Point(186, 15);
            this.txtbId.Name = "txtbId";
            this.txtbId.Size = new System.Drawing.Size(80, 20);
            this.txtbId.TabIndex = 8;
            this.txtbId.TextChanged += new System.EventHandler(this.txtbId_TextChanged);
            // 
            // numPartitionId
            // 
            this.numPartitionId.Location = new System.Drawing.Point(186, 41);
            this.numPartitionId.Name = "numPartitionId";
            this.numPartitionId.Size = new System.Drawing.Size(80, 20);
            this.numPartitionId.TabIndex = 10;
            // 
            // chbUsePartitionId
            // 
            this.chbUsePartitionId.AutoSize = true;
            this.chbUsePartitionId.Location = new System.Drawing.Point(22, 42);
            this.chbUsePartitionId.Name = "chbUsePartitionId";
            this.chbUsePartitionId.Size = new System.Drawing.Size(151, 17);
            this.chbUsePartitionId.TabIndex = 11;
            this.chbUsePartitionId.Text = "Идентификатор раздела";
            this.chbUsePartitionId.UseVisualStyleBackColor = true;
            // 
            // btnReadLast
            // 
            this.btnReadLast.Location = new System.Drawing.Point(17, 134);
            this.btnReadLast.Name = "btnReadLast";
            this.btnReadLast.Size = new System.Drawing.Size(237, 23);
            this.btnReadLast.TabIndex = 5;
            this.btnReadLast.Text = "Читать последний";
            this.btnReadLast.UseVisualStyleBackColor = true;
            this.btnReadLast.Click += new System.EventHandler(this.btnReadLast_Click);
            // 
            // btnReadPrev
            // 
            this.btnReadPrev.Enabled = false;
            this.btnReadPrev.Location = new System.Drawing.Point(17, 163);
            this.btnReadPrev.Name = "btnReadPrev";
            this.btnReadPrev.Size = new System.Drawing.Size(237, 23);
            this.btnReadPrev.TabIndex = 6;
            this.btnReadPrev.Text = "Читать предыдущий";
            this.btnReadPrev.UseVisualStyleBackColor = true;
            this.btnReadPrev.Click += new System.EventHandler(this.btnReadPrev_Click);
            // 
            // FramesReader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 461);
            this.Controls.Add(this.chbUsePartitionId);
            this.Controls.Add(this.numPartitionId);
            this.Controls.Add(this.txtbId);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnView);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FramesReader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Чтение видеокадров";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDeltaTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStreamId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeStamp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPartitionId)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnReadOne;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numTimeStamp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numOffset;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnReadFirst;
        private System.Windows.Forms.Button btnReadMany;
        private System.Windows.Forms.Button btnReadNext;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtbId;
        private System.Windows.Forms.NumericUpDown numStreamId;
        private System.Windows.Forms.Button btnIndex;
        private System.Windows.Forms.NumericUpDown numPartitionId;
        private System.Windows.Forms.CheckBox chbUsePartitionId;
        private System.Windows.Forms.CheckBox chbUseDeltaTime;
        private System.Windows.Forms.NumericUpDown numDeltaTime;
        private System.Windows.Forms.Button btnReadPrev;
        private System.Windows.Forms.Button btnReadLast;
    }
}