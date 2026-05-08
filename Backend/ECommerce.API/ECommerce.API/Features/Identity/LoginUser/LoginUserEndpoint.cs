
namespace ECommerce.API.Features.Identity.LoginUser;

public class LoginUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/login", async (LoginUserCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);

            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }

            return Results.Ok(result);
        })
        .WithName("LoginUser")
        .Produces<LoginUserResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithTags("Identity");
    }
}
