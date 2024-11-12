namespace RuLib.SqlConn;

public class ConnOptions
{
    public string ConnectionString { get; set; }
    public DbType DbType { get; set; }
}

/// <summary>
/// Specifies the type of database to connect to.
/// </summary>
public enum DbType
{
    /// <summary>
    /// Microsoft SQL Server database.
    /// </summary>
    MsSql = 0,

    /// <summary>
    /// MySQL database, commonly used for web applications and open-source projects.
    /// </summary>
    MySql = 1,

    /// <summary>
    /// PostgreSQL database, known for its extensibility and SQL compliance.
    /// </summary>
    PostgreSql = 2,

    /// <summary>
    /// Oracle database, widely used in enterprise environments.
    /// </summary>
    OracleSql = 3
}