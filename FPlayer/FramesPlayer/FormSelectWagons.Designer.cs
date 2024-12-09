namespace FramesPlayer
{
    partial class FormSelectWagons
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gwWagonData = new System.Windows.Forms.DataGridView();
            this.btnSelectWagons = new System.Windows.Forms.Button();
            this.btnCloseForm = new System.Windows.Forms.Button();
            this.btnDeselectAll = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.WagId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SnSost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InvNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InvNumberNL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SelectCheckBox = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gwWagonData)).BeginInit();
            this.SuspendLayout();
            // 
            // gwWagonData
            // 
            this.gwWagonData.AllowUserToAddRows = false;
            this.gwWagonData.AllowUserToDeleteRows = false;
            this.gwWagonData.AllowUserToOrderColumns = true;
            this.gwWagonData.AllowUserToResizeColumns = false;
            this.gwWagonData.AllowUserToResizeRows = false;
            this.gwWagonData.BackgroundColor = System.Drawing.SystemColors.Control;
            this.gwWagonData.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gwWagonData.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.gwWagonData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gwWagonData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.WagId,
            this.ColumnSn,
            this.SnSost,
            this.InvNumber,
            this.InvNumberNL,
            this.SelectCheckBox});
            this.gwWagonData.Dock = System.Windows.Forms.DockStyle.Top;
            this.gwWagonData.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.gwWagonData.Location = new System.Drawing.Point(0, 0);
            this.gwWagonData.Name = "gwWagonData";
            this.gwWagonData.RowHeadersVisible = false;
            this.gwWagonData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gwWagonData.Size = new System.Drawing.Size(382, 402);
            this.gwWagonData.TabIndex = 0;
            // 
            // btnSelectWagons
            // 
            this.btnSelectWagons.Location = new System.Drawing.Point(12, 415);
            this.btnSelectWagons.Name = "btnSelectWagons";
            this.btnSelectWagons.Size = new System.Drawing.Size(75, 23);
            this.btnSelectWagons.TabIndex = 1;
            this.btnSelectWagons.Text = "ОК";
            this.btnSelectWagons.UseVisualStyleBackColor = true;
            this.btnSelectWagons.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCloseForm
            // 
            this.btnCloseForm.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCloseForm.Location = new System.Drawing.Point(93, 415);
            this.btnCloseForm.Name = "btnCloseForm";
            this.btnCloseForm.Size = new System.Drawing.Size(75, 23);
            this.btnCloseForm.TabIndex = 2;
            this.btnCloseForm.Text = "Закрыть";
            this.btnCloseForm.UseVisualStyleBackColor = true;
            this.btnCloseForm.Click += new System.EventHandler(this.CloseFormButton_Click);
            // 
            // btnDeselectAll
            // 
            this.btnDeselectAll.Location = new System.Drawing.Point(287, 415);
            this.btnDeselectAll.Name = "btnDeselectAll";
            this.btnDeselectAll.Size = new System.Drawing.Size(84, 23);
            this.btnDeselectAll.TabIndex = 3;
            this.btnDeselectAll.Text = "Снять все";
            this.btnDeselectAll.UseVisualStyleBackColor = true;
            this.btnDeselectAll.Click += new System.EventHandler(this.DeselectAllButton_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(197, 415);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(84, 23);
            this.btnSelectAll.TabIndex = 4;
            this.btnSelectAll.Text = "Выбрать все";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.SelectAllButton_Click);
            // 
            // WagId
            // 
            this.WagId.HeaderText = "ID";
            this.WagId.Name = "WagId";
            this.WagId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.WagId.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.WagId.Visible = false;
            // 
            // ColumnSn
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColumnSn.DefaultCellStyle = dataGridViewCellStyle1;
            this.ColumnSn.FillWeight = 50F;
            this.ColumnSn.HeaderText = "№ п/п";
            this.ColumnSn.Name = "ColumnSn";
            this.ColumnSn.ReadOnly = true;
            this.ColumnSn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnSn.Width = 50;
            // 
            // SnSost
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.SnSost.DefaultCellStyle = dataGridViewCellStyle2;
            this.SnSost.FillWeight = 60F;
            this.SnSost.HeaderText = "Номер в составе";
            this.SnSost.Name = "SnSost";
            this.SnSost.ReadOnly = true;
            this.SnSost.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SnSost.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.SnSost.Width = 60;
            // 
            // InvNumber
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.InvNumber.DefaultCellStyle = dataGridViewCellStyle3;
            this.InvNumber.HeaderText = "Распознанный инв. номер";
            this.InvNumber.Name = "InvNumber";
            this.InvNumber.ReadOnly = true;
            this.InvNumber.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.InvNumber.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // InvNumberNL
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.InvNumberNL.DefaultCellStyle = dataGridViewCellStyle4;
            this.InvNumberNL.HeaderText = "Инв. номер в натурном листе";
            this.InvNumberNL.Name = "InvNumberNL";
            this.InvNumberNL.ReadOnly = true;
            this.InvNumberNL.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.InvNumberNL.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // SelectCheckBox
            // 
            this.SelectCheckBox.FalseValue = "False";
            this.SelectCheckBox.FillWeight = 50F;
            this.SelectCheckBox.HeaderText = "";
            this.SelectCheckBox.Name = "SelectCheckBox";
            this.SelectCheckBox.TrueValue = "True";
            this.SelectCheckBox.Width = 50;
            // 
            // FormSelectWagons
            // 
            this.AcceptButton = this.btnSelectWagons;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCloseForm;
            this.ClientSize = new System.Drawing.Size(382, 447);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.btnDeselectAll);
            this.Controls.Add(this.btnCloseForm);
            this.Controls.Add(this.btnSelectWagons);
            this.Controls.Add(this.gwWagonData);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSelectWagons";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Выбор вагонов для экспорта видео";
            this.Load += new System.EventHandler(this.SelectWagonsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gwWagonData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gwWagonData;
        private System.Windows.Forms.Button btnSelectWagons;
        private System.Windows.Forms.Button btnCloseForm;
        private System.Windows.Forms.Button btnDeselectAll;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.DataGridViewTextBoxColumn WagId;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SnSost;
        private System.Windows.Forms.DataGridViewTextBoxColumn InvNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn InvNumberNL;
        private System.Windows.Forms.DataGridViewCheckBoxColumn SelectCheckBox;
    }
}