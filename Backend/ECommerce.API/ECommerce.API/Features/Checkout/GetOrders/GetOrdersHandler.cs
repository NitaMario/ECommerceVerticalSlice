
namespace ECommerce.API.Features.Checkout.GetOrders;

public record OrderItemModel(int ProductId, string ProductName, decimal UnitPrice, int Quantity, string ImageUrl);

public class OrderModel
{
    public int OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public List<OrderItemModel> Items { get; set; } = new();
}
public record GetOrdersResult(List<OrderModel> Orders);
public record GetOrdersQuery(int UserId) : IRequest<GetOrdersResult>;
public class GetOrdersHandler(ISqlConnectionFactory connectionFactory)
    : IRequestHandler<GetOrdersQuery, GetOrdersResult>
{
    public async Task<GetOrdersResult> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var sql = @"
            SELECT
                o.Id AS OrderId,
                o.TotalAmount,
                o.ShippingAddress,
                o.OrderDate,
                oi.ProductId,
                oi.Quantity,
                oi.UnitPrice,
                p.[Name] AS ProductName,
                p.ImageUrl
            FROM Orders o
            INNER JOIN OrderItems oi ON o.Id = oi.OrderId
            INNER JOIN Products p ON oi.ProductId = p.Id
            WHERE o.UserId = @UserId
            ORDER BY o.OrderDate DESC";

        using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@UserId", query.UserId);

        using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        var orderDictionary = new Dictionary<int, OrderModel>();

        while (await reader.ReadAsync(cancellationToken))
        {
            var orderId = reader.GetInt32(reader.GetOrdinal("OrderId"));

            if (!orderDictionary.TryGetValue(orderId, out var order))
            {
                order = new OrderModel
                {
                    OrderId = orderId,
                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                    ShippingAddress = reader.GetString(reader.GetOrdinal("ShippingAddress")),
                    OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate"))
                };
                orderDictionary.Add(orderId, order);
            }

            var item = new OrderItemModel(
                reader.GetInt32(reader.GetOrdinal("ProductId")),
                reader.GetString(reader.GetOrdinal("ProductName")),
                reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                reader.GetInt32(reader.GetOrdinal("Quantity")),
                reader.GetString(reader.GetOrdinal("ImageUrl"))
            );

            order.Items.Add(item);
        }

        return new GetOrdersResult(orderDictionary.Values.ToList());
    }
}
