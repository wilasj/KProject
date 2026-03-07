namespace KProject.Api.Endpoints.Produtos;

public record ListaProdutosRequest(string? Busca, int? Pagina, int? Tamanho);
