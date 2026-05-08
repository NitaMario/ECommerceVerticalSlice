
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ECommerce.API.Features.Identity.LoginUser;

public record LoginUserResult(bool IsSuccess, string Token, string Message);

public record LoginUserCommand(string Email, string Password)
    : IRequest<LoginUserResult>;

public class LoginUserHandler(ISqlConnectionFactory connectionFactory, IConfiguration configuration)
    : IRequestHandler<LoginUserCommand, LoginUserResult>
{
    public async Task<LoginUserResult> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var sql = "SELECT Id, [Name], PasswordHash FROM Users WHERE Email = @Email";
        using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Email", command.Email);

        using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return new LoginUserResult(false, string.Empty, "Invalid email or password.");
        }

        var userId = reader.GetInt32(0);
        var name = reader.GetString(1);
        var dbPasswordHash = reader.GetString(2);

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(command.Password, dbPasswordHash);
        if (!isPasswordValid)
        {
            return new LoginUserResult(false, string.Empty, "Invalid email or password.");
        }

        var token = GenerateJwtToken(userId, name, command.Email);

        return new LoginUserResult(true, token, "Login successful.");
    }

    private string GenerateJwtToken(int userId, string name, string email)
    {
        var secretKey = Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Name, name),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(secretKey);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(configuration["Jwt:ExpirationInMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
