using KProject.Application.Interfaces;
using KProject.Common;
using KProject.Domain.Produtos;
using KProject.Infrastructure.Shared;

namespace KProject.Application.Produtos.CriaProduto;

public class CriaProdutoCommandHandler(AppDbContext context): ICommandHandler<CriaProdutoCommand, int>
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
        
        var produto = await context.Produtos.AddAsync(result.Value, token);
        
        await context.SaveChangesAsync(token);
        
        return Result.Success(produto.Entity.Id);       
    }
}