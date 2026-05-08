using System.Security.Claims;

namespace ECommerce.API.Features.Cart.AddToCart;
public record AddToCartRequest(int ProductId, int Quantity);

public class AddToCartEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/cart", async (AddToCartRequest request, ClaimsPrincipal user, IMediator mediator) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Results.Unauthorized();
            }

            var command = new AddToCartCommand(userId, request.ProductId, request.Quantity);

            var result = await mediator.Send(command);

            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
        })
        .WithName("AddToCart")
        .WithTags("Cart")
        .RequireAuthorization()
        .Produces<AddToCartResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}
