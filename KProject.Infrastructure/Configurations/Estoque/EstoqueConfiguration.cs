using KProject.Domain.Lote;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KProject.Infrastructure.Configurations.Estoque;

public class EstoqueConfiguration: IEntityTypeConfiguration<Domain.Estoque.Estoque>
{
    public void Configure(EntityTypeBuilder<Domain.Estoque.Estoque> builder)
    {
        builder.HasKey(e => e.Id);

        builder
            .HasOne<Lote>()
            .WithOne()
            .HasForeignKey<Domain.Estoque.Estoque>(l => l.LoteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(e => e.Historico)
            .WithOne()
            .HasForeignKey(e => e.EstoqueId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .Navigation(e => e.Historico)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder
            .HasIndex(e => e.LoteId)
            .IsUnique();
    }
}