using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KProject.Infrastructure.Configurations.Cliente;

public class ClienteConfiguration: IEntityTypeConfiguration<Domain.Cliente.Cliente>
{
    public void Configure(EntityTypeBuilder<Domain.Cliente.Cliente> builder)
    {
        builder.HasKey(c => c.Id);

        builder
            .Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(c => c.Nome);
    }
}