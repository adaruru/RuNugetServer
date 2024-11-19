using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace RuLib.SqlConn;

public class MsSqlConn : DecorConn, IConn
{
    public bool IsConnected => _connection.State == ConnectionState.Open;
    public ConnType DbType => ConnType.MsSql;

    private readonly SqlConnection _connection;
    public MsSqlConn(ConnOptions option)
    {
        _connection = new SqlConnection(option.ConnectionString);
    }
    public DbContext GetDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder();
        optionsBuilder.UseSqlServer(_connection.ConnectionString);
        return new DbContext(optionsBuilder.Options);
    }

    public bool OpenConn(out string message)
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
        // 實時捕捉消息
        _connection.InfoMessage += (sender, args) =>
        {
            foreach (SqlError error in args.Errors)
            {
                // 將每條 InfoMessage 加入消息緩存
                PrintMessage += error.Message + Environment.NewLine;
            }
        };

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

        // 實時捕捉消息
        _connection.InfoMessage += (sender, args) =>
        {
            foreach (SqlError error in args.Errors)
            {
                // 將每條 InfoMessage 加入消息緩存
                PrintMessage += error.Message + Environment.NewLine;
            }
        };

        using (var command = new SqlCommand(commandText, _connection))
        {
            try
            {
                _connection.Open();

                // 實時執行 SQL 指令
                int rowsAffected = command.ExecuteNonQuery();
                // 返回影響的行數
                return rowsAffected;
            }
            catch (Exception ex)
            {
                PrintMessage += $"Error: {ex.Message}";
                throw; // 重新拋出異常
            }
            finally
            {
                _connection.Close();
            }
        }
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}