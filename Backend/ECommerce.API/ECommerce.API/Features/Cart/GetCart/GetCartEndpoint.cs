using System.Security.Claims;

namespace ECommerce.API.Features.Cart.GetCart;

public class GetCartEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/cart", async (ClaimsPrincipal user, IMediator mediator) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Results.Unauthorized();
            }

            var query = new GetCartQuery(userId);
            var result = await mediator.Send(query);

            return Results.Ok(result);
        })
        .WithName("GetCart")
        .WithTags("Cart")
        .RequireAuthorization()
        .Produces<GetCartResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}
