using KProject.Application.Interfaces;

namespace KProject.Application.Vendas.EditaVenda;

public class EditaVendaCommand : ICommand
{
    public required int VendaId { get; init; }
    public required int AlteradoPor { get; init; }
    public required IReadOnlyList<EditaItemDto> Itens { get; init; }
}

public record EditaItemDto(int Id, uint Vendido, uint Devolvido);
