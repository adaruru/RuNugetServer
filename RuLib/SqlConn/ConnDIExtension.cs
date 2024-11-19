using Microsoft.Extensions.DependencyInjection;

namespace RuLib.SqlConn;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomConnection(this IServiceCollection services, ConnOptions options)
    {
        services.AddSingleton(options);
        services.AddScoped<IConn>(provider => options.DbType switch
        {
            ConnType.MsSql => new MsSqlConn(options),
            ConnType.MySql => new MySqlConn(options),
            ConnType.PostgreSql => new PostgreSqlConn(options),
            ConnType.OracleSql => new OracleSqlConn(options),
            _ => throw new NotSupportedException("Unsupported database type")
        });

        return services;
    }
}