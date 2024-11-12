using Microsoft.EntityFrameworkCore;
using RuLib.SqlConn;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace RuLib.Util
{
    public static class DbUtil
    {
        /// <summary>
        ///  基本的BulkInsert 
        /// </summary>
        /// <typeparam name="BaseEntity"> The type of the entity</typeparam>
        /// <param name="listOfBaseEntities"> Insert 資料來源</param>
        /// <param name="context">DB連線</param>
        /// <param name="timeout">逾時時間</param>
        public static void BulkInsertEntity<BaseEntity>(List<BaseEntity> listOfBaseEntities, DbContext context, int timeout = 300)
        {
            string tableName = EFCoreHelper.GetTableName(context, typeof(BaseEntity));

            // 檢核 listOfBaseEntities 內容
            // 檢核錯誤會被放入集合，只能驗證不可null的值
            ValidationContext validationContext = new ValidationContext(listOfBaseEntities, null, null);
            List<ValidationResult> validationErrors = new List<ValidationResult>();
            bool validateSuccess = Validator.TryValidateObject(listOfBaseEntities, validationContext, validationErrors, true);

            if (validateSuccess)
            {
                var connection = context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                {
                    //打開Connection連接
                    connection.Open();
                }

                //調用BulkInsert方法,將entitys集合數據批量插入到資料庫的tolocation表中
                BulkInsert((SqlConnection)connection, tableName, listOfBaseEntities, timeout);

                if (connection.State != ConnectionState.Closed)
                {
                    //關閉Connection連接
                    connection.Close();
                }
            }
            else
            {
                throw new Exception(string.Join(",", validationErrors.Select(r => $"ErrorMessage{r.ErrorMessage}")));
            }
        }

        /// <summary>
        /// Bulks the insert by temporary table.
        /// </summary>
        /// <typeparam name="TempEntity">The type of the emp entity.</typeparam>
        /// <typeparam name="TargetEntity">The type of the arget entity.</typeparam>
        /// <typeparam name="BaseEntity">The type of the ase entity.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="context">The database.</param>
        public static void BulkInsertByTempTable<TempEntity, TargetEntity, BaseEntity>(List<TargetEntity> source, DbContext context)
            where TempEntity : BaseEntity
            where TargetEntity : BaseEntity
            where BaseEntity : class
        {
            string tempTableName = EFCoreHelper.GetTableName(context, typeof(TempEntity));
            string targetTableName = EFCoreHelper.GetTableName(context, typeof(TargetEntity));

            using (var conndb = context)
            {
                //打開Connection連接
                if (conndb.Database.GetDbConnection().State != ConnectionState.Open)
                    conndb.Database.OpenConnection();

                //刪除暫存Table
                conndb.Database.ExecuteSqlRaw($"delete {tempTableName}");

                //將資料BulkInsert至暫存Table
                BulkInsert((SqlConnection)conndb.Database.GetDbConnection(), tempTableName, source.Select(r => r as BaseEntity).ToList());

                if (conndb.Database.GetDbConnection().State != ConnectionState.Closed)
                    //關閉Connection連接
                    conndb.Database.CloseConnection();
            }

            //取得除了key值外的columnNames
            var entityType = EFCoreHelper.GetEntityType(context, typeof(TempEntity));
            var columnNames = entityType.GetProperties().Select(r => r.Name).ToList().Where(r => !entityType.GetKeys().Select(k => k.GetName()).Contains(r)).ToList();

            //把資料從TempTable寫進TransactionLists Db改用回交易處理的DbContext
            context.Database.ExecuteSqlRaw(
                $"insert into {targetTableName} ([{string.Join("],[", columnNames)}]) " +
                $"SELECT [{string.Join("],[", columnNames)}] FROM [dbo].[{tempTableName}]"
            );
        }

        /// <summary>
        /// Bulk Insert
        /// </summary>
        /// <typeparam name="T">Collecion Type</typeparam>
        /// <param name="conn">SQL Connection</param>
        /// <param name="tableName">Insert Table Name</param>
        /// <param name="timeout">Over Time</param>
        /// <param name="list">資料集合</param>
        /// <param name="option">BulkCopy選項，預設為「對所有選項使用預設值」</param>
        /// <param name="columns">指定Insert欄位</param>
        public static void BulkInsert<T>(
            SqlConnection conn, string tableName,
            IList<T> list, int timeout = 300,
            SqlBulkCopyOptions option = SqlBulkCopyOptions.Default,
            params string[] columns)
        {
            using (var bulkCopy = new SqlBulkCopy(conn, option, null))
            {
                bulkCopy.BulkCopyTimeout = timeout;
                bulkCopy.BatchSize = list.Count;
                bulkCopy.DestinationTableName = tableName;

                var table = new DataTable();
                var props = typeof(T).GetProperties().Where(x => (x.PropertyType.Namespace.Equals("System") ||
                    x.PropertyType.IsEnum) && x.GetCustomAttributes(typeof(NotMappedAttribute), true)?.Count() == 0).ToArray();

                // 如果有特別指定 Insert 欄位
                if (columns.Any())
                {
                    props = props.Where(r => columns.Contains(r.Name)).ToArray();

                    foreach (var propertyInfo in props)
                    {
                        bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                        table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                    }

                    var values = new object[columns.Length];

                    foreach (var item in list)
                    {
                        for (var i = 0; i < values.Length; i++)
                        {
                            values[i] = props[i].GetValue(item);
                        }

                        table.Rows.Add(values);
                    }
                }
                else
                {
                    foreach (var propertyInfo in props)
                    {
                        bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                        table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                    }

                    var values = new object[props.Length];

                    foreach (var item in list)
                    {
                        for (var i = 0; i < values.Length; i++)
                        {
                            values[i] = props[i].GetValue(item);
                        }

                        table.Rows.Add(values);
                    }
                }

                table.TableName = tableName;
                bulkCopy.WriteToServer(table);
            }
        }

        /// <summary>
        /// 批次更新(來源從檔案來)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void BulkUpdate<T>(List<T> source, DbContext context, List<string> keys = null, params string[] columns)
        {

            var tbName = EFCoreHelper.GetTableName(context, typeof(T));
            var tempTableName = $"#TmpTable";

            using (var connection = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
            {
                connection.Open();
                var command = connection.CreateCommand();

                // create temp table
                //建立Temp Table
                try
                {
                    var tableName = EFCoreHelper.GetTableName(context, typeof(T));
                    command.CommandText = GetGeneruteTemplateSQL(connection, tableName);
                    command.ExecuteNonQuery();

                    var props = typeof(T).GetProperties().Where(x => (x.PropertyType.Namespace.Equals("System") ||
                                x.PropertyType.IsEnum) && x.GetCustomAttributes(typeof(NotMappedAttribute), true)?.Count() == 0)
                                .ToList();

                    var table = GetDataTable(source, tempTableName, props);
                    props.ForEach(p => bulkCopy.ColumnMappings.Add(p.Name, p.Name));
                    bulkCopy.DestinationTableName = tempTableName;
                    bulkCopy.WriteToServer(table);

                    var entityType = EFCoreHelper.GetEntityType(context, typeof(T));
                    keys = keys ?? entityType.GetKeys().Select(r => r.GetName()).ToList();

                    var updateColumns = columns.Select(r => $"{tbName}.[{r}] = {tempTableName}.[{r}]").ToList();
                    var joinStr = keys.Select(r => $"{tbName}.[{r}] = {tempTableName}.[{r}]").ToList();

                    //command.CommandText = $"select * from {tempTableName}";

                    //using (var reader = command.ExecuteReader())
                    //{
                    //    while (reader.Read())
                    //    {
                    //        var temp = reader;
                    //    }
                    //}

                    //把資料從TempTable Update進TransactionLists Db改用回交易處理的DbContext
                    command.CommandText =
                        $" UPDATE {tbName} " +
                        $" SET {string.Join(" , ", updateColumns)} " +
                        $" FROM {tbName} INNER JOIN {tempTableName} " +
                        $" ON " +
                        $" {string.Join(" and ", joinStr)} ";

                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    command.CommandText = $"IF EXISTS (SELECT * FROM sys.triggers WHERE name = '{tempTableName}')  DROP TRIGGER {tempTableName}";
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Bulks the update by temporary table.
        /// </summary>
        /// <typeparam name="TempEntity">The type of the emp entity.</typeparam>
        /// <typeparam name="TargetEntity">The type of the arget entity.</typeparam>
        /// <typeparam name="BaseEntity">The type of the ase entity.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="context">Db Context</param>
        /// <param name="option">BulkCopy選項，預設為「對所有選項使用預設值」</param>
        /// <param name="columns">The columns.</param>
        public static void BulkUpdateByTempTable<TempEntity, TargetEntity, BaseEntity>(List<TargetEntity> source, DbContext context, SqlBulkCopyOptions option = SqlBulkCopyOptions.Default, params string[] columns)
            where TempEntity : BaseEntity
            where TargetEntity : BaseEntity
            where BaseEntity : class
        {
            if (source == null) return;

            string tempTableName = EFCoreHelper.GetTableName(context, typeof(TempEntity));
            string targetTableName = EFCoreHelper.GetTableName(context, typeof(TargetEntity));

            //取得除了key值外的columnNames
            var entityType = EFCoreHelper.GetEntityType(context, typeof(TempEntity));
            var keys = entityType.GetKeys().Select(r => r.GetName()).ToList();

            using (var conndb = context)
            {
                if (conndb.Database.GetDbConnection().State != ConnectionState.Open)
                    conndb.Database.GetDbConnection().Open();

                //刪除暫存Table
                conndb.Database.ExecuteSqlRaw($"delete {tempTableName}");

                var bulkColumns = columns.ToList();
                bulkColumns.AddRange(keys);

                //將資料BulkInsert至暫存Table
                BulkInsert((SqlConnection)conndb.Database.GetDbConnection(), tempTableName, source.Select(r => r).ToList(), option: option, columns: bulkColumns.ToArray());

                if (conndb.Database.GetDbConnection().State != ConnectionState.Closed)
                    conndb.Database.GetDbConnection().Close();
            }

            var updateColumns = columns.Select(r => $"{targetTableName}.[{r}] = {tempTableName}.[{r}]").ToList();
            var joinStr = keys.Select(r => $"{targetTableName}.[{r}] = {tempTableName}.[{r}]").ToList();

            //把資料從TempTable Update進TransactionLists Db改用回交易處理的DbContext
            var updateRow = context.Database.ExecuteSqlRaw(
                            $" UPDATE {targetTableName} " +
                            $" SET {string.Join(" , ", updateColumns)} " +
                            $" FROM {targetTableName} INNER JOIN {tempTableName} " +
                            $" ON " +
                            $" {string.Join(" and ", joinStr)} ");
        }

        /// <summary>
        /// 輸入 DB 中指定 table 名稱，並在資料庫中建立對應其欄位的 tmpTable 的 Create SQL
        /// </summary>
        /// <param name="conn">SQL 連線</param>
        /// <param name="tbName">對應 table 名稱</param>
        /// <returns>Create SQL</returns>
        public static string GetGeneruteTemplateSQL(SqlConnection conn, string tbName)
        {
            var tempTableName = $"#TmpTable";

            using (var command = conn.CreateCommand())
            {
                command.CommandText = "select * from information_schema.columns where table_name = @tableName";
                command.Parameters.Add("@tableName", SqlDbType.VarChar).Value = tbName;
                command.CommandType = CommandType.Text;

                var columnList = new List<ColumnInfo>();

                // Loop over the results and create a ColumnInfo object for each Column in the schema.
                // 使用迴圈取得欄位資訊
                using (IDataReader reader = command.ExecuteReader(CommandBehavior.KeyInfo))
                {
                    while (reader.Read())
                    {
                        columnList.Add(new ColumnInfo().ReadFromReader(reader));
                    }
                }

                string createTempCommand = "create table {0} ({1})";
                StringBuilder sb = new StringBuilder();

                // Loop over each column info object and construct the string needed for the SQL script.
                // 使用迴圈將欄位資訊轉SQL
                columnList.ForEach(column => sb.Append(column.ToString()));

                return string.Format(createTempCommand, tempTableName,
                    string.Join(",", columnList.Select(c => c.ToString()).ToArray()));
            }
        }

        static DataTable GetDataTable<T>(List<T> source, string tableName, List<PropertyInfo> props)
        {
            var table = new DataTable();
            props.ForEach(p => table.Columns.Add(p.Name, Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType));
            var values = new object[props.Count];

            foreach (var item in source)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }

                table.Rows.Add(values);
            }

            table.TableName = tableName;
            return table;
        }

        internal class ColumnInfo
        {
            public string Name { get; set; }

            public string DataType { get; set; }

            public int OrdinalPosition { get; set; }

            public bool IsNullable { get; set; }

            public string MaxLength { get; set; }

            protected string MaxLengthFormatted
            {
                // note that columns with a max length return –1.
                get { return MaxLength.Equals("-1") ? "max" : MaxLength; }
            }

            public ColumnInfo ReadFromReader(IDataReader reader)
            {
                // get the necessary information from the datareader.
                // run the SQL on your database to see all the other information available.
                Name = reader["COLUMN_NAME"].ToString();
                DataType = reader["DATA_TYPE"].ToString();

                if (DataType.ToUpper() == "DECIMAL")
                {
                    //20190122 EasonChen 預設為 18,0 要給他不然會一直把小數點截掉
                    DataType += "(18,5)";
                }

                OrdinalPosition = (int)reader["ORDINAL_POSITION"];
                IsNullable = (string)reader["IS_NULLABLE"] == "YES";
                MaxLength = reader["CHARACTER_MAXIMUM_LENGTH"].ToString();

                return this;
            }

            public override string ToString()
            {
                string COLLATE = DataType.ToUpper().IndexOf("CHAR") > 0 ? " COLLATE Chinese_Taiwan_Stroke_CI_AS " : "";

                return string.Format("[{0}] {1}{2}{3} NULL",
                    Name,
                    DataType,
                    MaxLength == string.Empty ? "" : "(" + MaxLengthFormatted + ")",
                    COLLATE);
            }
        }
    }

    public static class DbUtil2
    {
        /// <summary>
        ///  基本的BulkInsert 
        /// </summary>
        /// <typeparam name="BaseEntity"> The type of the entity</typeparam>
        /// <param name="listOfBaseEntities"> Insert 資料來源</param>
        /// <param name="context">DB連線</param>
        /// <param name="timeout">逾時時間</param>
        public static void BulkInsertEntity<BaseEntity>(List<BaseEntity> listOfBaseEntities, DbContext context, Func<DbContext> getNewDbContextFunc, int timeout = 300)
        {
            string tableName = EFCoreHelper.GetTableName(context, typeof(BaseEntity));

            // 檢核 listOfBaseEntities 內容
            // 檢核錯誤會被放入集合，只能驗證不可null的值
            ValidationContext validationContext = new ValidationContext(listOfBaseEntities, null, null);
            List<ValidationResult> validationErrors = new List<ValidationResult>();
            bool validateSuccess = Validator.TryValidateObject(listOfBaseEntities, validationContext, validationErrors, true);

            if (validateSuccess)
            {
                using (var connection = getNewDbContextFunc())
                {
                    if (connection.Database.GetDbConnection().State != ConnectionState.Open)
                    {
                        //打開Connection連接
                        connection.Database.GetDbConnection().Open();
                    }

                    //調用BulkInsert方法,將entitys集合數據批量插入到資料庫的tolocation表中
                    BulkInsert((SqlConnection)connection.Database.GetDbConnection(), tableName, listOfBaseEntities, timeout);

                    if (connection.Database.GetDbConnection().State != ConnectionState.Closed)
                    {
                        //關閉Connection連接
                        connection.Database.GetDbConnection().Close();
                    }
                }
            }
            else
            {
                throw new Exception(string.Join(",", validationErrors.Select(r => $"ErrorMessage{r.ErrorMessage}")));
            }
        }

        /// <summary>
        /// Bulks the insert by temporary table.
        /// </summary>
        /// <typeparam name="TempEntity">The type of the emp entity.</typeparam>
        /// <typeparam name="TargetEntity">The type of the arget entity.</typeparam>
        /// <typeparam name="BaseEntity">The type of the ase entity.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="context">The database.</param>
        public static void BulkInsertByTempTable<TempEntity, TargetEntity, BaseEntity>(List<TargetEntity> source, DbContext context)
            where TempEntity : BaseEntity
            where TargetEntity : BaseEntity
            where BaseEntity : class
        {
            string tempTableName = EFCoreHelper.GetTableName(context, typeof(TempEntity));
            string targetTableName = EFCoreHelper.GetTableName(context, typeof(TargetEntity));

            using (var conndb = context)
            {
                if (conndb.Database.GetDbConnection().State != ConnectionState.Open)
                {
                    //打開Connection連接
                    conndb.Database.GetDbConnection().Open();
                }

                //刪除暫存Table
                conndb.Database.ExecuteSqlRaw($"delete {tempTableName}");

                //將資料BulkInsert至暫存Table
                BulkInsert((SqlConnection)conndb.Database.GetDbConnection(), tempTableName, source.Select(r => r as BaseEntity).ToList());

                if (conndb.Database.GetDbConnection().State != ConnectionState.Closed)
                {
                    //關閉Connection連接
                    conndb.Database.GetDbConnection().Close();
                }
            }

            //取得除了key值外的columnNames
            var entityType = EFCoreHelper.GetEntityType(context, typeof(TempEntity));
            var columnNames = entityType.GetProperties().Select(r => r.Name).ToList().Where(r => !entityType.GetKeys().Select(k => k.GetName()).Contains(r)).ToList();

            //把資料從TempTable寫進TransactionLists Db改用回交易處理的DbContext
            context.Database.ExecuteSqlRaw(
                $"insert into {targetTableName} ([{string.Join("],[", columnNames)}]) " +
                $"SELECT [{string.Join("],[", columnNames)}] FROM [dbo].[{tempTableName}]"
            );
        }

        /// <summary>
        /// Bulk Insert
        /// </summary>
        /// <typeparam name="T">Collecion Type</typeparam>
        /// <param name="conn">SQL Connection</param>
        /// <param name="tableName">Insert Table Name</param>
        /// <param name="timeout">Over Time</param>
        /// <param name="list">資料集合</param>
        /// <param name="option">BulkCopy選項，預設為「對所有選項使用預設值」</param>
        /// <param name="columns">指定Insert欄位</param>
        public static void BulkInsert<T>(
            SqlConnection conn, string tableName,
            IList<T> list, int timeout = 300,
            SqlBulkCopyOptions option = SqlBulkCopyOptions.Default,
            params string[] columns)
        {
            using (var bulkCopy = new SqlBulkCopy(conn, option, null))
            {
                bulkCopy.BulkCopyTimeout = timeout;
                bulkCopy.BatchSize = list.Count;
                bulkCopy.DestinationTableName = tableName;

                var table = new DataTable();
                var props = typeof(T).GetProperties().Where(x => (x.PropertyType.Namespace.Equals("System") ||
                    x.PropertyType.IsEnum) && x.GetCustomAttributes(typeof(NotMappedAttribute), true)?.Count() == 0).ToArray();

                // 如果有特別指定 Insert 欄位
                if (columns.Any())
                {
                    props = props.Where(r => columns.Contains(r.Name)).ToArray();

                    foreach (var propertyInfo in props)
                    {
                        bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                        table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                    }

                    var values = new object[columns.Length];

                    foreach (var item in list)
                    {
                        for (var i = 0; i < values.Length; i++)
                        {
                            values[i] = props[i].GetValue(item);
                        }

                        table.Rows.Add(values);
                    }
                }
                else
                {
                    foreach (var propertyInfo in props)
                    {
                        bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                        table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                    }

                    var values = new object[props.Length];

                    foreach (var item in list)
                    {
                        for (var i = 0; i < values.Length; i++)
                        {
                            values[i] = props[i].GetValue(item);
                        }

                        table.Rows.Add(values);
                    }
                }

                table.TableName = tableName;
                bulkCopy.WriteToServer(table);
            }
        }

        /// <summary>
        /// 批次更新(來源從檔案來)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void BulkUpdate<T>(List<T> source, DbContext context, List<string> keys = null, params string[] columns)
        {

            var tbName = EFCoreHelper.GetTableName(context, typeof(T));
            var tempTableName = $"#TmpTable";

            using (var connection = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    var command = connection.CreateCommand();

                    // create temp table
                    //建立Temp Table
                    try
                    {
                        var tableName = EFCoreHelper.GetTableName(context, typeof(T));
                        command.CommandText = GetGeneruteTemplateSQL(connection, tableName);
                        command.ExecuteNonQuery();

                        var props = typeof(T).GetProperties().Where(x => (x.PropertyType.Namespace.Equals("System") ||
                                    x.PropertyType.IsEnum) && x.GetCustomAttributes(typeof(NotMappedAttribute), true)?.Count() == 0)
                                    .ToList();

                        var table = GetDataTable(source, tempTableName, props);
                        props.ForEach(p => bulkCopy.ColumnMappings.Add(p.Name, p.Name));
                        bulkCopy.DestinationTableName = tempTableName;
                        bulkCopy.WriteToServer(table);

                        var entityType = EFCoreHelper.GetEntityType(context, typeof(T));
                        keys = keys ?? entityType.GetKeys().Select(r => r.GetName()).ToList();

                        var updateColumns = columns.Select(r => $"{tbName}.[{r}] = {tempTableName}.[{r}]").ToList();
                        var joinStr = keys.Select(r => $"{tbName}.[{r}] = {tempTableName}.[{r}]").ToList();

                        //把資料從TempTable Update進TransactionLists Db改用回交易處理的DbContext
                        command.CommandText =
                            $" UPDATE {tbName} " +
                            $" SET {string.Join(" , ", updateColumns)} " +
                            $" FROM {tbName} INNER JOIN {tempTableName} " +
                            $" ON " +
                            $" {string.Join(" and ", joinStr)} ";

                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        command.CommandText = $"IF EXISTS (SELECT * FROM sys.triggers WHERE name = '{tempTableName}')  DROP TRIGGER {tempTableName}";
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Bulks the update by temporary table.
        /// </summary>
        /// <typeparam name="TempEntity">The type of the emp entity.</typeparam>
        /// <typeparam name="TargetEntity">The type of the arget entity.</typeparam>
        /// <typeparam name="BaseEntity">The type of the ase entity.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="context">Db Context</param>
        /// <param name="option">BulkCopy選項，預設為「對所有選項使用預設值」</param>
        /// <param name="columns">The columns.</param>
        public static void BulkUpdateByTempTable<TempEntity, TargetEntity, BaseEntity>(List<TargetEntity> source, DbContext context, SqlBulkCopyOptions option = SqlBulkCopyOptions.Default, params string[] columns)
            where TempEntity : BaseEntity
            where TargetEntity : BaseEntity
            where BaseEntity : class
        {
            if (source == null) return;

            string tempTableName = EFCoreHelper.GetTableName(context, typeof(TempEntity));
            string targetTableName = EFCoreHelper.GetTableName(context, typeof(TargetEntity));

            //取得除了key值外的columnNames
            var entityType = EFCoreHelper.GetEntityType(context, typeof(TempEntity));
            var keys = entityType.GetKeys().Select(r => r.GetName()).ToList();

            using (var conndb = context)
            {
                if (conndb.Database.GetDbConnection().State != ConnectionState.Open)
                    conndb.Database.GetDbConnection().Open();

                //刪除暫存Table
                conndb.Database.ExecuteSqlRaw($"delete {tempTableName}");

                var bulkColumns = columns.ToList();
                bulkColumns.AddRange(keys);

                //將資料BulkInsert至暫存Table
                BulkInsert((SqlConnection)conndb.Database.GetDbConnection(), tempTableName, source.Select(r => r).ToList(), option: option, columns: bulkColumns.ToArray());

                if (conndb.Database.GetDbConnection().State != ConnectionState.Closed)
                    conndb.Database.GetDbConnection().Close();
            }

            var updateColumns = columns.Select(r => $"{targetTableName}.[{r}] = {tempTableName}.[{r}]").ToList();
            var joinStr = keys.Select(r => $"{targetTableName}.[{r}] = {tempTableName}.[{r}]").ToList();

            //把資料從TempTable Update進TransactionLists Db改用回交易處理的DbContext
            var updateRow = context.Database.ExecuteSqlRaw(
                            $" UPDATE {targetTableName} " +
                            $" SET {string.Join(" , ", updateColumns)} " +
                            $" FROM {targetTableName} INNER JOIN {tempTableName} " +
                            $" ON " +
                            $" {string.Join(" and ", joinStr)} ");
        }

        /// <summary>
        /// 輸入 DB 中指定 table 名稱，並在資料庫中建立對應其欄位的 tmpTable 的 Create SQL
        /// </summary>
        /// <param name="conn">SQL 連線</param>
        /// <param name="tbName">對應 table 名稱</param>
        /// <returns>Create SQL</returns>
        public static string GetGeneruteTemplateSQL(SqlConnection conn, string tbName)
        {
            var tempTableName = $"#TmpTable";

            using (var command = conn.CreateCommand())
            {
                command.CommandText = "select * from information_schema.columns where table_name = @tableName";
                command.Parameters.Add("@tableName", SqlDbType.VarChar).Value = tbName;
                command.CommandType = CommandType.Text;

                var columnList = new List<ColumnInfo>();

                // Loop over the results and create a ColumnInfo object for each Column in the schema.
                // 使用迴圈取得欄位資訊
                using (IDataReader reader = command.ExecuteReader(CommandBehavior.KeyInfo))
                {
                    while (reader.Read())
                    {
                        columnList.Add(new ColumnInfo().ReadFromReader(reader));
                    }
                }

                string createTempCommand = "create table {0} ({1})";
                StringBuilder sb = new StringBuilder();

                // Loop over each column info object and construct the string needed for the SQL script.
                // 使用迴圈將欄位資訊轉SQL
                columnList.ForEach(column => sb.Append(column.ToString()));

                return string.Format(createTempCommand, tempTableName,
                    string.Join(",", columnList.Select(c => c.ToString()).ToArray()));
            }
        }

        static DataTable GetDataTable<T>(List<T> source, string tableName, List<PropertyInfo> props)
        {
            var table = new DataTable();
            props.ForEach(p => table.Columns.Add(p.Name, Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType));
            var values = new object[props.Count];

            foreach (var item in source)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }

                table.Rows.Add(values);
            }

            table.TableName = tableName;
            return table;
        }

        internal class ColumnInfo
        {
            public string Name { get; set; }

            public string DataType { get; set; }

            public int OrdinalPosition { get; set; }

            public bool IsNullable { get; set; }

            public string MaxLength { get; set; }

            protected string MaxLengthFormatted
            {
                // note that columns with a max length return –1.
                get { return MaxLength.Equals("-1") ? "max" : MaxLength; }
            }

            public ColumnInfo ReadFromReader(IDataReader reader)
            {
                // get the necessary information from the datareader.
                // run the SQL on your database to see all the other information available.
                Name = reader["COLUMN_NAME"].ToString();
                DataType = reader["DATA_TYPE"].ToString();

                if (DataType.ToUpper() == "DECIMAL")
                {
                    //20190122 EasonChen 預設為 18,0 要給他不然會一直把小數點截掉
                    DataType += "(18,5)";
                }

                OrdinalPosition = (int)reader["ORDINAL_POSITION"];
                IsNullable = (string)reader["IS_NULLABLE"] == "YES";
                MaxLength = reader["CHARACTER_MAXIMUM_LENGTH"].ToString();

                return this;
            }

            public override string ToString()
            {
                string COLLATE = DataType.ToUpper().IndexOf("CHAR") > 0 ? " COLLATE Chinese_Taiwan_Stroke_CI_AS " : "";

                return string.Format("[{0}] {1}{2}{3} NULL",
                    Name,
                    DataType,
                    MaxLength == string.Empty ? "" : "(" + MaxLengthFormatted + ")",
                    COLLATE);
            }
        }
    }
}