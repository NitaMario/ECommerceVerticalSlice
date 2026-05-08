namespace ECommerce.API.Features.Catalog.GetProducts;

public record ProductModel(int Id, string Name, string Description, decimal Price, string ImageUrl);

public record GetProductsResult(IEnumerable<ProductModel> Products);

public record GetProductsQuery : IRequest<GetProductsResult>;

public class GetProductsHandler(ISqlConnectionFactory connectionFactory)
    : IRequestHandler<GetProductsQuery, GetProductsResult>
{
    public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var products = new List<ProductModel>();

        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var sql = "SELECT Id, [Name], [Description], Price, ImageUrl FROM Products";
        using var command = new SqlCommand(sql, connection);

        using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            products.Add(new ProductModel(
                Id: reader.GetInt32(0),
                Name: reader.GetString(1),
                Description: reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                Price: reader.GetDecimal(3),
                ImageUrl: reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                ));
        }

        return new GetProductsResult(products);
    }
}
