using System.Diagnostics;
using ShipmentData.Interfaces;
using ShipmentData.Models;
using ShipmentData.Utils;

namespace ShipmentData
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void txtFilePath_Click(object sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "xlsx (.xlsx)|*.xlsx"
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
                if (product == "WESER")
                {
                    var dataList = ExcelUtil.ReadShipmentDataFile<WeserModel>(filePath);
                    var summaryList = ExcelUtil.ReadSummaryFile<WeserSummaryModel>(summaryFilePath);
                    foreach (var data in dataList)
                    {
                        GetSummaryData(data, summaryList);
                    }
                    ExcelUtil.WriteBackToShipmentDataFile(filePath, dataList);
                }
                else if (product == "DENALI")
                {
                    var dataList = ExcelUtil.ReadShipmentDataFile<DenaliModel>(filePath);
                    var summaryList = ExcelUtil.ReadSummaryFile<DenaliSummaryModel>(summaryFilePath);
                    foreach (var data in dataList)
                    {
                        GetSummaryData(data, summaryList);
                    }
                    ExcelUtil.WriteBackToShipmentDataFile(filePath, dataList);
                }

                MessageBox.Show("处理完成，即将打开文件");
                ClearText();
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

        private void GetSummaryData<TProduct, TSummary>(TProduct product, List<TSummary> list)
                where TProduct : IProduct
                where TSummary : ISummary
        {
            var summaryRecord = list.Where(summary => summary.SN == product.SN && $"CH{summary.Channel.Replace("CH", "")}" == product.Channel).FirstOrDefault();
            if (summaryRecord == null)
            {
                throw new Exception($"未找到SN：{product.SN}, Channel:{product.Channel}的Summary数据!");
            }
            var sourceProperties = typeof(TSummary).GetProperties().ToDictionary(prop => prop.Name, prop => prop);
            var targetProperties = typeof(TProduct).GetProperties();
            foreach (var targetProp in targetProperties)
            {
                // 检查 T2 是否有同名属性
                if (sourceProperties.TryGetValue(targetProp.Name, out var sourceProp))
                {
                    // 确保源属性可读，目标属性可写，且类型兼容
                    if (sourceProp.CanRead && targetProp.CanWrite && sourceProp.PropertyType == targetProp.PropertyType)
                    {
                        // 获取源属性的值
                        var value = sourceProp.GetValue(summaryRecord);
                        // 赋值给目标属性
                        targetProp.SetValue(product, value);
                    }
                }
            }
            //product.Ppi = summaryRecord.Ppi;
            //product.ER = summaryRecord.ER;
            //product.IL_by_PD = summaryRecord.IL_by_PD;
            //product.Heater_Resistance = summaryRecord.Heater_Resistance;
            //product.MPD_b_1V = summaryRecord.MPD_b_1V;
            //product.MPD_t_1V = summaryRecord.MPD_t_1V;
            //product.MPD_Ld_1V = summaryRecord.MPD_Ld_1V;
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
