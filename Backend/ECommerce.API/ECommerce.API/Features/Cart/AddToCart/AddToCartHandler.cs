
namespace ECommerce.API.Features.Cart.AddToCart;

public record AddToCartResult(bool IsSuccess, string Message);
public record AddToCartCommand(int UserId, int ProductId, int Quantity)
    : IRequest<AddToCartResult>;

public class AddToCartHandler(ISqlConnectionFactory connectionFactory)
    : IRequestHandler<AddToCartCommand, AddToCartResult>
{
    public async Task<AddToCartResult> Handle(AddToCartCommand command, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var sql = @"
            IF EXISTS (SELECT 1 FROM CartItems WHERE UserId = @UserId AND ProductId = @ProductId)
            BEGIN
                UPDATE CartItems
                SET Quantity = Quantity + @Quantity
                WHERE UserId = @UserId AND ProductId = @ProductId
            END
            ELSE
            BEGIN
                INSERT INTO CartItems (UserId, ProductId, Quantity, CreatedAt)
                VALUES (@UserId, @ProductId, @Quantity, @CreatedAt)
            END";

        using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@UserId", command.UserId);
        cmd.Parameters.AddWithValue("@ProductId", command.ProductId);
        cmd.Parameters.AddWithValue("@Quantity", command.Quantity);
        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

        var rowsAffected = await cmd.ExecuteNonQueryAsync(cancellationToken);

        return rowsAffected > 0
            ? new AddToCartResult(true, "Successfully added item to cart.")
            : new AddToCartResult(false, "Failed to add item to cart.");
    }
}
