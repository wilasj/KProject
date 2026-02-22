using KProject.Api.Extensions;
using KProject.Application.Interfaces;
using KProject.Application.Usuario.Login;

namespace KProject.Api.Endpoints.Usuario;

public class Login: IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/login", async (
            LoginRequest request,
            ICommandHandler<LoginCommand> handler,
            CancellationToken token
        ) =>
        {
            var command = new LoginCommand()
            {
                Email = request.Email,
                Password = request.Password
            };

            var result = await handler.Handle(command, token);

            return result.IsFailure ? result.ToHttpResult() : TypedResults.Ok();
        });
    }
}