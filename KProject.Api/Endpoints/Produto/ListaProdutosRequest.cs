namespace KProject.Api.Endpoints.Produto;

public record ListaProdutosRequest(string? Busca, int? Pagina, int? Tamanho);
