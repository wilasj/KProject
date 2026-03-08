using KProject.Application.Interfaces;
using KProject.Application.Interfaces.Produtos;
using KProject.Common;
using KProject.Domain.Produtos;

namespace KProject.Application.Produtos.CriaProduto;

public class CriaProdutoCommandHandler(IProdutoRepository produtoRepository, IUnitOfWork unitOfWork): ICommandHandler<CriaProdutoCommand, int>
{
    public async Task<Result<int>> Handle(CriaProdutoCommand command, CancellationToken token)
    {
        var result = Produto.Criar(command.Nome, 
            command.Referencia, 
            command.Descricao, 
            command.CodigoAnvisa);

        if (result.IsFailure)
        {
            return Result.Failure<int>(result.Errors);
        }
        
        await produtoRepository.AddAsync(result.Value, token);
        await unitOfWork.SaveChangesAsync(token);

        return Result.Success(result.Value.Id);
    }
}