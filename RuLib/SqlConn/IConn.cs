using Microsoft.EntityFrameworkCore;

namespace RuLib.SqlConn;

public interface IConn : IDisposable
{
    bool IsConnected { get; }
    ConnType DbType { get; }
    string PrintMessage { get; set; }

    DbContext GetDbContext();
    bool OpenConn(out string message);
    DataTable ExecuteQuery(string query);
    int ExecuteNonQuery(string commandText);

    public void Dispose();
}