using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using ShipmentData.Interfaces;
using ShipmentData.Models;

namespace ShipmentData
{
    public partial class MainForm : Form
    {
        private List<SummaryFileInfoItem> summaryFileList = new();
        // 保存用户最后一次确认的过滤规则
        private List<FilterRule> _savedFilterRules = new List<FilterRule>();
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
                // 第一步：原有 SN list 过滤，得到原始 productList
                var productList = factory.ProcessSummaryData(dataList, snList, summaryList);
                // 如果没有自定义过滤规则，直接跳过
                if (_savedFilterRules == null || _savedFilterRules.Count == 0)
                {
                    // 直接写入
                    factory.WriteBackToShipmentDataFile(filePath, productList);
                    MessageBox.Show($"处理完成！共 {productList.DistinctBy(t => t.SN).Count()} Pcs记录。", "info");
                    return;
                }
                // 第二步：找出所有“至少有一条记录不符合自定义条件”的 SN
                var badSNs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var item in productList)
                {
                    // 假设 IProductModel 有 SN 属性（如果没有，用反射获取）
                    string sn = item.SN;  // 如果没有 SN 属性，请改成反射：propValue = item.GetType().GetProperty("SN")?.GetValue(item)?.ToString()

                    if (string.IsNullOrWhiteSpace(sn))
                        continue;

                    // 检查这条记录是否符合所有规则
                    bool isValid = true;

                    foreach (var rule in _savedFilterRules)
                    {
                        var prop = item.GetType().GetProperty(rule.Field);
                        if (prop == null) continue;

                        var propValue = prop.GetValue(item);

                        if (!IsMatchRule(propValue, rule.Operator, rule.Value))
                        {
                            isValid = false;
                            break;
                        }
                    }

                    if (!isValid)
                    {
                        badSNs.Add(sn);
                    }
                }

                // 第三步：过滤掉所有 bad SN 的记录
                var finalList = productList
                    .Where(item => !badSNs.Contains(item.SN))  // 假设有 SN 属性
                    .ToList();

                // 第四步：重新生成编号
                if (finalList.Any())
                {
                    // 按 SN 分组，并排序（可选：按原始顺序或其他字段）
                    var groupedBySN = finalList
                        .GroupBy(item => item.SN)
                        .ToList();

                    int currentNo = 1;

                    foreach (var group in groupedBySN)
                    {
                        // 同一个 SN 的所有记录都用同一个序号
                        foreach (var item in group)
                        {
                            item.No = currentNo;
                        }
                        currentNo++;
                    }
                }

                // 第五步：写入文件
                factory.WriteBackToShipmentDataFile(filePath, finalList);

                MessageBox.Show($"处理完成！\n原始记录：{productList.DistinctBy(t => t.SN).Count()} Pcs      过滤后：{finalList.DistinctBy(t => t.SN).Count()} Pcs", "info");
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

