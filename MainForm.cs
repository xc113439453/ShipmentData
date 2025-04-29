using System.Diagnostics;
using System.Reflection;
using ShipmentData.Interfaces;
using ShipmentData.Models;

namespace ShipmentData
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            var fileVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
            this.Text = $"{this.Text} V{fileVersion}";
        }

        private void txtFilePath_Click(object sender, EventArgs e)
        {
            var product = GetSelectedRadioButtonValue(gbProduct).ToUpper();
            using var openFileDialog = new OpenFileDialog
            {
                Filter = product == "DENALIV3" && (sender as TextBox).Name == "txtSummaryPath" ? "CSV (.csv)|*.csv" : "xlsx (.xlsx)|*.xlsx"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ((TextBox)sender).Text = openFileDialog.FileName;
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            try
            {
                ValidateRequired();
                ToggleControls(false);
                string filePath = txtShipmentDataPath.Text;
                string summaryFilePath = txtSummaryPath.Text;
                var product = GetSelectedRadioButtonValue(gbProduct).ToUpper();

                IProductFactory factory = product switch
                {
                    "WESER" => new WeserFactory(),
                    "DENALI" => new DenaliFactory(),
                    "DENALIV3" => new DenaliV3Factory(),
                    _ => throw new InvalidOperationException("未找到对应的产品类型！.")
                };

                var dataList = factory.ReadShipmentDataFile(filePath);
                var summaryList = factory.ReadSummaryFile(summaryFilePath);
                var productList = factory.ProcessSummaryData(dataList, summaryList);
                factory.WriteBackToShipmentDataFile(filePath, productList);

                MessageBox.Show("处理完成，即将打开文件");
                //ClearText();
                OpenFile(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ToggleControls(true);
            }
        }

        private void ValidateRequired()
        {
            if (string.IsNullOrEmpty(txtSummaryPath.Text))
                throw new Exception("请选择Summary文件的路径！");
            if (!File.Exists(txtSummaryPath.Text))
                throw new Exception("选择的Summary文件不存在！");
            if (string.IsNullOrEmpty(txtShipmentDataPath.Text))
                throw new Exception("请选择出货数据表文件的路径！");
            if (!File.Exists(txtShipmentDataPath.Text))
                throw new Exception("选择的出货数据表文件不存在！");
        }

        private void ToggleControls(bool enabled)
        {
            btnProcess.Enabled = enabled;
            gbProduct.Enabled = enabled;
            txtSummaryPath.Enabled = enabled;
            txtShipmentDataPath.Enabled = enabled;
        }

        private void OpenFile(string filePath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true, // 使用系统外壳执行，调用默认程序
                Verb = "open" // 指定操作（打开文件）
            };
            Process.Start(startInfo);
        }

        private string GetSelectedRadioButtonValue(GroupBox groupBox)
        {
            // 遍历 GroupBox 中的所有控件
            foreach (Control control in groupBox.Controls)
            {
                // 检查控件是否为 RadioButton 且被选中
                if (control is RadioButton radioButton && radioButton.Checked)
                {
                    return radioButton.Text;
                }
            }
            // 如果没有选中项，返回 null
            throw new Exception("未找到选中项");
        }

        private void ClearText()
        {
            this.txtShipmentDataPath.Clear();
            this.txtSummaryPath.Clear();
        }
    }
}
