using KProject.Application.Interfaces;

namespace KProject.Application.Vendas.CriaVenda;

public class CriaVendaCommand : ICommand
{
    public required int ClienteId { get; init; }
    public required int CriadaPor { get; init; }
    public required IReadOnlyList<NovoItemDto> Itens { get; init; }
}

public record NovoItemDto(int LoteId, string PacienteNome, uint Quantidade);
