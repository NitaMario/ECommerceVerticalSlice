namespace ECommerce.API.Features.Identity.RegisterUser;

public class RegisterUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/register", async (RegisterUserCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);
        })
        .WithName("RegisterUser")
        .Produces<RegisterUserResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithTags("Identity");
    }
}
