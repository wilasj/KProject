namespace KProject.Api.Endpoints.Clientes;

public record ListaClientesRequest(string? Busca, int? Pagina, int? Tamanho);
