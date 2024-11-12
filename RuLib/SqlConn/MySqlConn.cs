using System.Data;
using MySql.Data.MySqlClient;

namespace RuLib.SqlConn;

public class MySqlConn : IConn
{
    public bool IsConnected => _connection.State == ConnectionState.Open;
    public DbType DbType => DbType.MsSql;

    private readonly MySqlConnection _connection;

    public MySqlConn(string connectionString)
    {
        _connection = new MySqlConnection(connectionString);
    }

    public bool TestConnection(out string message)
    {
        try
        {
            _connection.Open();
            message = "MySql 連線成功。";
            return true;
        }
        catch (Exception ex)
        {
            message = $"MySql 連線失敗：{ex.Message}";
            return false;
        }
        finally
        {
            _connection.Close();
        }
    }

    public DataTable ExecuteQuery(string query)
    {
        DataTable dataTable = new DataTable();
        using (var command = new MySqlCommand(query, _connection))
        using (var adapter = new MySqlDataAdapter(command))
        {
            _connection.Open();
            adapter.Fill(dataTable);
            _connection.Close();
        }
        return dataTable;
    }

    public int ExecuteNonQuery(string commandText)
    {
        using (var command = new MySqlCommand(commandText, _connection))
        {
            _connection.Open();
            int rowsAffected = command.ExecuteNonQuery();
            _connection.Close();
            return rowsAffected;
        }
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}