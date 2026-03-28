namespace KProject.Api.Endpoints.Vendas;

public record EditaVendaRequest(IReadOnlyList<EditaItemRequest> Itens);
public record EditaItemRequest(int Id, uint Vendido, uint Devolvido);
