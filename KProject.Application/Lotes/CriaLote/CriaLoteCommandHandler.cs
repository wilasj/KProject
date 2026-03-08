using KProject.Application.Interfaces;
using KProject.Common;
using KProject.Domain.Estoques;
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

        var loteResult = Lote.Criar(command.ProdutoId, command.Numero, command.Validade);
        if (loteResult.IsFailure)
            return Result.Failure<int>(loteResult.Errors);

        await using var tx = await db.Database.BeginTransactionAsync(token);

        var lote = loteResult.Value;
        db.Lotes.Add(lote);
        await db.SaveChangesAsync(token);

        var estoqueResult = Estoque.Criar(lote.Id, command.QuantidadeInicial);
        if (estoqueResult.IsFailure)
            return Result.Failure<int>(estoqueResult.Errors);

        db.Estoques.Add(estoqueResult.Value);
        await db.SaveChangesAsync(token);

        await tx.CommitAsync(token);

        return Result.Success(lote.Id);
    }
}
