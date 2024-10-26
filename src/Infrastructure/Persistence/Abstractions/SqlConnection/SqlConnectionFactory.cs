using System.Data;
using Domain.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence.Abstractions.SqlConnection;

public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnectionString") ?? "";
    }

    public IDbConnection CreateConnection()
    {
        var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString);
        connection.Open();
        return connection;
    }
}