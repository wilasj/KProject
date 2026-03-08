using KProject.Application.Interfaces;
using KProject.Application.Interfaces.Lotes;
using KProject.Application.Interfaces.Produtos;
using KProject.Common;
using KProject.Domain.Lotes;

namespace KProject.Application.Lotes.CriaLote;

public class CriaLoteCommandHandler(
    IProdutoRepository produtos,
    ILoteRepository lotes,
    IUnitOfWork unitOfWork) : ICommandHandler<CriaLoteCommand, int>
{
    public async Task<Result<int>> Handle(CriaLoteCommand command, CancellationToken token)
    {
        var produtoExiste = await produtos.ExistsAsync(command.ProdutoId, token);

        if (!produtoExiste)
            return Result.Failure<int>(
                Error.NotFound("Produto.NaoEncontrado", $"Produto com ID {command.ProdutoId} não encontrado"));

        var loteResult = Lote.Criar(command.ProdutoId, command.Numero, command.Validade, command.QuantidadeInicial);
        if (loteResult.IsFailure)
            return Result.Failure<int>(loteResult.Errors);

        await lotes.AddAsync(loteResult.Value, token);
        await unitOfWork.SaveChangesAsync(token);

        return Result.Success(loteResult.Value.Id);
    }
}
