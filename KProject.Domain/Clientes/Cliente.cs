namespace KProject.Domain.Clientes;

public sealed class Cliente
{
    public int Id { get; private set; }
    public string Nome { get; private set; }

    private Cliente(string nome)
    {
        Nome = nome;
    }

    public static Cliente Criar(string nome) => new(nome);
}