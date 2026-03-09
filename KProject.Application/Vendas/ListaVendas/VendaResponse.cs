using KProject.Domain.Vendas;

namespace KProject.Application.Vendas.ListaVendas;

public record VendaResponse(int Id, string ClienteNome, DateTime CriadaEm, StatusVenda Status, int TotalItens);
