
namespace RuLib.DbConnect;

public class ConnOptions
{
    public string ConnectionString { get; set; }
    public DbType DbType { get; set; }
}

public enum DbType
{
    MsSql,
    MySql,
    PostgreSql,
    OracleSql,
}