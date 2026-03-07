using Microsoft.AspNetCore.Identity;

namespace KProject.Api.Endpoints.Usuarios;

public class Logout : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/logout", async (
            SignInManager<IdentityUser<int>> signInManager,
            CancellationToken token) =>
        {
            await signInManager.SignOutAsync();
            return TypedResults.Ok();
        }).RequireAuthorization();
    }
}
