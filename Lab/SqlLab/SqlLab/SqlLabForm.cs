using Google.Protobuf.WellKnownTypes;
using RuLib.JsonI18n;
using RuLib.SqlConn;
using System.Data;
using System.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SqlLab
{
    public partial class SqlLabForm : Form
    {
        public IConn Conn { get; set; }
        public ConnOptions Options { get; set; }
        public Schema Schema { get; set; } = new Schema();

        public SqlLabForm(IConn conn, ConnOptions options)
        {
            Conn = conn;
            Options = options;
            InitializeComponent();

        }

        private void SqlLabFormLoad(object sender, EventArgs e)
        {
            var tablesSelect = new Dictionary<int, string>();
            var query = @"
SELECT st.name AS TableName,
       ISNULL(p.value, '') AS TableDescription
FROM sys.tables st
--JOIN INFORMATION_SCHEMA.TABLES ist
--     ON st.name = ist.TABLE_NAME
LEFT JOIN sys.extended_properties p 
       ON p.major_id = st.object_id
       AND p.minor_id = 0
       AND p.name = 'MS_Description'
WHERE st.name != 'sysdiagrams' 
    AND st.name !='dtproperties'
	--AND ist.TABLE_TYPE = 'BASE TABLE'  
ORDER BY st.name";

            var dataTable = Conn.ExecuteQuery(query);
            foreach (DataRow row in dataTable.Rows)
            {
                string tableName = row["TableName"]?.ToString();
                string tableDescription = row["TableDescription"]?.ToString();

                // Create a new Table instance
                Table table = new Table
                {
                    TableName = tableName,
                    TableDescription = tableDescription
                };
                Schema.Tables?.Add(table);
            }
            tablesSelect = Schema.Tables.Select((table, index) => new { index, table.TableName })
                         .ToDictionary(item => item.index, item => item.TableName ?? string.Empty);
            TablesSelect.ValueMember = "Value";
            TablesSelect.DataSource = new BindingSource(tablesSelect, null);
        }

        private void ConnTestBtnClick(object sender, EventArgs e)
        {
            var err = string.Empty;
            var isSuccess = Conn.OpenConn(out err);
            if (isSuccess)
            {
                MessageBox.Show($"連線成功,Err:{err}");
            }
            else
            {
                MessageBox.Show(err);
            }
        }

        private void RandomInsertClick(object sender, EventArgs e)
        {
            var count = Convert.ToInt32(InsertCount.Text);
            var tableName = TablesSelect.SelectedValue.ToString();
            SetColumn(tableName);
            var table = Schema.Tables.FirstOrDefault(x => x.TableName == tableName);
            if (table == null)
            {
                MessageBox.Show("請選擇表格");
                return;
            }

            // 過濾不需要插入的列，例如 Identity 列、具有默認值的列或自動生成類型
            var insertableColumns = table.Columns
                .Where(c => string.IsNullOrEmpty(c.Identity) || c.Identity != "Y") // 跳過自增列
                .Where(c => string.IsNullOrEmpty(c.DefaultValue)) // 跳過有默認值的列
                .Where(c => c.DataType.ToLower() != "timestamp" && c.DataType.ToLower() != "rowversion"); // 跳過自動生成的列

            for (int i = 0; i < count; i++)
            {

                // 隨機生成數據並插入
                var columnNames = string.Join(", ", insertableColumns.Select(c => c.ColumnName));
                var values = string.Join(", ", insertableColumns.Select(c => GenerateRandomValue(c)));

                // 組合 SQL 語句
                var query = $@"
INSERT INTO [{tableName}] ({columnNames}) VALUES ({values});
";

                CurrentQueryTextBox.Text = query;
                // 執行 SQL
                try
                {
                    Conn.ExecuteNonQuery(query);
                }
                catch (Exception ex)
                {
                    ErrorTextLabel.Text = $"插入失敗: {ex.Message}";
                    return;
                }
            }
        }

        private void RandomBulkInsertClick(object sender, EventArgs e)
        {
            var count = Convert.ToInt32(InsertCount.Text);
            var tableName = TablesSelect.SelectedValue.ToString();
            SetColumn(tableName);
            var table = Schema.Tables.FirstOrDefault(x => x.TableName == tableName);
            if (table == null)
            {
                MessageBox.Show("請選擇表格");
                return;
            }

            // 過濾不需要插入的列，例如 Identity 列、具有默認值的列或自動生成類型
            var insertableColumns = table.Columns
                .Where(c => string.IsNullOrEmpty(c.Identity) || c.Identity != "Y") // 跳過自增列
                .Where(c => string.IsNullOrEmpty(c.DefaultValue)) // 跳過有默認值的列
                .Where(c => c.DataType.ToLower() != "timestamp" && c.DataType.ToLower() != "rowversion"); // 跳過自動生成的列

            var query = string.Empty;
            for (int i = 0; i < count; i++)
            {

                // 隨機生成數據並插入
                var columnNames = string.Join(", ", insertableColumns.Select(c => c.ColumnName));
                var values = string.Join(", ", insertableColumns.Select(c => GenerateRandomValue(c)));

                // 組合 SQL 語句 --SET IDENTITY_INSERT {tableName} ON;
                query += $@"
INSERT INTO [{tableName}] ({columnNames}) VALUES ({values});
";
            }
            CurrentQueryTextBox.Text = query;
            // 執行 SQL
            try
            {
                Conn.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                ErrorTextLabel.Text = $"插入失敗: {ex.Message}";
                return;
            }

        }

        private string GenerateRandomValue(Column column)
        {
            if (column.ColumnName == "Address") return $"'Address'";

            // 根據數據類型生成值
            switch (GetBaseDataType(column.DataType.ToLower(), out int length))
            {

                case "uniqueidentifier":
                    return "NEWID()";
                case "int":
                case "bigint":
                case "smallint":
                case "money":
                    return new Random().Next(1, 10000).ToString();
                case "decimal":
                case "float":
                case "real":
                    return new Random().NextDouble().ToString("F2");
                case "varchar":
                case "nvarchar":
                case "nchar":
                case "text":
                case "ntext":
                    length = length > 30 ? 30 : length;
                    return $"'{GenerateRandomString(length)}'";
                case "date":
                case "datetime":
                    return $"'{DateTime.Now.AddDays(-new Random().Next(1, 1000)):yyyy-MM-dd}'";
                case "bit":
                    return new Random().Next(0, 2) == 1 ? "1" : "0";
                case "image":
                    return "CAST('0x' AS VARBINARY(MAX))";
                default:
                    return "0"; // 未支持的類型
            }
        }
        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                                        .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }

        private string GetBaseDataType(string dataType, out int length)
        {
            length = 0; // 初始化長度為 0（默認無長度）

            // 使用正則表達式提取基礎類型和長度
            var match = System.Text.RegularExpressions.Regex.Match(dataType, @"^(\w+)(\(([-]?\d+)\))?$");
            if (match.Success)
            {
                string baseType = match.Groups[1].Value; // 提取基礎類型
                if (match.Groups[3].Success) // 如果匹配到長度
                {
                    length = int.Parse(match.Groups[3].Value); // 設置長度
                }
                return baseType; // 返回基礎類型
            }

            return dataType; // 無法匹配時，直接返回原始數據類型
        }
        private void TableComboBoxSelectChange(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void ManualStopConnClick(object sender, EventArgs e)
        {

        }

        public void SetColumn(string tableName)
        {

            var query = $@"
SELECT DISTINCT sc.column_id AS [Sort]
	,sc.name AS [ColumnName]
	,ic.DATA_TYPE + CASE 
		WHEN ISNULL(ic.CHARACTER_MAXIMUM_LENGTH, '') = ''
			THEN ''
		ELSE '(' + CAST(ic.CHARACTER_MAXIMUM_LENGTH AS VARCHAR) + ')'
		END AS [DataType]
	,ISNULL(ic.COLUMN_DEFAULT, '') AS [DefaultValue]
	,CASE sc.is_identity
		WHEN 1
			THEN 'Y'
		ELSE ''
		END AS [Identity]
	,CASE 
		WHEN ISNULL(ik.TABLE_NAME, '') <> ''
			THEN 'Y'
		ELSE ''
		END AS [PrimaryKey]
	,ISNULL(sep.value, '') AS [ColumnDescription]
	,CASE 
		WHEN sc.is_nullable = 0
			THEN 'Y'
		ELSE ''
		END AS [NotNull]
	,ic.CHARACTER_MAXIMUM_LENGTH AS [Length]
	,ic.NUMERIC_PRECISION AS [Precision]
	,ic.NUMERIC_SCALE AS [Scale]
	,st.name AS [TableName]
FROM sys.tables st
INNER JOIN sys.columns sc ON st.object_id = sc.object_id
    AND st.name = '{tableName}' --check table
INNER JOIN INFORMATION_SCHEMA.COLUMNS ic ON ic.TABLE_NAME = st.name
	AND ic.COLUMN_NAME = sc.name
LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ik ON ik.TABLE_NAME = ic.TABLE_NAME
	AND ik.COLUMN_NAME = ic.COLUMN_NAME
LEFT JOIN sys.extended_properties sep ON st.object_id = sep.major_id
	AND sc.column_id = sep.minor_id
	AND sep.name = 'MS_Description'
LEFT JOIN sys.extended_properties p ON p.major_id = st.object_id
	AND p.minor_id = 0
	AND p.name = 'MS_Description'
ORDER BY st.name
	,sc.column_id
	,sc.name;
";
            var table = Schema.Tables.FirstOrDefault(x => x.TableName == tableName);
            using SqlConnection con = new SqlConnection(Options.ConnectionString);
            using SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            using SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            con.Close();

            foreach (DataRow row in dataTable.Rows)
            {
                Column column = new Column
                {
                    Sort = row["Sort"]?.ToString(),
                    ColumnName = row["ColumnName"]?.ToString(),
                    DataType = row["DataType"]?.ToString(),
                    DefaultValue = row["DefaultValue"]?.ToString(),
                    Identity = row["Identity"]?.ToString(),
                    PrimaryKey = row["PrimaryKey"]?.ToString(),
                    ColumnDescription = row["ColumnDescription"]?.ToString(),
                    NotNull = row["NotNull"]?.ToString(),
                    Length = row["Length"]?.ToString(),
                    Precision = row["Precision"]?.ToString(),
                    Scale = row["Scale"]?.ToString(),
                };
                table.Columns?.Add(column);
            }

        }

        public void SetColumn()
        {
            for (int i = 0; i < Schema.Tables.Count; i++)
            {
                var query = @"
SELECT DISTINCT sc.column_id AS [Sort]
	,sc.name AS [ColumnName]
	,ic.DATA_TYPE + CASE 
		WHEN ISNULL(ic.CHARACTER_MAXIMUM_LENGTH, '') = ''
			THEN ''
		ELSE '(' + CAST(ic.CHARACTER_MAXIMUM_LENGTH AS VARCHAR) + ')'
		END AS [DataType]
	,ISNULL(ic.COLUMN_DEFAULT, '') AS [DefaultValue]
	,CASE sc.is_identity
		WHEN 1
			THEN 'Y'
		ELSE ''
		END AS [Identity]
	,CASE 
		WHEN ISNULL(ik.TABLE_NAME, '') <> ''
			THEN 'Y'
		ELSE ''
		END AS [PrimaryKey]
	,ISNULL(sep.value, '') AS [ColumnDescription]
	,CASE 
		WHEN sc.is_nullable = 0
			THEN 'Y'
		ELSE ''
		END AS [NotNull]
	,ic.CHARACTER_MAXIMUM_LENGTH AS [Length]
	,ic.NUMERIC_PRECISION AS [Precision]
	,ic.NUMERIC_SCALE AS [Scale]
	,st.name AS [TableName]
FROM sys.tables st
INNER JOIN sys.columns sc ON st.object_id = sc.object_id
    AND st.name = @TableName --check table
INNER JOIN INFORMATION_SCHEMA.COLUMNS ic ON ic.TABLE_NAME = st.name
	AND ic.COLUMN_NAME = sc.name
LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ik ON ik.TABLE_NAME = ic.TABLE_NAME
	AND ik.COLUMN_NAME = ic.COLUMN_NAME
LEFT JOIN sys.extended_properties sep ON st.object_id = sep.major_id
	AND sc.column_id = sep.minor_id
	AND sep.name = 'MS_Description'
LEFT JOIN sys.extended_properties p ON p.major_id = st.object_id
	AND p.minor_id = 0
	AND p.name = 'MS_Description'
ORDER BY st.name
	,sc.column_id
	,sc.name;
";

                using SqlConnection con = new SqlConnection(query);
                using SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                cmd.Parameters.AddWithValue("@TableName", Schema.Tables[i].TableName);
                using SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                con.Close();

                foreach (DataRow row in dataTable.Rows)
                {
                    Column column = new Column
                    {
                        Sort = row["Sort"]?.ToString(),
                        ColumnName = row["ColumnName"]?.ToString(),
                        DataType = row["DataType"]?.ToString(),
                        DefaultValue = row["DefaultValue"]?.ToString(),
                        Identity = row["Identity"]?.ToString(),
                        PrimaryKey = row["PrimaryKey"]?.ToString(),
                        ColumnDescription = row["ColumnDescription"]?.ToString(),
                        NotNull = row["NotNull"]?.ToString(),
                        Length = row["Length"]?.ToString(),
                        Precision = row["Precision"]?.ToString(),
                        Scale = row["Scale"]?.ToString(),
                    };
                    Schema.Tables[i].Columns?.Add(column);
                }
            }
        }

        private void ErrorTextLabelCopy(object sender, EventArgs e)
        {
            // 獲取 Label 的文本
            var label = sender as Label;

            if (label != null && !string.IsNullOrEmpty(label.Text))
            {
                // 將文本複製到剪貼板
                Clipboard.SetText(label.Text);
            }
        }

    }
}
