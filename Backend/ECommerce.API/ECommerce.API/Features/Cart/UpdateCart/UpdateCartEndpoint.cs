namespace ECommerce.API.Features.Cart.UpdateCart;

public record UpdateCartRequest(int ProductId, int NewQuantity);

public class UpdateCartEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/cart", async (UpdateCartRequest request, ClaimsPrincipal user, IMediator mediator) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Results.Unauthorized();
            }

            var command = new UpdateCartCommand(userId, request.ProductId, request.NewQuantity);
            var result = await mediator.Send(command);

            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
        })
        .WithName("UpdateCart")
        .WithTags("Cart")
        .RequireAuthorization()
        .Produces<UpdateCartResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}
