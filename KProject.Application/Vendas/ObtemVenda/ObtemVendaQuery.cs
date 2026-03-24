using KProject.Application.Interfaces;

namespace KProject.Application.Vendas.ObtemVenda;

public record ObtemVendaQuery(int VendaId) : IQuery<VendaDetalheResponse>;
