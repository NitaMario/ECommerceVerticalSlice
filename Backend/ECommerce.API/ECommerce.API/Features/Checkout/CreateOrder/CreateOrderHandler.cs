namespace ECommerce.API.Features.Checkout.CreateOrder;
public record CreateOrderRequest(string ShippingAddress);

public record CreateOrderResult(bool IsSuccess, string Message, int? OrderId = null);

public record CreateOrderCommand(int UserId, string ShippingAddress) : IRequest<CreateOrderResult>;
public class CreateOrderHandler(ISqlConnectionFactory connectionFactory)
    : IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var sql = @"
            DECLARE @TotalAmount DECIMAL(18, 2);
            
            SELECT @TotalAmount = SUM(p.Price * c.Quantity)
            FROM CartItems c
            INNER JOIN Products p ON c.ProductId = p.Id
            WHERE c.UserId = @UserId;

            IF @TotalAmount IS NULL OR @TotalAmount = 0
            BEGIN
                SELECT -1;
                RETURN;
            END

            BEGIN TRANSACTION;
            
            DECLARE @InsertedIds TABLE (Id INT);

            INSERT INTO Orders (UserId, TotalAmount, ShippingAddress, OrderDate)
            OUTPUT INSERTED.Id INTO @InsertedIds
            VALUES (@UserId, @TotalAmount, @ShippingAddress, GETUTCDATE());

            DECLARE @OrderId INT = (SELECT TOP 1 Id FROM @InsertedIds);
            
            INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
            SELECT @OrderId, c.ProductId, c.Quantity, p.Price
            FROM CartItems c
            INNER JOIN Products p ON c.ProductId = p.Id
            WHERE c.UserId = @UserId;

            DELETE FROM CartItems WHERE UserId = @UserId;
 
            COMMIT TRANSACTION;
 
            SELECT @OrderId;";

        using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@UserId", command.UserId);
        cmd.Parameters.AddWithValue("@ShippingAddress", command.ShippingAddress);

        var result = await cmd.ExecuteScalarAsync(cancellationToken);

        if (result == null || (int)result == -1)
        {
            return new CreateOrderResult(false, "Cannot create order: Cart is empty.");
        }

        var newOrderId = (int)result;
        return new CreateOrderResult(true, "Order created successfully.", newOrderId);
    }
}
