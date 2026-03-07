using System.Security.Claims;
using KProject.Api.Extensions;
using KProject.Application.Convites.CriaConvite;
using KProject.Application.Interfaces;

namespace KProject.Api.Endpoints.Convites;

public class CriaConvite : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/convites", async (
            ClaimsPrincipal user,
            ICommandHandler<CriaConviteCommand, string> handler,
            CancellationToken token) =>
        {
            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await handler.Handle(new CriaConviteCommand { UsuarioId = userId }, token);
            return result.IsFailure ? result.ToHttpResult() : TypedResults.Ok(new { token = result.Value });
        }).RequireAuthorization();
    }
}
