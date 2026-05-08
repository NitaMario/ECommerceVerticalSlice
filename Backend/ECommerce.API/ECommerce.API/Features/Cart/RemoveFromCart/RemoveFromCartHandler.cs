namespace ECommerce.API.Features.Cart.RemoveFromCart;
public record RemoveFromCartResult(bool IsSuccess, string Message);

public record RemoveFromCartCommand(int UserId, int ProductId)
    : IRequest<RemoveFromCartResult>;

public class RemoveFromCartHandler(ISqlConnectionFactory connectionFactory)
    : IRequestHandler<RemoveFromCartCommand, RemoveFromCartResult>
{
    public async Task<RemoveFromCartResult> Handle(RemoveFromCartCommand command, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var sql = "DELETE FROM CartItems WHERE UserId = @UserId AND ProductId = @ProductId";

        using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@UserId", command.UserId);
        cmd.Parameters.AddWithValue("@ProductId", command.ProductId);

        var rowsAffected = await cmd.ExecuteNonQueryAsync(cancellationToken);

        return rowsAffected > 0
            ? new RemoveFromCartResult(true, "Successfully removed item from cart.")
            : new RemoveFromCartResult(false, "Item not found in cart.");
    }
}