        /// <summary>
        /// 判断单个属性值是否符合过滤规则（完全避免任何可空类型模式匹配）
        /// </summary>
        /// <param name="propValue">反射得到的属性值（可能为 null）</param>
        /// <param name="op">操作符</param>
        /// <param name="ruleValue">规则值（字符串）</param>
        /// <returns>是否匹配</returns>
        private bool IsMatchRule(object propValue, string op, string ruleValue)
        {
            if (propValue == null)
            {
                return op == "!=";  // null != 值 → true，其余 false（可改成 true）
            }

            string ruleVal = ruleValue?.Trim() ?? "";

            // 字符串
            if (propValue is string strVal)
            {
                return op switch
                {
                    "==" => strVal == ruleVal,
                    "!=" => strVal != ruleVal,
                    "Contains" => strVal.Contains(ruleVal, StringComparison.OrdinalIgnoreCase),
                    "StartsWith" => strVal.StartsWith(ruleVal, StringComparison.OrdinalIgnoreCase),
                    "EndsWith" => strVal.EndsWith(ruleVal, StringComparison.OrdinalIgnoreCase),
                    _ => true
                };
            }

            // decimal 处理
            decimal? decNullable = propValue as decimal?;
            if (decNullable.HasValue)
            {
                if (!decimal.TryParse(ruleVal, out var ruleDec)) return false;
                var actualDec = decNullable.Value;
                return op switch
                {
                    "==" => actualDec == ruleDec,
                    "!=" => actualDec != ruleDec,
                    ">" => actualDec > ruleDec,
                    ">=" => actualDec >= ruleDec,
                    "<" => actualDec < ruleDec,
                    "<=" => actualDec <= ruleDec,
                    _ => true
                };
            }
            else if (propValue is decimal decVal)
            {
                if (!decimal.TryParse(ruleVal, out var ruleDec)) return false;
                return op switch
                {
                    "==" => decVal == ruleDec,
                    "!=" => decVal != ruleDec,
                    ">" => decVal > ruleDec,
                    ">=" => decVal >= ruleDec,
                    "<" => decVal < ruleDec,
                    "<=" => decVal <= ruleDec,
                    _ => true
                };
            }

            // int 处理
            int? intNullable = propValue as int?;
            if (intNullable.HasValue)
            {
                if (!int.TryParse(ruleVal, out var ruleInt)) return false;
                var actualInt = intNullable.Value;
                return op switch
                {
                    "==" => actualInt == ruleInt,
                    "!=" => actualInt != ruleInt,
                    ">" => actualInt > ruleInt,
                    ">=" => actualInt >= ruleInt,
                    "<" => actualInt < ruleInt,
                    "<=" => actualInt <= ruleInt,
                    _ => true
                };
            }
            else if (propValue is int intVal)
            {
                if (!int.TryParse(ruleVal, out var ruleInt)) return false;
                return op switch
                {
                    "==" => intVal == ruleInt,
                    "!=" => intVal != ruleInt,
                    ">" => intVal > ruleInt,
                    ">=" => intVal >= ruleInt,
                    "<" => intVal < ruleInt,
                    "<=" => intVal <= ruleInt,
                    _ => true
                };
            }

            // bool 处理
            bool? boolNullable = propValue as bool?;
            if (boolNullable.HasValue)
            {
                if (!bool.TryParse(ruleVal, out var ruleBool)) return false;
                var actualBool = boolNullable.Value;
                return op switch
                {
                    "==" => actualBool == ruleBool,
                    "!=" => actualBool != ruleBool,
                    _ => true
                };
            }
            else if (propValue is bool boolVal)
            {
                if (!bool.TryParse(ruleVal, out var ruleBool)) return false;
                return op switch
                {
                    "==" => boolVal == ruleBool,
                    "!=" => boolVal != ruleBool,
                    _ => true
                };
            }

            // DateTime 处理
            DateTime? dtNullable = propValue as DateTime?;
            if (dtNullable.HasValue)
            {
                if (!DateTime.TryParse(ruleVal, out var ruleDt)) return false;
                var actualDt = dtNullable.Value;
                return op switch
                {
                    "==" => actualDt == ruleDt,
                    "!=" => actualDt != ruleDt,
                    ">" => actualDt > ruleDt,
                    ">=" => actualDt >= ruleDt,
                    "<" => actualDt < ruleDt,
                    "<=" => actualDt <= ruleDt,
                    _ => true
                };
            }
            else if (propValue is DateTime dtVal)
            {
                if (!DateTime.TryParse(ruleVal, out var ruleDt)) return false;
                return op switch
                {
                    "==" => dtVal == ruleDt,
                    "!=" => dtVal != ruleDt,
                    ">" => dtVal > ruleDt,
                    ">=" => dtVal >= ruleDt,
                    "<" => dtVal < ruleDt,
                    "<=" => dtVal <= ruleDt,
                    _ => true
                };
            }

            // 其他类型默认通过
            return true;
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
            foreach (ListViewItem item in lvSummaryFiles.SelectedItems)
            {
                SummaryFileInfoItem fileItem = item.Tag as SummaryFileInfoItem;
                if (fileItem != null)
                {
                    summaryFileList.Remove(fileItem);
                    lvSummaryFiles.Items.Remove(item);
                }
            }
        }

        private void Station_CheckedChanged(object sender, EventArgs e)
        {
            lvSummaryFiles.Items.Clear();
            summaryFileList.Clear();
            txtShipmentDataPath.Clear();
            txtSNListFilePath.Clear();
            _savedFilterRules.Clear();
            lblFilterStatus.Visible = false;
        }

        private void btnAddFilter_Click(object sender, EventArgs e)
        {
            string currentProduct = GetSelectedRadioButtonValue(gbProduct).ToUpper();  // 假设你有产品选择下拉框
            string station = GetSelectedRadioButtonValue(gbStation).ToUpper();
            using var filterDlg = new FilterDialog(currentProduct, station);
            filterDlg.SetInitialRules(_savedFilterRules);
            if (filterDlg.ShowDialog() == DialogResult.OK)
            {
                _savedFilterRules = filterDlg.FilterRules.ToList();
                // 把 rules 存到成员变量，或直接传给 factory.ProcessSummaryData(..., rules)
                lblFilterStatus.Text = $"过滤条件：{_savedFilterRules.Count} 条";
                lblFilterStatus.Visible = _savedFilterRules.Count > 0;
            }
        }
    }
}
