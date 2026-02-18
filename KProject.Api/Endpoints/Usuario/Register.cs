using KProject.Api.Extensions;
using KProject.Application.Interfaces;
using KProject.Application.User.Register;

namespace KProject.Api.Endpoints.Usuario;

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
                Password = registerRequest.Password
            };

            var result = await handler.Handle(command, token);

            return result.IsFailure ? result.ToHttpResult() : TypedResults.Created();
        });
    }
}