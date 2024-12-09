namespace AlfaPribor.VideoStorage.Demo
{
    partial class FramesWriter
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.chbChangeRecTime = new System.Windows.Forms.CheckBox();
            this.dtpEndRecTime = new System.Windows.Forms.DateTimePicker();
            this.dtpEndRecDate = new System.Windows.Forms.DateTimePicker();
            this.dtpBeginRecTime = new System.Windows.Forms.DateTimePicker();
            this.dtpBeginRecDate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chbManyStreams = new System.Windows.Forms.CheckBox();
            this.txtbId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numFramesCount = new System.Windows.Forms.NumericUpDown();
            this.rbtnWriteMany = new System.Windows.Forms.RadioButton();
            this.rbtnWriteOne = new System.Windows.Forms.RadioButton();
            this.button5 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnEditFrame = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnWriteFrame = new System.Windows.Forms.Button();
            this.rbtnWriteManyStreams = new System.Windows.Forms.RadioButton();
            this.numStreamsCount = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numFramesOnStream = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFramesCount)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStreamsCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFramesOnStream)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numFramesOnStream);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.numStreamsCount);
            this.groupBox1.Controls.Add(this.rbtnWriteManyStreams);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.chbManyStreams);
            this.groupBox1.Controls.Add(this.txtbId);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numFramesCount);
            this.groupBox1.Controls.Add(this.rbtnWriteMany);
            this.groupBox1.Controls.Add(this.rbtnWriteOne);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(242, 319);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Шаг 1";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.chbChangeRecTime);
            this.groupBox4.Controls.Add(this.dtpEndRecTime);
            this.groupBox4.Controls.Add(this.dtpEndRecDate);
            this.groupBox4.Controls.Add(this.dtpBeginRecTime);
            this.groupBox4.Controls.Add(this.dtpBeginRecDate);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Location = new System.Drawing.Point(6, 175);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(230, 137);
            this.groupBox4.TabIndex = 9;
            this.groupBox4.TabStop = false;
            // 
            // chbChangeRecTime
            // 
            this.chbChangeRecTime.AutoSize = true;
            this.chbChangeRecTime.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chbChangeRecTime.Location = new System.Drawing.Point(8, 0);
            this.chbChangeRecTime.Name = "chbChangeRecTime";
            this.chbChangeRecTime.Size = new System.Drawing.Size(112, 17);
            this.chbChangeRecTime.TabIndex = 10;
            this.chbChangeRecTime.Text = "Изменить время";
            this.chbChangeRecTime.UseVisualStyleBackColor = true;
            // 
            // dtpEndRecTime
            // 
            this.dtpEndRecTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpEndRecTime.Location = new System.Drawing.Point(136, 102);
            this.dtpEndRecTime.Name = "dtpEndRecTime";
            this.dtpEndRecTime.ShowUpDown = true;
            this.dtpEndRecTime.Size = new System.Drawing.Size(88, 20);
            this.dtpEndRecTime.TabIndex = 5;
            // 
            // dtpEndRecDate
            // 
            this.dtpEndRecDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpEndRecDate.Location = new System.Drawing.Point(136, 76);
            this.dtpEndRecDate.Name = "dtpEndRecDate";
            this.dtpEndRecDate.Size = new System.Drawing.Size(88, 20);
            this.dtpEndRecDate.TabIndex = 4;
            // 
            // dtpBeginRecTime
            // 
            this.dtpBeginRecTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpBeginRecTime.Location = new System.Drawing.Point(136, 50);
            this.dtpBeginRecTime.Name = "dtpBeginRecTime";
            this.dtpBeginRecTime.ShowUpDown = true;
            this.dtpBeginRecTime.Size = new System.Drawing.Size(88, 20);
            this.dtpBeginRecTime.TabIndex = 3;
            // 
            // dtpBeginRecDate
            // 
            this.dtpBeginRecDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpBeginRecDate.Location = new System.Drawing.Point(136, 24);
            this.dtpBeginRecDate.Name = "dtpBeginRecDate";
            this.dtpBeginRecDate.Size = new System.Drawing.Size(88, 20);
            this.dtpBeginRecDate.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Окончание записи";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Начало записи";
            // 
            // chbManyStreams
            // 
            this.chbManyStreams.AutoSize = true;
            this.chbManyStreams.Location = new System.Drawing.Point(31, 100);
            this.chbManyStreams.Name = "chbManyStreams";
            this.chbManyStreams.Size = new System.Drawing.Size(199, 17);
            this.chbManyStreams.TabIndex = 8;
            this.chbManyStreams.Text = "Каждому видеокадру - свой поток";
            this.chbManyStreams.UseVisualStyleBackColor = true;
            // 
            // txtbId
            // 
            this.txtbId.Location = new System.Drawing.Point(169, 25);
            this.txtbId.Name = "txtbId";
            this.txtbId.Size = new System.Drawing.Size(67, 20);
            this.txtbId.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Идентификатор видеозаписи";
            // 
            // numFramesCount
            // 
            this.numFramesCount.Location = new System.Drawing.Point(169, 74);
            this.numFramesCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numFramesCount.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numFramesCount.Name = "numFramesCount";
            this.numFramesCount.Size = new System.Drawing.Size(67, 20);
            this.numFramesCount.TabIndex = 5;
            this.numFramesCount.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // rbtnWriteMany
            // 
            this.rbtnWriteMany.AutoSize = true;
            this.rbtnWriteMany.Location = new System.Drawing.Point(14, 74);
            this.rbtnWriteMany.Name = "rbtnWriteMany";
            this.rbtnWriteMany.Size = new System.Drawing.Size(120, 17);
            this.rbtnWriteMany.TabIndex = 1;
            this.rbtnWriteMany.TabStop = true;
            this.rbtnWriteMany.Text = "Несколько кадров";
            this.rbtnWriteMany.UseVisualStyleBackColor = true;
            // 
            // rbtnWriteOne
            // 
            this.rbtnWriteOne.AutoSize = true;
            this.rbtnWriteOne.Location = new System.Drawing.Point(14, 51);
            this.rbtnWriteOne.Name = "rbtnWriteOne";
            this.rbtnWriteOne.Size = new System.Drawing.Size(78, 17);
            this.rbtnWriteOne.TabIndex = 0;
            this.rbtnWriteOne.TabStop = true;
            this.rbtnWriteOne.Text = "Один кадр";
            this.rbtnWriteOne.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(173, 460);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(80, 23);
            this.button5.TabIndex = 2;
            this.button5.Text = "Закрыть";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.bttClose_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnEditFrame);
            this.groupBox2.Location = new System.Drawing.Point(12, 337);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(242, 53);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Шаг 2";
            // 
            // btnEditFrame
            // 
            this.btnEditFrame.Location = new System.Drawing.Point(6, 19);
            this.btnEditFrame.Name = "btnEditFrame";
            this.btnEditFrame.Size = new System.Drawing.Size(230, 23);
            this.btnEditFrame.TabIndex = 4;
            this.btnEditFrame.Text = "Редактировать видеокадр";
            this.btnEditFrame.UseVisualStyleBackColor = true;
            this.btnEditFrame.Click += new System.EventHandler(this.btnEditFrame_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnWriteFrame);
            this.groupBox3.Location = new System.Drawing.Point(12, 396);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(242, 58);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Шаг 3";
            // 
            // btnWriteFrame
            // 
            this.btnWriteFrame.Location = new System.Drawing.Point(6, 19);
            this.btnWriteFrame.Name = "btnWriteFrame";
            this.btnWriteFrame.Size = new System.Drawing.Size(230, 23);
            this.btnWriteFrame.TabIndex = 2;
            this.btnWriteFrame.Text = "Поместить в хранилище";
            this.btnWriteFrame.UseVisualStyleBackColor = true;
            this.btnWriteFrame.Click += new System.EventHandler(this.btnWriteFrame_Click);
            // 
            // rbtnWriteManyStreams
            // 
            this.rbtnWriteManyStreams.AutoSize = true;
            this.rbtnWriteManyStreams.Location = new System.Drawing.Point(14, 123);
            this.rbtnWriteManyStreams.Name = "rbtnWriteManyStreams";
            this.rbtnWriteManyStreams.Size = new System.Drawing.Size(125, 17);
            this.rbtnWriteManyStreams.TabIndex = 10;
            this.rbtnWriteManyStreams.TabStop = true;
            this.rbtnWriteManyStreams.Text = "Несколько потоков";
            this.rbtnWriteManyStreams.UseVisualStyleBackColor = true;
            // 
            // numStreamsCount
            // 
            this.numStreamsCount.Location = new System.Drawing.Point(169, 123);
            this.numStreamsCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numStreamsCount.Name = "numStreamsCount";
            this.numStreamsCount.Size = new System.Drawing.Size(67, 20);
            this.numStreamsCount.TabIndex = 11;
            this.numStreamsCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(48, 151);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Кадров в потоке";
            // 
            // numFramesOnStream
            // 
            this.numFramesOnStream.Location = new System.Drawing.Point(169, 149);
            this.numFramesOnStream.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numFramesOnStream.Name = "numFramesOnStream";
            this.numFramesOnStream.Size = new System.Drawing.Size(67, 20);
            this.numFramesOnStream.TabIndex = 13;
            this.numFramesOnStream.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // FramesWriter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(265, 488);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FramesWriter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Запись видеокадров";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFramesCount)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numStreamsCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFramesOnStream)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbtnWriteMany;
        private System.Windows.Forms.RadioButton rbtnWriteOne;
        private System.Windows.Forms.NumericUpDown numFramesCount;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnEditFrame;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnWriteFrame;
        private System.Windows.Forms.TextBox txtbId;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chbManyStreams;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpBeginRecTime;
        private System.Windows.Forms.DateTimePicker dtpBeginRecDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chbChangeRecTime;
        private System.Windows.Forms.DateTimePicker dtpEndRecTime;
        private System.Windows.Forms.DateTimePicker dtpEndRecDate;
        private System.Windows.Forms.RadioButton rbtnWriteManyStreams;
        private System.Windows.Forms.NumericUpDown numStreamsCount;
        private System.Windows.Forms.NumericUpDown numFramesOnStream;
        private System.Windows.Forms.Label label4;
    }
}