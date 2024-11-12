
using System.Data;
using System.Data.SqlClient;
namespace RuLib.DbConnect;

public class MsSqlConn : IConn
{
    public bool IsConnected => _connection.State == ConnectionState.Open;
    public DbType DbType  => DbType.MsSql;

    private readonly SqlConnection _connection;
    public MsSqlConn(string connectionString)
    {
        _connection = new SqlConnection(connectionString);
    }

    public bool TestConnection(out string message)
    {
        try
        {
            _connection.Open();
            message = "連線成功。";
            return true;
        }
        catch (Exception ex)
        {
            message = $"連線失敗：{ex.Message}";
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
        using (var command = new SqlCommand(query, _connection))
        using (var adapter = new SqlDataAdapter(command))
        {
            _connection.Open();
            adapter.Fill(dataTable);
            _connection.Close();
        }
        return dataTable;
    }

    public int ExecuteNonQuery(string commandText)
    {
        using (var command = new SqlCommand(commandText, _connection))
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