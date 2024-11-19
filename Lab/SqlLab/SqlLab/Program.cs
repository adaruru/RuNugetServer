using Microsoft.Extensions.DependencyInjection;
using RuLib.SqlConn;

namespace SqlLab;

internal static class Program
{
    public static ServiceProvider ServiceProvider { get; private set; }
    /// <summary> 
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // 建立 DI 容器
        var services = new ServiceCollection();

        //// 註冊 ConnOptions
        //services.AddSingleton(new ConnOptions
        //{
        //    ConnectionString = "Data Source=IPC16;Initial Catalog=Northwind;user id=sa;password=sa",
        //    DbType = ConnType.MsSql
        //});

        //// 註冊 IConn 為 MsSqlConn
        //services.AddScoped<IConn, MsSqlConn>();

        services.AddCustomConnection(new ConnOptions
        {
            ConnectionString = "Data Source=IPC16;Initial Catalog=Northwind;user id=sa;password=sa",
            DbType = ConnType.MsSql
        });

        // 註冊表單
        services.AddSingleton<SqlLabForm>();

        // 建立 ServiceProvider
        ServiceProvider = services.BuildServiceProvider();

        // 啟動應用程式
        ApplicationConfiguration.Initialize();
        Application.Run(ServiceProvider.GetRequiredService<SqlLabForm>());
    }
}