namespace KProject.Application.Produtos.ListaProdutos;

public record ProdutoResponse(int Id, string Nome, string Referencia, string Descricao, string CodigoAnvisa, DateTime CriadoEm, int TotalLotes);