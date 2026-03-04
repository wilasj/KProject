using KProject.Application.Shared;

namespace KProject.Application.Produto.ListaProdutos;

public class ProdutoSpecification : Specification<Domain.Produto.Produto>
{
    public ProdutoSpecification(string? busca)
    {
        //nao usamos a comparacao usando StringComparison porque isso aqui vai ser traduzido pra expressoes do EF,
        //e ele nao sabe traduzir isso
        if (!string.IsNullOrEmpty(busca))
            Criteria = p => p.Nome.ToLower().Contains(busca.ToLower());

        OrderBy = p => p.Nome;
    }
}
