namespace KProject.Api.Endpoints.Vendas;

public record CriaVendaRequest(int ClienteId, IReadOnlyList<NovoItemRequest> Itens);
public record NovoItemRequest(int LoteId, string PacienteNome, uint Quantidade);
