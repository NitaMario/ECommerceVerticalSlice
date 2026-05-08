namespace ECommerce.API.Features.Catalog.GetProducts;

public class GetProductsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetProductsQuery());

            return Results.Ok(result);
        })
        .WithName("GetProducts")
        .Produces<GetProductsResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithTags("Catalog");
    }
}
