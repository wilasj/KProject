using KProject.Application.Interfaces;

namespace KProject.Application.Vendas.CancelaVenda;

public class CancelaVendaCommand : ICommand
{
    public required int VendaId { get; init; }
    public required int CanceladoPor { get; init; }
}
