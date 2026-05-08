namespace ECommerce.API.Features.Cart.GetCart;

public record CartItemModel(int ProductId, string Name, decimal Price, int Quantity, string ImageUrl, decimal SubTotal);
public record GetCartResult(List<CartItemModel> Items, decimal TotalPrice);
public record GetCartQuery(int UserId) : IRequest<GetCartResult>;

public class GetCartHandler(ISqlConnectionFactory connectionFactory) 
    : IRequestHandler<GetCartQuery, GetCartResult>
{
    public async Task<GetCartResult> Handle(GetCartQuery query, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var sql = @"
            SELECT 
                c.ProductId,
                p.[Name],
                p.Price,
                c.Quantity,
                p.ImageUrl
            FROM CartItems c
            INNER JOIN Products p ON c.ProductId = p.Id
            WHERE c.UserId = @UserId";

        using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@UserId", query.UserId);

        using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        var items = new List<CartItemModel>();
        decimal totalPrice = 0;

        while (await reader.ReadAsync(cancellationToken))
        {
            var price = reader.GetDecimal(reader.GetOrdinal("Price"));
            var quantity = reader.GetInt32(reader.GetOrdinal("Quantity"));
            var subTotal = price * quantity;

            var item = new CartItemModel(
                reader.GetInt32(reader.GetOrdinal("ProductId")),
                reader.GetString(reader.GetOrdinal("Name")),
                price,
                quantity,
                reader.GetString(reader.GetOrdinal("ImageUrl")),
                subTotal
                );

            items.Add(item);
            totalPrice += subTotal;
        }
        return new GetCartResult(items, totalPrice);
    }
}