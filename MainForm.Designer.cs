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
            rbDenali = new RadioButton();
            rbWeser = new RadioButton();
            txtSummaryPath = new TextBox();
            txtShipmentDataPath = new TextBox();
            btnProcess = new Button();
            gbProduct.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(39, 130);
            label1.Name = "label1";
            label1.Size = new Size(122, 20);
            label1.TabIndex = 0;
            label1.Text = "Summary文件：";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(39, 184);
            label2.Name = "label2";
            label2.Size = new Size(129, 20);
            label2.TabIndex = 1;
            label2.Text = "出货数据表文件：";
            // 
            // gbProduct
            // 
            gbProduct.Controls.Add(rbDenali);
            gbProduct.Controls.Add(rbWeser);
            gbProduct.Location = new Point(39, 28);
            gbProduct.Name = "gbProduct";
            gbProduct.Size = new Size(614, 68);
            gbProduct.TabIndex = 2;
            gbProduct.TabStop = false;
            gbProduct.Text = "产品类型";
            // 
            // rbDenali
            // 
            rbDenali.AutoSize = true;
            rbDenali.Location = new Point(151, 29);
            rbDenali.Name = "rbDenali";
            rbDenali.Size = new Size(75, 24);
            rbDenali.TabIndex = 1;
            rbDenali.TabStop = true;
            rbDenali.Text = "Denali";
            rbDenali.UseVisualStyleBackColor = true;
            // 
            // rbWeser
            // 
            rbWeser.AutoSize = true;
            rbWeser.Location = new Point(19, 29);
            rbWeser.Name = "rbWeser";
            rbWeser.Size = new Size(76, 24);
            rbWeser.TabIndex = 0;
            rbWeser.TabStop = true;
            rbWeser.Text = "Weser";
            rbWeser.UseVisualStyleBackColor = true;
            // 
            // txtSummaryPath
            // 
            txtSummaryPath.Location = new Point(181, 127);
            txtSummaryPath.Name = "txtSummaryPath";
            txtSummaryPath.ReadOnly = true;
            txtSummaryPath.Size = new Size(472, 27);
            txtSummaryPath.TabIndex = 3;
            txtSummaryPath.Click += txtFilePath_Click;
            // 
            // txtShipmentDataPath
            // 
            txtShipmentDataPath.Location = new Point(181, 181);
            txtShipmentDataPath.Name = "txtShipmentDataPath";
            txtShipmentDataPath.ReadOnly = true;
            txtShipmentDataPath.Size = new Size(472, 27);
            txtShipmentDataPath.TabIndex = 4;
            txtShipmentDataPath.Click += txtFilePath_Click;
            // 
            // btnProcess
            // 
            btnProcess.Location = new Point(559, 261);
            btnProcess.Name = "btnProcess";
            btnProcess.Size = new Size(94, 29);
            btnProcess.TabIndex = 5;
            btnProcess.Text = "生成";
            btnProcess.UseVisualStyleBackColor = true;
            btnProcess.Click += btnProcess_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(710, 367);
            Controls.Add(btnProcess);
            Controls.Add(txtShipmentDataPath);
            Controls.Add(txtSummaryPath);
            Controls.Add(gbProduct);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "MainForm";
            Text = "出货数据生成";
            gbProduct.ResumeLayout(false);
            gbProduct.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private GroupBox gbProduct;
        private RadioButton rbDenali;
        private RadioButton rbWeser;
        private TextBox txtSummaryPath;
        private TextBox txtShipmentDataPath;
        private Button btnProcess;
    }
}
