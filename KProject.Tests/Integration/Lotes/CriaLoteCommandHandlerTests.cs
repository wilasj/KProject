using KProject.Application.Interfaces;
using KProject.Application.Lotes.CriaLote;
using KProject.Common;
using Microsoft.EntityFrameworkCore;
using KProject.Domain.Produtos;
using KProject.Infrastructure.Shared;
using KProject.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace KProject.Tests.Integration.Lotes;

[Collection(nameof(DatabaseCollection))]
public class CriaLoteCommandHandlerTests(DatabaseFixture fixture)
{
    private async Task<Result<int>> Handle(CriaLoteCommand command)
    {
        await using var scope = fixture.Factory.Services.CreateAsyncScope();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CriaLoteCommand, int>>();
        return await handler.Handle(command, TestContext.Current.CancellationToken);
    }

    private async Task<int> SeedProduto(string nome = "Produto Teste")
    {
        int id = 0;
        await fixture.ExecuteDbContext(async db =>
        {
            var produto = Produto.Criar(nome, $"REF-{nome}", "Descricao", "ANVISA-001").Value;
            db.Produtos.Add(produto);
            await db.SaveChangesAsync();
            id = produto.Id;
        });
        return id;
    }

    [Fact]
    public async Task CriaLote_Valido_RetornaIdEPersisteLoteEEstoque()
    {
        var produtoId = await SeedProduto("Produto CriaLote A");
        var validade = new DateOnly(2027, 6, 1);

        var result = await Handle(new CriaLoteCommand
        {
            ProdutoId = produtoId,
            Numero = 1,
            Validade = validade,
            QuantidadeInicial = 50
        });

        result.IsSuccess.ShouldBeTrue();

        await fixture.ExecuteDbContext(async db =>
        {
            var lote = await db.Lotes.FindAsync(result.Value);
            lote.ShouldNotBeNull();
            lote.Numero.ShouldBe(1);
            lote.Validade.ShouldBe(validade);
            lote.ProdutoId.ShouldBe(produtoId);

            var estoque = await db.Estoques.FirstOrDefaultAsync(e => e.LoteId == lote.Id);
            estoque.ShouldNotBeNull();
            estoque.QuantidadeAtual.ShouldBe(50);
        });
    }

    [Fact]
    public async Task CriaLote_ComQuantidadeZero_CriaEstoqueVazio()
    {
        var produtoId = await SeedProduto("Produto CriaLote B");

        var result = await Handle(new CriaLoteCommand
        {
            ProdutoId = produtoId,
            Numero = 2,
            Validade = new DateOnly(2027, 1, 1),
            QuantidadeInicial = 0
        });

        result.IsSuccess.ShouldBeTrue();

        await fixture.ExecuteDbContext(async db =>
        {
            var estoque = await db.Estoques.FirstOrDefaultAsync(e => e.LoteId == result.Value);
            estoque.ShouldNotBeNull();
            estoque.QuantidadeAtual.ShouldBe(0);
        });
    }

    [Fact]
    public async Task CriaLote_ProdutoNaoEncontrado_RetornaNotFound()
    {
        var result = await Handle(new CriaLoteCommand
        {
            ProdutoId = 999999,
            Numero = 1,
            Validade = new DateOnly(2027, 1, 1)
        });

        result.IsFailure.ShouldBeTrue();
        result.Errors.First().Type.ShouldBe(ErrorType.NotFound);
    }

    [Fact]
    public async Task CriaLote_NumeroZero_RetornaValidation()
    {
        var produtoId = await SeedProduto("Produto CriaLote C");

        var result = await Handle(new CriaLoteCommand
        {
            ProdutoId = produtoId,
            Numero = 0,
            Validade = new DateOnly(2027, 1, 1)
        });

        result.IsFailure.ShouldBeTrue();
        result.Errors.First().Type.ShouldBe(ErrorType.Validation);
    }

    [Fact]
    public async Task CriaLote_ValidadeNaoInformada_RetornaValidation()
    {
        var produtoId = await SeedProduto("Produto CriaLote D");

        var result = await Handle(new CriaLoteCommand
        {
            ProdutoId = produtoId,
            Numero = 1,
            Validade = DateOnly.MinValue
        });

        result.IsFailure.ShouldBeTrue();
        result.Errors.First().Type.ShouldBe(ErrorType.Validation);
    }
}
