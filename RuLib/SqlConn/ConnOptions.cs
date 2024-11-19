namespace RuLib.SqlConn;

public class ConnOptions
{
    public string ConnectionString { get; set; }
    public ConnType DbType { get; set; }
}

/// <summary>
/// Specifies the type of database to connect to.
/// </summary>
public enum ConnType
{
    None = 0,
    /// <summary>
    /// Microsoft SQL Server database.
    /// </summary>
    MsSql = 1,

    /// <summary>
    /// MySQL database, commonly used for web applications and open-source projects.
    /// </summary>
    MySql = 2,

    /// <summary>
    /// PostgreSQL database, known for its extensibility and SQL compliance.
    /// </summary>
    PostgreSql = 3,

    /// <summary>
    /// Oracle database, widely used in enterprise environments.
    /// </summary>
    OracleSql = 4
}