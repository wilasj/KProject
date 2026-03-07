using KProject.Domain.Clientes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KProject.Infrastructure.Clientes;

public class ClienteConfiguration: IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.HasKey(c => c.Id);

        builder
            .Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(c => c.Nome);
    }
}