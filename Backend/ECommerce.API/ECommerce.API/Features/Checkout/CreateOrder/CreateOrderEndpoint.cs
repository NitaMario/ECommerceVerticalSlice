using System.Security.Claims;

namespace ECommerce.API.Features.Checkout.CreateOrder;

public class CreateOrderEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/orders", async (CreateOrderRequest request, ClaimsPrincipal user, IMediator mediator) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Results.Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(request.ShippingAddress))
            {
                return Results.BadRequest(new CreateOrderResult(false, "Shipping address is required."));
            }

            var command = new CreateOrderCommand(userId, request.ShippingAddress);
            var result = await mediator.Send(command);

            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
        })
        .WithName("CreateOrder")
        .WithTags("Orders")
        .RequireAuthorization()
        .Produces<CreateOrderResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}
