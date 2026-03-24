using KProject.Domain.Vendas;

namespace KProject.Application.Vendas.ObtemVenda;

public record VendaDetalheResponse(
    int Id,
    StatusVenda Status,
    DateTime CriadaEm,
    string CriadaPor,
    DateTime? ModificadaEm,
    string ClienteNome,
    uint TotalConsignado,
    uint TotalVendido,
    uint TotalDevolvido,
    List<ItemDetalheResponse> Itens);

public record ItemDetalheResponse(
    int Id,
    string ProdutoNome,
    int LoteNumero,
    string PacienteNome,
    uint QuantidadeConsignada,
    uint Vendido,
    uint Devolvido,
    uint EmAberto);
