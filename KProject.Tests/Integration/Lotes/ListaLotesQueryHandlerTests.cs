using KProject.Application.Lotes.ListaLotes;
using KProject.Common;
using KProject.Domain.Estoques;
using KProject.Domain.Lotes;
using KProject.Domain.Produtos;
using KProject.Infrastructure.Shared;
using KProject.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace KProject.Tests.Integration.Lotes;

[Collection(nameof(DatabaseCollection))]
public class ListaLotesQueryHandlerTests(DatabaseFixture fixture)
{
    private async Task<Result<IReadOnlyList<LoteResponse>>> Handle(ListaLotesQuery query)
    {
        await using var scope = fixture.Factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var handler = new ListaLotesQueryHandler(db);
        return await handler.Handle(query, TestContext.Current.CancellationToken);
    }

    private async Task<(Produto produto, List<Lote> lotes)> SeedProdutoComLotes(
        string nomeProduto,
        params (int numero, DateOnly validade, uint quantidade)[] lotesData)
    {
        Produto produto = null!;
        var lotes = new List<Lote>();

        await fixture.ExecuteDbContext(async db =>
        {
            produto = Produto.Criar(nomeProduto, $"REF-{nomeProduto}", "Descricao", "ANVISA-001").Value;
            db.Produtos.Add(produto);
            await db.SaveChangesAsync();

            foreach (var (numero, validade, quantidade) in lotesData)
            {
                var lote = Lote.Criar(produto.Id, numero, validade).Value;
                db.Lotes.Add(lote);
                await db.SaveChangesAsync();

                var estoque = Estoque.Criar(lote.Id, quantidade).Value;
                db.Estoques.Add(estoque);
                await db.SaveChangesAsync();

                lotes.Add(lote);
            }
        });

        return (produto, lotes);
    }

    [Fact]
    public async Task ListaLotes_RetornaLotesCorretos()
    {
        var (produto, lotes) = await SeedProdutoComLotes("Produto A",
            (101, new DateOnly(2027, 3, 15), 10),
            (102, new DateOnly(2027, 9, 1), 20));

        var result = await Handle(new ListaLotesQuery(produto.Id));

        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(2);
        result.Value.ShouldContain(l => l.Numero == 101 && l.QuantidadeTotal == 10);
        result.Value.ShouldContain(l => l.Numero == 102 && l.QuantidadeTotal == 20);
    }

    [Fact]
    public async Task ListaLotes_OrdenadoPorValidade()
    {
        var (produto, _) = await SeedProdutoComLotes("Produto B",
            (201, new DateOnly(2028, 6, 1), 5),
            (202, new DateOnly(2026, 12, 31), 8),
            (203, new DateOnly(2027, 3, 15), 3));

        var result = await Handle(new ListaLotesQuery(produto.Id));

        result.IsSuccess.ShouldBeTrue();
        var validades = result.Value.Select(l => l.Validade).ToList();
        validades.ShouldBe(validades.OrderBy(v => v).ToList());
    }

    [Fact]
    public async Task ListaLotes_SemLotes_RetornaListaVazia()
    {
        Produto produto = null!;
        await fixture.ExecuteDbContext(async db =>
        {
            produto = Produto.Criar("Produto C", "REF-C", "Descricao", "ANVISA-001").Value;
            db.Produtos.Add(produto);
            await db.SaveChangesAsync();
        });

        var result = await Handle(new ListaLotesQuery(produto.Id));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public async Task ListaLotes_ProdutoNaoEncontrado_RetornaNotFound()
    {
        var result = await Handle(new ListaLotesQuery(999999));

        result.IsFailure.ShouldBeTrue();
        result.Errors.First().Type.ShouldBe(ErrorType.NotFound);
    }
}
