namespace KProject.Api.Endpoints.Usuario;

public class Me: IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/me", () => Task.FromResult(TypedResults.Ok())).RequireAuthorization();
    }
}