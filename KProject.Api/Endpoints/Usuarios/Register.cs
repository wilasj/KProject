using KProject.Api.Extensions;
using KProject.Application.Interfaces;
using KProject.Application.Usuarios.Register;

namespace KProject.Api.Endpoints.Usuarios;

public class Register: IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/register", async (
            RegisterRequest registerRequest,
            ICommandHandler<RegisterCommand> handler,
            CancellationToken token) =>
        {
            var command = new RegisterCommand
            {
                Email = registerRequest.Email,
                Password = registerRequest.Password,
                ConviteToken = registerRequest.ConviteToken,
            };

            var result = await handler.Handle(command, token);

            return result.IsFailure ? result.ToHttpResult() : TypedResults.Created();
        });
    }
}