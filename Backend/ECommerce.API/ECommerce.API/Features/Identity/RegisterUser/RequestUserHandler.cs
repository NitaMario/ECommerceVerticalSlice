namespace ECommerce.API.Features.Identity.RegisterUser;

public record RegisterUserResult(bool IsSuccess, string Message);

public record RegisterUserCommand(string Name, string Email, string Password)
    : IRequest<RegisterUserResult>;

public class RequestUserHandler(ISqlConnectionFactory connectionFactory)
    : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    public async Task<RegisterUserResult> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var checkSql = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
        using var checkCmd = new SqlCommand(checkSql, connection);
        checkCmd.Parameters.AddWithValue("@Email", command.Email);

        var exists = (int)await checkCmd.ExecuteScalarAsync(cancellationToken) > 0;
        if (exists)
        {
            return new RegisterUserResult(false, "Email is already registered.");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password);

        var insertSql = @"
            INSERT INTO Users ([Name], Email, PasswordHash, CreatedAt)
            VALUES (@Name, @Email, @PasswordHash, @CreatedAt)";

        using var insertCmd = new SqlCommand(insertSql, connection);
        insertCmd.Parameters.AddWithValue("@Name", command.Name);
        insertCmd.Parameters.AddWithValue("@Email", command.Email);
        insertCmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

        insertCmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

        await insertCmd.ExecuteNonQueryAsync(cancellationToken);

        return new RegisterUserResult(true, "User registered successfully.");
    }
}
