namespace KProject.Api.Endpoints.Usuario;

public class Me: IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/me", (HttpContext httpContext) =>
            TypedResults.Ok(new { Email = httpContext.User.Identity!.Name })
        ).RequireAuthorization();
    }
}