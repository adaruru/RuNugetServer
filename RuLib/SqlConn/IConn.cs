using System.Data;

namespace RuLib.DbConnect;

public interface IConn : IDisposable
{
    bool IsConnected { get; }
    DbType DbType { get; }

    bool TestConnection(out string message);
    DataTable ExecuteQuery(string query);
    int ExecuteNonQuery(string commandText);
}
