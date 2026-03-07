using KProject.Common;

namespace KProject.Domain.Convites;

public sealed class Convite
{
    public int Id { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public int CriadoPorId { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public DateTime? UsadoEm { get; private set; }

    public static Convite Criar(int criadoPorId) => new()
    {
        Token = Guid.NewGuid().ToString("N"),
        CriadoPorId = criadoPorId,
        CriadoEm = DateTime.UtcNow,
    };

    public Result Usar()
    {
        if (!Disponivel)
            return Result.Failure(Error.Failure("Convite.JaUtilizado", "Esse convite já foi utilizado."));
        
        UsadoEm = DateTime.UtcNow;

        return Result.Success();
    }

    public bool Disponivel => UsadoEm is null;
}
