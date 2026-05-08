namespace ECommerce.API.Features.Cart.RemoveFromCart;

public class RemoveFromCartEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/cart/{productId:int}", async (int productId, ClaimsPrincipal user, IMediator mediator) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Results.Unauthorized();
            }

            var command = new RemoveFromCartCommand(userId, productId);
            var result = await mediator.Send(command);

            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
        })
        .WithName("RemoveFromCart")
        .WithTags("Cart")
        .RequireAuthorization()
        .Produces<RemoveFromCartResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}
