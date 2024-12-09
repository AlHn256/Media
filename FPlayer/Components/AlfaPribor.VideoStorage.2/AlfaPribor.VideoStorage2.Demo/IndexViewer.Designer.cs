namespace AlfaPribor.VideoStorage.Demo
{
    partial class IndexViewer
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtbId = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lvwStreamInfo = new System.Windows.Forms.ListView();
            this.clhStreamId = new System.Windows.Forms.ColumnHeader();
            this.clhContentType = new System.Windows.Forms.ColumnHeader();
            this.clhResolution = new System.Windows.Forms.ColumnHeader();
            this.clhRotation = new System.Windows.Forms.ColumnHeader();
            this.clhWidth = new System.Windows.Forms.ColumnHeader();
            this.clhHeight = new System.Windows.Forms.ColumnHeader();
            this.label4 = new System.Windows.Forms.Label();
            this.txtbEndRecord = new System.Windows.Forms.TextBox();
            this.txtbBeginRecord = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtbNextFrame = new System.Windows.Forms.TextBox();
            this.btnPrevFrame = new System.Windows.Forms.Button();
            this.btnNextFrame = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.numTimeStamp = new System.Windows.Forms.NumericUpDown();
            this.numStreamId = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtbPrevFrame = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtbLastFrame = new System.Windows.Forms.TextBox();
            this.txtbFirstFrame = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.numDeltaTime = new System.Windows.Forms.NumericUpDown();
            this.btnFirstFrame = new System.Windows.Forms.Button();
            this.btnLastFrame = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeStamp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStreamId)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDeltaTime)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Идентификатор видеозаписи";
            // 
            // txtbId
            // 
            this.txtbId.Location = new System.Drawing.Point(177, 6);
            this.txtbId.Name = "txtbId";
            this.txtbId.ReadOnly = true;
            this.txtbId.Size = new System.Drawing.Size(92, 20);
            this.txtbId.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lvwStreamInfo);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtbEndRecord);
            this.groupBox1.Controls.Add(this.txtbBeginRecord);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(282, 32);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(325, 280);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Данные";
            // 
            // lvwStreamInfo
            // 
            this.lvwStreamInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwStreamInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhStreamId,
            this.clhContentType,
            this.clhResolution,
            this.clhRotation,
            this.clhWidth,
            this.clhHeight});
            this.lvwStreamInfo.FullRowSelect = true;
            this.lvwStreamInfo.GridLines = true;
            this.lvwStreamInfo.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvwStreamInfo.Location = new System.Drawing.Point(9, 96);
            this.lvwStreamInfo.MultiSelect = false;
            this.lvwStreamInfo.Name = "lvwStreamInfo";
            this.lvwStreamInfo.Size = new System.Drawing.Size(306, 175);
            this.lvwStreamInfo.TabIndex = 5;
            this.lvwStreamInfo.UseCompatibleStateImageBehavior = false;
            this.lvwStreamInfo.View = System.Windows.Forms.View.Details;
            // 
            // clhStreamId
            // 
            this.clhStreamId.Text = "ID";
            this.clhStreamId.Width = 30;
            // 
            // clhContentType
            // 
            this.clhContentType.Text = "Тип содержимого";
            this.clhContentType.Width = 120;
            // 
            // clhResolution
            // 
            this.clhResolution.Text = "Разр.кадра";
            // 
            // clhRotation
            // 
            this.clhRotation.Text = "Угол пов-та";
            // 
            // clhWidth
            // 
            this.clhWidth.Text = "Ширина кадра";
            // 
            // clhHeight
            // 
            this.clhHeight.Text = "Высота кадра";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(155, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Информация о видеопотоках";
            // 
            // txtbEndRecord
            // 
            this.txtbEndRecord.Location = new System.Drawing.Point(112, 50);
            this.txtbEndRecord.Name = "txtbEndRecord";
            this.txtbEndRecord.ReadOnly = true;
            this.txtbEndRecord.Size = new System.Drawing.Size(142, 20);
            this.txtbEndRecord.TabIndex = 3;
            // 
            // txtbBeginRecord
            // 
            this.txtbBeginRecord.Location = new System.Drawing.Point(112, 24);
            this.txtbBeginRecord.Name = "txtbBeginRecord";
            this.txtbBeginRecord.ReadOnly = true;
            this.txtbBeginRecord.Size = new System.Drawing.Size(142, 20);
            this.txtbBeginRecord.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Окончание записи";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Начало записи";
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(506, 318);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(95, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Закрыть";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Location = new System.Drawing.Point(12, 32);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(264, 280);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Действия";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtbPrevFrame);
            this.groupBox3.Controls.Add(this.numTimeStamp);
            this.groupBox3.Controls.Add(this.numStreamId);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.txtbNextFrame);
            this.groupBox3.Controls.Add(this.btnPrevFrame);
            this.groupBox3.Controls.Add(this.btnNextFrame);
            this.groupBox3.Location = new System.Drawing.Point(9, 19);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(245, 138);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Позиционирование";
            // 
            // txtbNextFrame
            // 
            this.txtbNextFrame.Location = new System.Drawing.Point(153, 79);
            this.txtbNextFrame.Name = "txtbNextFrame";
            this.txtbNextFrame.ReadOnly = true;
            this.txtbNextFrame.Size = new System.Drawing.Size(86, 20);
            this.txtbNextFrame.TabIndex = 12;
            // 
            // btnPrevFrame
            // 
            this.btnPrevFrame.Location = new System.Drawing.Point(6, 106);
            this.btnPrevFrame.Name = "btnPrevFrame";
            this.btnPrevFrame.Size = new System.Drawing.Size(116, 23);
            this.btnPrevFrame.TabIndex = 10;
            this.btnPrevFrame.Text = "Предыдущий кадр";
            this.btnPrevFrame.UseVisualStyleBackColor = true;
            this.btnPrevFrame.Click += new System.EventHandler(this.btnPrevFrame_Click);
            // 
            // btnNextFrame
            // 
            this.btnNextFrame.Location = new System.Drawing.Point(6, 77);
            this.btnNextFrame.Name = "btnNextFrame";
            this.btnNextFrame.Size = new System.Drawing.Size(116, 23);
            this.btnNextFrame.TabIndex = 9;
            this.btnNextFrame.Text = "Следующий кадр";
            this.btnNextFrame.UseVisualStyleBackColor = true;
            this.btnNextFrame.Click += new System.EventHandler(this.btnNextFrame_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 170);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(126, 13);
            this.label10.TabIndex = 15;
            this.label10.Text = "Допуск метки времени";
            // 
            // numTimeStamp
            // 
            this.numTimeStamp.Location = new System.Drawing.Point(153, 51);
            this.numTimeStamp.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numTimeStamp.Name = "numTimeStamp";
            this.numTimeStamp.Size = new System.Drawing.Size(86, 20);
            this.numTimeStamp.TabIndex = 16;
            // 
            // numStreamId
            // 
            this.numStreamId.Location = new System.Drawing.Point(153, 25);
            this.numStreamId.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numStreamId.Name = "numStreamId";
            this.numStreamId.Size = new System.Drawing.Size(86, 20);
            this.numStreamId.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 53);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Метка времени";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(125, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Идентификатор потока";
            // 
            // txtbPrevFrame
            // 
            this.txtbPrevFrame.Location = new System.Drawing.Point(153, 108);
            this.txtbPrevFrame.Name = "txtbPrevFrame";
            this.txtbPrevFrame.ReadOnly = true;
            this.txtbPrevFrame.Size = new System.Drawing.Size(86, 20);
            this.txtbPrevFrame.TabIndex = 17;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnLastFrame);
            this.groupBox4.Controls.Add(this.btnFirstFrame);
            this.groupBox4.Controls.Add(this.numDeltaTime);
            this.groupBox4.Controls.Add(this.txtbLastFrame);
            this.groupBox4.Controls.Add(this.txtbFirstFrame);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Location = new System.Drawing.Point(9, 163);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(245, 108);
            this.groupBox4.TabIndex = 16;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Точка отсчета";
            // 
            // txtbLastFrame
            // 
            this.txtbLastFrame.Location = new System.Drawing.Point(153, 76);
            this.txtbLastFrame.Name = "txtbLastFrame";
            this.txtbLastFrame.ReadOnly = true;
            this.txtbLastFrame.Size = new System.Drawing.Size(86, 20);
            this.txtbLastFrame.TabIndex = 17;
            // 
            // txtbFirstFrame
            // 
            this.txtbFirstFrame.Location = new System.Drawing.Point(153, 47);
            this.txtbFirstFrame.Name = "txtbFirstFrame";
            this.txtbFirstFrame.ReadOnly = true;
            this.txtbFirstFrame.Size = new System.Drawing.Size(86, 20);
            this.txtbFirstFrame.TabIndex = 16;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(126, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Допуск метки времени";
            // 
            // numDeltaTime
            // 
            this.numDeltaTime.Location = new System.Drawing.Point(153, 20);
            this.numDeltaTime.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numDeltaTime.Name = "numDeltaTime";
            this.numDeltaTime.Size = new System.Drawing.Size(86, 20);
            this.numDeltaTime.TabIndex = 18;
            // 
            // btnFirstFrame
            // 
            this.btnFirstFrame.Location = new System.Drawing.Point(11, 45);
            this.btnFirstFrame.Name = "btnFirstFrame";
            this.btnFirstFrame.Size = new System.Drawing.Size(111, 23);
            this.btnFirstFrame.TabIndex = 19;
            this.btnFirstFrame.Text = "Первый кадр";
            this.btnFirstFrame.UseVisualStyleBackColor = true;
            this.btnFirstFrame.Click += new System.EventHandler(this.btnFirstFrame_Click);
            // 
            // btnLastFrame
            // 
            this.btnLastFrame.Location = new System.Drawing.Point(11, 74);
            this.btnLastFrame.Name = "btnLastFrame";
            this.btnLastFrame.Size = new System.Drawing.Size(111, 23);
            this.btnLastFrame.TabIndex = 20;
            this.btnLastFrame.Text = "Последний кадр";
            this.btnLastFrame.UseVisualStyleBackColor = true;
            this.btnLastFrame.Click += new System.EventHandler(this.btnLastFrame_Click);
            // 
            // IndexViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 347);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtbId);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IndexViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Индекс видеозаписи";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeStamp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStreamId)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDeltaTime)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtbId;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtbEndRecord;
        private System.Windows.Forms.TextBox txtbBeginRecord;
        private System.Windows.Forms.ListView lvwStreamInfo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ColumnHeader clhStreamId;
        private System.Windows.Forms.ColumnHeader clhContentType;
        private System.Windows.Forms.ColumnHeader clhResolution;
        private System.Windows.Forms.ColumnHeader clhRotation;
        private System.Windows.Forms.ColumnHeader clhWidth;
        private System.Windows.Forms.ColumnHeader clhHeight;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtbNextFrame;
        private System.Windows.Forms.Button btnPrevFrame;
        private System.Windows.Forms.Button btnNextFrame;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numTimeStamp;
        private System.Windows.Forms.NumericUpDown numStreamId;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtbPrevFrame;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtbLastFrame;
        private System.Windows.Forms.TextBox txtbFirstFrame;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnLastFrame;
        private System.Windows.Forms.Button btnFirstFrame;
        private System.Windows.Forms.NumericUpDown numDeltaTime;
    }
}