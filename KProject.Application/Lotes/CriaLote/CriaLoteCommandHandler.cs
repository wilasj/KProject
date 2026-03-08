using KProject.Application.Interfaces;
using KProject.Common;
using KProject.Domain.Lotes;
using KProject.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace KProject.Application.Lotes.CriaLote;

public class CriaLoteCommandHandler(AppDbContext db) : ICommandHandler<CriaLoteCommand, int>
{
    public async Task<Result<int>> Handle(CriaLoteCommand command, CancellationToken token)
    {
        var produtoExiste = await db.Produtos
            .AnyAsync(p => p.Id == command.ProdutoId, token);

        if (!produtoExiste)
            return Result.Failure<int>(
                Error.NotFound("Produto.NaoEncontrado", $"Produto com ID {command.ProdutoId} não encontrado"));

        var loteResult = Lote.Criar(command.ProdutoId, command.Numero, command.Validade, command.QuantidadeInicial);
        if (loteResult.IsFailure)
            return Result.Failure<int>(loteResult.Errors);

        db.Lotes.Add(loteResult.Value);
        await db.SaveChangesAsync(token);

        return Result.Success(loteResult.Value.Id);
    }
}
