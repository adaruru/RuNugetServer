using Microsoft.EntityFrameworkCore;

namespace RuLib.SqlConn;

public interface IConn : IDisposable
{
    bool IsConnected { get; }
    ConnType DbType { get; }
    DbContext GetDbContext();
    bool OpenConn(out string message);
    DataTable ExecuteQuery(string query);
    int ExecuteNonQuery(string commandText);
}
