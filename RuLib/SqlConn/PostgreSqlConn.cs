using System.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace RuLib.SqlConn;

public class PostgreSqlConn : IConn
{
    public bool IsConnected => _connection.State == ConnectionState.Open;
    public DbType DbType => DbType.PostgreSql;

    private readonly NpgsqlConnection _connection;

    public PostgreSqlConn(ConnOptions option)
    {
        _connection = new NpgsqlConnection(option.ConnectionString);
    }

    public DbContext GetDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder();
        optionsBuilder.UseNpgsql(_connection.ConnectionString);
        return new DbContext(optionsBuilder.Options);
    }

    public bool TestConnection(out string message)
    {
        try
        {
            _connection.Open();
            message = "PostgreSql 連線成功。";
            return true;
        }
        catch (Exception ex)
        {
            message = $"PostgreSql 連線失敗：{ex.Message}";
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
        using (var command = new NpgsqlCommand(query, _connection))
        using (var adapter = new NpgsqlDataAdapter(command))
        {
            _connection.Open();
            adapter.Fill(dataTable);
            _connection.Close();
        }
        return dataTable;
    }

    public int ExecuteNonQuery(string commandText)
    {
        using (var command = new NpgsqlCommand(commandText, _connection))
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
