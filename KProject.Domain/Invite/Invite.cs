namespace KProject.Domain.Invite;

public sealed class Invite
{
    public int Id { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public int CriadoPorId { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public DateTime? UsadoEm { get; private set; }

    public static Invite Criar(int criadoPorId) => new()
    {
        Token = Guid.NewGuid().ToString("N"),
        CriadoPorId = criadoPorId,
        CriadoEm = DateTime.UtcNow,
    };

    public void Usar() => UsadoEm = DateTime.UtcNow;

    public bool Disponivel => UsadoEm is null;
}
