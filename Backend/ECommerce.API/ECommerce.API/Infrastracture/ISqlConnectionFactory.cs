namespace ECommerce.API.Infrastracture;

public interface ISqlConnectionFactory
{
    SqlConnection CreateConnection();
}

