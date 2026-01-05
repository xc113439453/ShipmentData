using System.Diagnostics;
using System.Reflection;
using ShipmentData.Interfaces;
using ShipmentData.Models;

namespace ShipmentData
{
    public partial class MainForm : Form
    {
        private List<SummaryFileInfoItem> summaryFileList = new();
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
                var product = GetSelectedRadioButtonValue(gbProduct).ToUpper();
                var station = GetSelectedRadioButtonValue(gbStation).ToUpper();
                IProductFactory factory = (product, station) switch
                {
                    ("DENALIV3", "晶圆") => new DenaliV3WaferFactory<DenaliV3WaferSummaryModel, DenaliV3WaferModel>(),
                    ("DENALIV3", "芯片") => new DenaliV3PICFactory<DenaliV3PICSummaryModel, DenaliV3PICModel>(),
                    _ => throw new InvalidOperationException("未找到对应的产品类型！.")
                };

                var snList = factory.ReadSNListFile(txtSNListFilePath.Text);
                var dataList = factory.ReadShipmentDataFile(filePath);
                var summaryList = factory.ReadSummaryFile(summaryFileList);

                var productList = factory.ProcessSummaryData(dataList, snList, summaryList);
                factory.WriteBackToShipmentDataFile(filePath, productList);
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
            if (summaryFileList.Count == 0)
                throw new Exception("请添加Summary文件的！");
            if (summaryFileList.Any(t => !File.Exists(t.FilePath)))
                throw new Exception("Summary文件列表存在失效文件！");
            if (string.IsNullOrEmpty(txtShipmentDataPath.Text))
                throw new Exception("请选择出货数据表文件的路径！");
            if (!File.Exists(txtShipmentDataPath.Text))
                throw new Exception("选择的出货数据表文件不存在！");
            if (string.IsNullOrEmpty(txtSNListFilePath.Text))
                throw new Exception("请选择SN列表文件的路径！");
            if (!File.Exists(txtSNListFilePath.Text))
                throw new Exception("选择的SN列表文件不存在！");
        }

        private void ToggleControls(bool enabled)
        {
            btnProcess.Enabled = enabled;
            gbProduct.Enabled = enabled;
            btnAdd.Enabled = enabled;
            btnDel.Enabled = enabled;
            txtShipmentDataPath.Enabled = enabled;
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var product = GetSelectedRadioButtonValue(gbProduct).ToUpper();
            var station = GetSelectedRadioButtonValue(gbStation);
            using var openFileDialog = new OpenFileDialog
            {
                Title = "请选择Summary文件",
                Filter = GetFilterByProductTypeAndStation(product, station),
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                lvSummaryFiles.BeginUpdate();
                foreach (string filePath in openFileDialog.FileNames)
                {
                    SummaryFileInfoItem item = new SummaryFileInfoItem(filePath);

                    // 避免重复添加（根据完整路径判断）
                    if (summaryFileList.Exists(f => f.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase)))
                    {
                        MessageBox.Show($"文件已存在：{item.FileName}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        continue;
                    }

                    // 添加到 ListView
                    ListViewItem lvi = new ListViewItem(item.FileName);  // 第一列：文件名
                    lvi.SubItems.Add(item.FilePath);                     // 第二列：完整路径

                    // 可选：将对象绑定到 Tag，方便后续取回完整信息
                    lvi.Tag = item;

                    lvSummaryFiles.Items.Add(lvi);

                    // 添加到列表集合
                    summaryFileList.Add(item);
                }

                lvSummaryFiles.EndUpdate();
            }
        }

        string GetFilterByProductTypeAndStation(string product, string station)
        {
            return (product, station) switch
            {
                ("DENALIV3", "晶圆") => "CSV文件|*.csv",
                ("DENALIV3", "芯片") => "Excel文件|*.xlsx",
                _ => "*.*" // 默认情况，任意 xlsx
            };
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            // 如果没有选中任何行，提示用户
            if (lvSummaryFiles.SelectedItems.Count == 0)
            {
                MessageBox.Show("请选中要删除的行", "提示",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            for (int i = lvSummaryFiles.SelectedItems.Count - 1; i >= 0; i--)
            {
                ListViewItem item = lvSummaryFiles.SelectedItems[i];
                lvSummaryFiles.Items.Remove(item);  // 从 ListView 中移除该行
            }

            foreach (ListViewItem item in lvSummaryFiles.SelectedItems)
            {
                SummaryFileInfoItem fileItem = item.Tag as SummaryFileInfoItem;
                if (fileItem != null)
                {
                    summaryFileList.Remove(fileItem);
                }
            }
        }

        private void Station_CheckedChanged(object sender, EventArgs e)
        {
            lvSummaryFiles.Items.Clear();
            summaryFileList.Clear();
            txtShipmentDataPath.Clear();
            txtSNListFilePath.Clear();
        }
    }
}
