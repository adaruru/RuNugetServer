using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RuLib.SqlConn
{
    public class EFCoreHelper
    {
        public DbContext Context { get; private set; }
        public EFCoreHelper(ConnOptions connOptions)
        {
            // 根據 dbtype 初始化對應的 DbContext
            Context = CreateDbContext(connOptions);
        }
        private DbContext CreateDbContext(ConnOptions options)
        {
            IConn connection = options.DbType switch
            {
                ConnType.MsSql => new MsSqlConn(options),
                ConnType.MySql => new MySqlConn(options),
                ConnType.OracleSql => new OracleSqlConn(options),
                ConnType.PostgreSql => new PostgreSqlConn(options),
                _ => throw new ArgumentException("Unsupported database type")
            };
            return connection.GetDbContext();
        }

        public static IEntityType GetEntityType(DbContext context, Type entityType)
        {
            return context.Model.FindEntityType(entityType);
        }

        public static PropertyInfo GetPropertyInfo(IModel Model, Type entityType)
        {
            PropertyInfo result = new PropertyInfo();
            result.PKs.AddRange(Model.FindEntityType(entityType).FindPrimaryKey().Properties.Select(x => x.Name));
            return result;
        }

        public static string GetTableName(DbContext context, Type entityType)
        {
            return context.Model.FindEntityType(entityType).GetTableName();
        }

        public string GetTableName(Type entityType)
        {
            var entity = Context.Model.FindEntityType(entityType);
            if (entity == null)
                throw new ArgumentException("Entity type not found in the current DbContext");
            return $"{entity.GetSchema()}.{entity.GetTableName()}";
        }


        public class PropertyInfo
        {
            public PropertyInfo()
            {
                PKs = new List<string>();
            }

            public List<string> PKs { get; }
        }
    }
}