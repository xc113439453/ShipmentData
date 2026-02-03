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
                var productList = factory.ProcessSummaryData(dataList, snList, summaryList);
                if (_savedFilterRules != null && _savedFilterRules.Count > 0)
                {
                    productList = ApplyUserExtraFilters(productList, _savedFilterRules);
                }
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

        private List<T> ApplyUserExtraFilters<T>(List<T> source, List<FilterRule> rules) where T : IProductModel
        {
            if (source == null || source.Count == 0 || rules == null || rules.Count == 0)
                return source ?? new List<T>();

            var query = source.AsQueryable();

            foreach (var rule in rules)
            {
                query = ApplySingleFilterRule(query, rule);
            }

            return query.ToList();
        }

        /// <summary>
        /// 对查询应用单条用户自定义过滤规则
        /// </summary>
        /// <typeparam name="T">必须实现 IProductModel 接口的类型</typeparam>
        /// <param name="query">待过滤的 IQueryable 数据源</param>
        /// <param name="rule">单条过滤规则</param>
        /// <returns>应用过滤后的 IQueryable</returns>
        private IQueryable<T> ApplySingleFilterRule<T>(IQueryable<T> query, FilterRule rule)
            where T : IProductModel
        {
            if (query == null || rule == null)
            {
                return query;
            }

            switch (rule.Field)
            {

                case nameof(BaseProductModel.Heater_Resistance):
                    return ApplyNullableDecimalFilter(query, x => x.Heater_Resistance, rule.Operator, rule.Value);

                case nameof(BaseProductModel.MPD_b_1V):
                    return ApplyNullableDecimalFilter(query, x => x.MPD_b_1V, rule.Operator, rule.Value);

                case nameof(BaseProductModel.MPD_t_1V):
                    return ApplyNullableDecimalFilter(query, x => x.MPD_t_1V, rule.Operator, rule.Value);

                case nameof(BaseProductModel.MPD_Ld_1V):
                    return ApplyNullableDecimalFilter(query, x => x.MPD_Ld_1V, rule.Operator, rule.Value);

                case nameof(BaseProductModel.Ppi):
                    return ApplyNullableDecimalFilter(query, x => x.Ppi, rule.Operator, rule.Value);

                case nameof(BaseProductModel.ER):
                    return ApplyNullableDecimalFilter(query, x => x.ER, rule.Operator, rule.Value);


                // DenaliV3PICModel 专有字段
                case nameof(DenaliV3PICModel.IL_by_PD):
                    return query.OfType<DenaliV3PICModel>().Where(pic => ApplyDecimalCondition(pic.IL_by_PD, rule.Operator, rule.Value)).Cast<T>();

                case nameof(DenaliV3PICModel.Loop):
                    return query.OfType<DenaliV3PICModel>().Where(pic => ApplyDecimalCondition(pic.Loop, rule.Operator, rule.Value)).Cast<T>();

                // DenaliV3WaferModel 专有字段
                case nameof(DenaliV3WaferModel.IL_by_Power):
                    return query.OfType<DenaliV3WaferModel>().Where(pic => ApplyDecimalCondition(pic.IL_by_Power, rule.Operator, rule.Value)).Cast<T>();

                default:
                    // 未知字段 → 不做过滤，直接返回原查询
                    return query;
            }
        }

        // 字符串条件判断
        private bool ApplyStringCondition(string source, string op, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return true;
            value = value.Trim();

            return op switch
            {
                "==" => source == value,
                "!=" => source != value,
                "Contains" => source?.Contains(value) ?? false,
                "StartsWith" => source?.StartsWith(value) ?? false,
                "EndsWith" => source?.EndsWith(value) ?? false,
                _ => true
            };
        }

        // 可空 decimal 过滤（用于公共 decimal? 字段）
        private IQueryable<T> ApplyNullableDecimalFilter<T>(
            IQueryable<T> query,
            Expression<Func<T, decimal?>> selector,
            string op,
            string valueStr)
            where T : IProductModel
        {
            if (!decimal.TryParse(valueStr, out var value)) return query;

            return op switch
            {
                "==" => query.Where(x => selector.Compile()(x) == value),
                "!=" => query.Where(x => selector.Compile()(x) != value),
                ">" => query.Where(x => selector.Compile()(x) > value),
                ">=" => query.Where(x => selector.Compile()(x) >= value),
                "<" => query.Where(x => selector.Compile()(x) < value),
                "<=" => query.Where(x => selector.Compile()(x) <= value),
                _ => query
            };
        }

        // 专有 decimal 字段的条件判断
        private bool ApplyDecimalCondition(decimal? source, string op, string valueStr)
        {
            if (!decimal.TryParse(valueStr, out var value)) return false;
            if (!source.HasValue) return false;

            return op switch
            {
                "==" => source.Value == value,
                "!=" => source.Value != value,
                ">" => source.Value > value,
                ">=" => source.Value >= value,
                "<" => source.Value < value,
                "<=" => source.Value <= value,
                _ => false
            };
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
