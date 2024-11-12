using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

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
                DbType.MsSql => new MsSqlConn(options),
                DbType.MySql => new MySqlConn(options),
                DbType.OracleSql => new OracleSqlConn(options),
                DbType.PostgreSql => new PostgreSqlConn(options),
                _ => throw new ArgumentException("Unsupported database type")
            };

            return connection.GetDbContext();
        }

        public static IEntityType GetEntityType(DbContext context, Type entityType)
        {
            return context.Model.FindEntityType(entityType);
        }

        public static PropertInfo GetPropertyInfo(IModel Model, Type entityType)
        {
            PropertInfo result = new PropertInfo();
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


        public class PropertInfo
        {
            public PropertInfo()
            {
                PKs = new List<string>();
            }

            public List<string> PKs { get; }
        }
    }
}