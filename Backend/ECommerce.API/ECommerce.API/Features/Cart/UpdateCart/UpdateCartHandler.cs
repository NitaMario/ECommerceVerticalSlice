namespace ECommerce.API.Features.Cart.UpdateCart;

public record UpdateCartResult(bool IsSuccess, string Message);
public record UpdateCartCommand(int UserId, int ProductId, int NewQuantity)
    : IRequest<UpdateCartResult>;
public class UpdateCartHandler(ISqlConnectionFactory connectionFactory)
    : IRequestHandler<UpdateCartCommand, UpdateCartResult>
{
    public async Task<UpdateCartResult> Handle(UpdateCartCommand command, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        if (command.NewQuantity <= 0)
        {
            var deleteSql = "DELETE FROM CartItems WHERE UserId = @UserId AND ProductId = @ProductId";
            using var deleteCmd = new SqlCommand(deleteSql, connection);
            deleteCmd.Parameters.AddWithValue("@UserId", command.UserId);
            deleteCmd.Parameters.AddWithValue("@ProductId", command.ProductId);
            await deleteCmd.ExecuteNonQueryAsync(cancellationToken);

            return new UpdateCartResult(true, "Item removed from cart.");
        }

        var sql = "UPDATE CartItems SET Quantity = @NewQuantity WHERE UserId = @UserId AND ProductId = @ProductId";
        using var sqlCmd = new SqlCommand(sql, connection);
        sqlCmd.Parameters.AddWithValue("@UserId", command.UserId);
        sqlCmd.Parameters.AddWithValue("@ProductId", command.ProductId);
        sqlCmd.Parameters.AddWithValue("@NewQuantity", command.NewQuantity);

        var rowsAffected = await sqlCmd.ExecuteNonQueryAsync(cancellationToken);

        return rowsAffected > 0
            ? new UpdateCartResult(true, "Quantity updated.")
            : new UpdateCartResult(false, "Item not found in cart.");
    }
}
