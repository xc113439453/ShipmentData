using System.ComponentModel;
using System.Reflection;
using ShipmentData.Attributes;
using ShipmentData.Models;

namespace ShipmentData
{
    public partial class FilterDialog : Form
    {
        private readonly string _productType;  // e.g. "PIC" 或 "Wafer"
        private readonly string _station;

        // 这里声明成员变量（私有，只在窗体内部用）
        private List<FieldItem> _allFields = new List<FieldItem>();

        public List<FilterRule> FilterRules { get; private set; } = new List<FilterRule>();

        public FilterDialog(string productType, string station)
        {
            InitializeComponent();
            _productType = productType;
            _station = station;
            LoadFilterableFields();
            SetupDataGridView();
        }

        private bool IsFilterableType(Type t)
        {
            return t == typeof(string) ||
                   t == typeof(int) || t == typeof(int?) ||
                   t == typeof(decimal) || t == typeof(decimal?) ||
                   t == typeof(DateTime) || t == typeof(DateTime?) ||
                   t == typeof(bool) || t == typeof(bool?);
        }

        private void LoadFilterableFields()
        {

            _allFields.Clear();  // 清空旧的（如果多次调用）
            Type baseType = typeof(BaseProductModel);
            // 先获取基础字段（BaseProductModel 的所有可筛选属性）
            var baseProps = typeof(BaseProductModel)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(
                    p => p.CanRead &&
                    IsFilterableType(p.PropertyType) &&
                    !p.GetCustomAttributes(typeof(ExcludeFromFilterAttribute), false).Any())
                .ToList();

            // 添加专有字段
            Type specificModelType = (_productType, _station) switch
            {
                ("DENALIV3", "芯片") => typeof(DenaliV3PICModel),
                ("DENALIV3", "晶圆") => typeof(DenaliV3WaferModel),
                _ => typeof(BaseProductModel)
            };

            // 获取该模型的特有属性（排除 BaseProductModel 已有的）
            var specificProps = specificModelType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(
                    p => p.DeclaringType != typeof(BaseProductModel) &&
                    IsFilterableType(p.PropertyType) &&
                    !p.GetCustomAttributes(typeof(ExcludeFromFilterAttribute), false).Any())
                .ToList();

            // 合并基础 + 特有
            var allProps = baseProps.Concat(specificProps).ToList();

            // 构建显示列表：用 DisplayName，如果没有则用属性名
            var fieldList = allProps.Select(p =>
            {
                var displayAttr = p.GetCustomAttribute<DisplayNameAttribute>();
                return new FieldItem
                {
                    PropertyName = p.Name,
                    DisplayName = displayAttr?.DisplayName ?? p.Name,
                    PropertyType = p.PropertyType
                };
            })
            .OrderBy(f => f.DisplayName)
            .ToList();

            // 排序（可选，按 DisplayName）
            _allFields = fieldList.OrderBy(f => f.DisplayName).ToList();

            var cmb = (DataGridViewComboBoxColumn)dgvFilters.Columns["colField"];
            cmb.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
            cmb.Items.Clear();

            foreach (var f in fieldList)
            {
                cmb.Items.Add(f);  // 对象添加到 ComboBox
            }

            // 自定义显示
            cmb.DisplayMember = "DisplayName";
            cmb.ValueMember = "PropertyName";
        }

        // 辅助类（放在 FilterDialog 或单独文件）
        public class FieldItem
        {
            public string PropertyName { get; set; }
            public string DisplayName { get; set; }
            public Type PropertyType { get; set; }

            public override string ToString() => DisplayName;  // ComboBox 默认显示
        }
        private void SetupDataGridView()
        {
            // 假设已在 Designer 中添加列：colField, colOperator, colValue, colLogic, colDelete
            var opCol = dgvFilters.Columns["colOperator"] as DataGridViewComboBoxColumn;
            opCol.Items.AddRange(new[] { "==", "!=", ">", ">=", "<", "<=" });

            var logicCol = dgvFilters.Columns["colLogic"] as DataGridViewComboBoxColumn;
            logicCol.Items.AddRange(new[] { "AND", "OR" });

            dgvFilters.AllowUserToAddRows = false; // 我们手动加
            AddEmptyRow(); // 初始一行
        }

