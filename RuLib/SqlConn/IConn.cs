using Microsoft.EntityFrameworkCore;

namespace RuLib.SqlConn;

public interface IConn : IDisposable
{
    bool IsConnected { get; }
    DbType DbType { get; }
    DbContext GetDbContext();
    bool TestConnection(out string message);
    DataTable ExecuteQuery(string query);
    int ExecuteNonQuery(string commandText);
}
