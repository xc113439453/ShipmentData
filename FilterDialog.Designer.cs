namespace ShipmentData
{
    partial class FilterDialog
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
            dgvFilters = new DataGridView();
            colField = new DataGridViewComboBoxColumn();
            colOperator = new DataGridViewComboBoxColumn();
            colValue = new DataGridViewTextBoxColumn();
            colLogic = new DataGridViewComboBoxColumn();
            colDelete = new DataGridViewButtonColumn();
            btnAddRow = new Button();
            btnClear = new Button();
            btnOK = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvFilters).BeginInit();
            SuspendLayout();
            // 
            // dgvFilters
            // 
            dgvFilters.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvFilters.Columns.AddRange(new DataGridViewColumn[] { colField, colOperator, colValue, colLogic, colDelete });
            dgvFilters.Location = new Point(62, 85);
            dgvFilters.Name = "dgvFilters";
            dgvFilters.RowHeadersWidth = 51;
            dgvFilters.Size = new Size(677, 279);
            dgvFilters.TabIndex = 0;
            dgvFilters.CellBeginEdit += dgvFilters_CellBeginEdit;
            dgvFilters.CellClick += dgvFilters_CellClick;
            // 
            // colField
            // 
            colField.HeaderText = "过滤字段";
            colField.MinimumWidth = 6;
            colField.Name = "colField";
            colField.Width = 125;
            // 
            // colOperator
            // 
            colOperator.HeaderText = "操作符";
            colOperator.MinimumWidth = 6;
            colOperator.Name = "colOperator";
            colOperator.Width = 125;
            // 
            // colValue
            // 
            colValue.HeaderText = "值";
            colValue.MinimumWidth = 6;
            colValue.Name = "colValue";
            colValue.Width = 125;
            // 
            // colLogic
            // 
            colLogic.HeaderText = "And/Or";
            colLogic.Items.AddRange(new object[] { "And", "Or" });
            colLogic.MinimumWidth = 6;
            colLogic.Name = "colLogic";
            colLogic.Width = 125;
            // 
            // colDelete
            // 
            colDelete.HeaderText = "删除";
            colDelete.MinimumWidth = 6;
            colDelete.Name = "colDelete";
            colDelete.Width = 125;
            // 
            // btnAddRow
            // 
            btnAddRow.Location = new Point(62, 32);
            btnAddRow.Name = "btnAddRow";
            btnAddRow.Size = new Size(69, 29);
            btnAddRow.TabIndex = 1;
            btnAddRow.Text = "添加行";
            btnAddRow.UseVisualStyleBackColor = true;
            btnAddRow.Click += btnAddRow_Click;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(148, 32);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(69, 29);
            btnClear.TabIndex = 2;
            btnClear.Text = "清空";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // btnOK
            // 
            btnOK.Location = new Point(670, 390);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(69, 29);
            btnOK.TabIndex = 3;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // FilterDialog
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnOK);
            Controls.Add(btnClear);
            Controls.Add(btnAddRow);
            Controls.Add(dgvFilters);
            Name = "FilterDialog";
            Text = "FilterDialog";
            ((System.ComponentModel.ISupportInitialize)dgvFilters).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvFilters;
        private DataGridViewComboBoxColumn Field;
        private DataGridViewComboBoxColumn colField;
        private DataGridViewComboBoxColumn colOperator;
        private DataGridViewTextBoxColumn colValue;
        private DataGridViewComboBoxColumn colLogic;
        private DataGridViewButtonColumn colDelete;
        private Button btnAddRow;
        private Button btnClear;
        private Button btnOK;
    }
}