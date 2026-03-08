using KProject.Application.Interfaces;

namespace KProject.Application.Lotes.CriaLote;

public class CriaLoteCommand : ICommand
{
    public required int ProdutoId { get; init; }
    public required int Numero { get; init; }
    public required DateOnly Validade { get; init; }
    public uint QuantidadeInicial { get; init; }
}
