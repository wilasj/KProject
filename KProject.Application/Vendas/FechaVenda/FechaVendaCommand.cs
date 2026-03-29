using KProject.Application.Interfaces;

namespace KProject.Application.Vendas.FechaVenda;

public class FechaVendaCommand : ICommand
{
    public required int VendaId { get; init; }
    public required int FechadoPor { get; init; }
}