        // 新增：接收主窗口传来的初始规则
        public void SetInitialRules(IEnumerable<FilterRule> initialRules)
        {
            dgvFilters.Rows.Clear();

            if (initialRules == null || !initialRules.Any())
            {
                AddEmptyRow();
                return;
            }

            var cmb = dgvFilters.Columns["colField"] as DataGridViewComboBoxColumn;
            if (cmb == null) return;

            foreach (var rule in initialRules)
            {
                int rowIndex = dgvFilters.Rows.Add();
                var row = dgvFilters.Rows[rowIndex];

                // 查找当前 ComboBox 是否有这个 DisplayName
                var fieldItem = _allFields.FirstOrDefault(f => f.PropertyName == rule.Field);
                string displayToSet = fieldItem?.PropertyName;

                if (fieldItem == null)
                {
                    // 当前产品/站点环境下没有这个字段，旧规则已失效
                    dgvFilters.Rows.RemoveAt(rowIndex);
                    continue;
                }

                row.Cells["colField"].Value = displayToSet;   // 现在安全赋值
                row.Cells["colOperator"].Value = rule.Operator ?? "==";
                row.Cells["colValue"].Value = rule.Value ?? "";
                row.Cells["colLogic"].Value = rule.Logic ?? "AND";
                row.Cells["colDelete"].Value = "删除";
            }

            // 如果加载后一行都没有，加一个空行
            if (dgvFilters.Rows.Count == 0)
            {
                AddEmptyRow();
            }
        }

        private void dgvFilters_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dgvFilters.Columns[e.ColumnIndex].Name != "colValue") return;
            var row = dgvFilters.Rows[e.RowIndex];

            // 获取当前行选中的字段显示名称（字符串）
            string selectedDisplayName = row.Cells[0].Value?.ToString()?.Trim();

            // 如果没选字段，直接返回
            if (string.IsNullOrWhiteSpace(selectedDisplayName))
                return;

            // 通过显示名称反查 FieldItem
            var fieldObj = _allFields.FirstOrDefault(f => f.DisplayName == selectedDisplayName);

            //var fieldObj = row.Cells[0].Value as FieldItem;
            if (fieldObj == null) return;

            // 根据 PropertyType 切换编辑控件（高级方式需自定义 CellTemplate）
            // 简单方式：提示或限制输入
            if (fieldObj.PropertyType == typeof(decimal) || fieldObj.PropertyType == typeof(decimal?))
            {
                // 可改为 NumericUpDownColumn，但需预先设置列类型
                MessageBox.Show("请输入数字（支持小数）");
            }
        }

        private void AddEmptyRow()
        {
            dgvFilters.Rows.Add("", "==", "", "AND", "删除");
        }

        private void btnAddRow_Click(object sender, EventArgs e)
        {
            AddEmptyRow();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dgvFilters.Rows.Clear();
            AddEmptyRow();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            FilterRules.Clear();

            foreach (DataGridViewRow row in dgvFilters.Rows)
            {
                if (row.IsNewRow) continue;

                string field = row.Cells["colField"].Value?.ToString()?.Trim();
                string op = row.Cells["colOperator"].Value?.ToString()?.Trim();
                string val = row.Cells["colValue"].Value?.ToString()?.Trim();
                string logic = row.Cells["colLogic"].Value?.ToString() ?? "AND";

                if (!string.IsNullOrWhiteSpace(field) && !string.IsNullOrWhiteSpace(op) && !string.IsNullOrWhiteSpace(val))
                {
                    FilterRules.Add(new FilterRule
                    {
                        Field = field,
                        Operator = op,
                        Value = val,
                        Logic = logic
                    });
                }
            }

            if (FilterRules.Count == 0)
            {
                MessageBox.Show("没有有效的过滤条件，将不过滤。");
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void dgvFilters_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvFilters.Columns[e.ColumnIndex].Name == "colDelete")
            {
                if (MessageBox.Show("删除此条件？", "确认", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    dgvFilters.Rows.RemoveAt(e.RowIndex);
                    if (dgvFilters.Rows.Count == 0) AddEmptyRow();
                }
            }
        }
    }
}

public class FilterRule
{
    public string Field { get; set; }
    public string Operator { get; set; }
    public string Value { get; set; }
    public string Logic { get; set; } = "AND";  // AND / OR
}