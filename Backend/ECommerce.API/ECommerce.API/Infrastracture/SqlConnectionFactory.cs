
namespace ECommerce.API.Infrastracture;

public class SqlConnectionFactory(IConfiguration configuration) : ISqlConnectionFactory
{
    public SqlConnection CreateConnection()
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        return new SqlConnection(connectionString);
    }
}
