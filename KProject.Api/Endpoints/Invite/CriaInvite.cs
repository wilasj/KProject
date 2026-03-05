using System.Security.Claims;
using KProject.Api.Extensions;
using KProject.Application.Interfaces;
using KProject.Application.Invite.CriaInvite;

namespace KProject.Api.Endpoints.Invite;

public class CriaInvite : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/invites", async (
            ClaimsPrincipal user,
            ICommandHandler<CriaInviteCommand, string> handler,
            CancellationToken token) =>
        {
            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await handler.Handle(new CriaInviteCommand { UsuarioId = userId }, token);
            return result.IsFailure ? result.ToHttpResult() : TypedResults.Ok(new { token = result.Value });
        }).RequireAuthorization();
    }
}
