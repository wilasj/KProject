namespace KProject.Api.Endpoints.Lotes;

public record CriaLoteRequest(int ProdutoId, int Numero, DateOnly Validade, uint QuantidadeInicial);
