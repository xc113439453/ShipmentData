namespace ShipmentData
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            gbProduct = new GroupBox();
            rbDenaliV3 = new RadioButton();
            txtShipmentDataPath = new TextBox();
            btnProcess = new Button();
            lvSummaryFiles = new ListView();
            colFileName = new ColumnHeader();
            colFilePath = new ColumnHeader();
            label3 = new Label();
            txtSNListFilePath = new TextBox();
            btnAdd = new Button();
            btnDel = new Button();
            gbStation = new GroupBox();
            rbPIC = new RadioButton();
            rbWafer = new RadioButton();
            btnAddFilter = new Button();
            lblFilterStatus = new Label();
            gbProduct.SuspendLayout();
            gbStation.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(39, 217);
            label1.Name = "label1";
            label1.Size = new Size(152, 20);
            label1.TabIndex = 0;
            label1.Text = "Summary文件列表：";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(39, 416);
            label2.Name = "label2";
            label2.Size = new Size(114, 20);
            label2.TabIndex = 1;
            label2.Text = "出货数据模板：";
            // 
            // gbProduct
            // 
            gbProduct.Controls.Add(rbDenaliV3);
            gbProduct.Location = new Point(39, 28);
            gbProduct.Name = "gbProduct";
            gbProduct.Size = new Size(614, 68);
            gbProduct.TabIndex = 2;
            gbProduct.TabStop = false;
            gbProduct.Text = "产品类型";
            // 
            // rbDenaliV3
            // 
            rbDenaliV3.AutoSize = true;
            rbDenaliV3.Location = new Point(19, 27);
            rbDenaliV3.Name = "rbDenaliV3";
            rbDenaliV3.Size = new Size(94, 24);
            rbDenaliV3.TabIndex = 2;
            rbDenaliV3.TabStop = true;
            rbDenaliV3.Text = "DenaliV3";
            rbDenaliV3.UseVisualStyleBackColor = true;
            // 
            // txtShipmentDataPath
            // 
            txtShipmentDataPath.Location = new Point(181, 413);
            txtShipmentDataPath.Name = "txtShipmentDataPath";
            txtShipmentDataPath.ReadOnly = true;
            txtShipmentDataPath.Size = new Size(472, 27);
            txtShipmentDataPath.TabIndex = 4;
            txtShipmentDataPath.Click += txtFilePath_Click;
            // 
            // btnProcess
            // 
            btnProcess.Location = new Point(586, 529);
            btnProcess.Name = "btnProcess";
            btnProcess.Size = new Size(67, 29);
            btnProcess.TabIndex = 5;
            btnProcess.Text = "生成";
            btnProcess.UseVisualStyleBackColor = true;
            btnProcess.Click += btnProcess_Click;
            // 
            // lvSummaryFiles
            // 
            lvSummaryFiles.Columns.AddRange(new ColumnHeader[] { colFileName, colFilePath });
            lvSummaryFiles.FullRowSelect = true;
            lvSummaryFiles.Location = new Point(39, 262);
            lvSummaryFiles.MultiSelect = false;
            lvSummaryFiles.Name = "lvSummaryFiles";
            lvSummaryFiles.Size = new Size(614, 121);
            lvSummaryFiles.TabIndex = 6;
            lvSummaryFiles.UseCompatibleStateImageBehavior = false;
            lvSummaryFiles.View = View.Details;
            // 
            // colFileName
            // 
            colFileName.Text = "文件名";
            colFileName.Width = 200;
            // 
            // colFilePath
            // 
            colFilePath.Text = "文件路径";
            colFilePath.Width = 400;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(39, 469);
            label3.Name = "label3";
            label3.Size = new Size(100, 20);
            label3.TabIndex = 7;
            label3.Text = "SNList文件：";
            // 
            // txtSNListFilePath
            // 
            txtSNListFilePath.Location = new Point(181, 466);
            txtSNListFilePath.Name = "txtSNListFilePath";
            txtSNListFilePath.ReadOnly = true;
            txtSNListFilePath.Size = new Size(472, 27);
            txtSNListFilePath.TabIndex = 8;
            txtSNListFilePath.Click += txtFilePath_Click;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(500, 212);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(67, 29);
            btnAdd.TabIndex = 9;
            btnAdd.Text = "添加";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnDel
            // 
            btnDel.Location = new Point(586, 214);
            btnDel.Name = "btnDel";
            btnDel.Size = new Size(67, 29);
            btnDel.TabIndex = 10;
            btnDel.Text = "删除";
            btnDel.UseVisualStyleBackColor = true;
            btnDel.Click += btnDel_Click;
            // 
            // gbStation
            // 
            gbStation.Controls.Add(rbPIC);
            gbStation.Controls.Add(rbWafer);
            gbStation.Location = new Point(39, 111);
            gbStation.Name = "gbStation";
            gbStation.Size = new Size(614, 68);
            gbStation.TabIndex = 3;
            gbStation.TabStop = false;
            gbStation.Text = "测试台类型";
            // 
            // rbPIC
            // 
            rbPIC.AutoSize = true;
            rbPIC.Location = new Point(113, 27);
            rbPIC.Name = "rbPIC";
            rbPIC.Size = new Size(60, 24);
            rbPIC.TabIndex = 3;
            rbPIC.Text = "芯片";
            rbPIC.UseVisualStyleBackColor = true;
            rbPIC.CheckedChanged += Station_CheckedChanged;
            // 
            // rbWafer
            // 
            rbWafer.AutoSize = true;
            rbWafer.Checked = true;
            rbWafer.Location = new Point(19, 27);
            rbWafer.Name = "rbWafer";
            rbWafer.Size = new Size(60, 24);
            rbWafer.TabIndex = 2;
            rbWafer.TabStop = true;
            rbWafer.Text = "晶圆";
            rbWafer.UseVisualStyleBackColor = true;
            rbWafer.CheckedChanged += Station_CheckedChanged;
            // 
            // btnAddFilter
            // 
            btnAddFilter.Location = new Point(39, 529);
            btnAddFilter.Name = "btnAddFilter";
            btnAddFilter.Size = new Size(118, 29);
            btnAddFilter.TabIndex = 11;
            btnAddFilter.Text = "添加过滤条件";
            btnAddFilter.UseVisualStyleBackColor = true;
            btnAddFilter.Click += btnAddFilter_Click;
            // 
            // lblFilterStatus
            // 
            lblFilterStatus.AutoSize = true;
            lblFilterStatus.Location = new Point(181, 533);
            lblFilterStatus.Name = "lblFilterStatus";
            lblFilterStatus.Size = new Size(0, 20);
            lblFilterStatus.TabIndex = 12;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(710, 594);
            Controls.Add(lblFilterStatus);
            Controls.Add(btnAddFilter);
            Controls.Add(gbStation);
            Controls.Add(btnDel);
            Controls.Add(btnAdd);
            Controls.Add(txtSNListFilePath);
            Controls.Add(label3);
            Controls.Add(lvSummaryFiles);
            Controls.Add(btnProcess);
            Controls.Add(txtShipmentDataPath);
            Controls.Add(gbProduct);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "MainForm";
            Text = "出货数据生成";
            gbProduct.ResumeLayout(false);
            gbProduct.PerformLayout();
            gbStation.ResumeLayout(false);
            gbStation.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private GroupBox gbProduct;
        private TextBox txtShipmentDataPath;
        private Button btnProcess;
        private RadioButton rbDenaliV3;
        private ListView lvSummaryFiles;
        private Label label3;
        private TextBox txtSNListFilePath;
        private ColumnHeader colFileName;
        private ColumnHeader colFilePath;
        private Button btnAdd;
        private Button btnDel;
        private GroupBox gbStation;
        private RadioButton rbWafer;
        private RadioButton rbPIC;
        private Button btnAddFilter;
        private Label lblFilterStatus;
    }
}
