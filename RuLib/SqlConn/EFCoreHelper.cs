using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RuLib.SqlConn
{
    public class EFCoreHelper
    {

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