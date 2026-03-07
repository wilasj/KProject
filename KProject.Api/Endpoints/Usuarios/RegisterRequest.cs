namespace KProject.Api.Endpoints.Usuarios;

public record RegisterRequest(string Email, string Password, string ConviteToken);