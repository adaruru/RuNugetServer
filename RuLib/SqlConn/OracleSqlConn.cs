using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace RuLib.SqlConn;

public class OracleSqlConn : IConn
{
    public bool IsConnected => _connection.State == ConnectionState.Open;
    public DbType DbType => DbType.OracleSql;

    private readonly OracleConnection _connection;

    public OracleSqlConn(ConnOptions option)
    {
        _connection = new OracleConnection(option.ConnectionString);
    }
    public DbContext GetDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder();
        optionsBuilder.UseOracle(_connection.ConnectionString);
        return new DbContext(optionsBuilder.Options);
    }

    public bool TestConnection(out string message)
    {
        try
        {
            _connection.Open();
            message = "OracleSql 連線成功。";
            return true;
        }
        catch (Exception ex)
        {
            message = $"OracleSql 連線失敗：{ex.Message}";
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
        using (var command = new OracleCommand(query, _connection))
        using (var adapter = new OracleDataAdapter(command))
        {
            _connection.Open();
            adapter.Fill(dataTable);
            _connection.Close();
        }
        return dataTable;
    }

    public int ExecuteNonQuery(string commandText)
    {
        using (var command = new OracleCommand(commandText, _connection))
        {
            _connection.Open();
            int rowsAffected = command.ExecuteNonQuery();
            _connection.Close();
            return rowsAffected;
        }
    }

    public void Dispose()
    {
        if (_connection.State != ConnectionState.Closed)
        {
            _connection.Close();
        }
        _connection.Dispose();
    }
}
