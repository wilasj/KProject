namespace KProject.Application.Produto.ListaProdutos;

public record ProdutoResponse(int Id, string Nome, string Referencia, string Descricao, string CodigoAnvisa, DateTime CriadoEm);