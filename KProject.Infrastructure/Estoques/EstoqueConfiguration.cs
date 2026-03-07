using KProject.Domain.Estoques;
using KProject.Domain.Lotes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KProject.Infrastructure.Estoques;

public class EstoqueConfiguration: IEntityTypeConfiguration<Estoque>
{
    public void Configure(EntityTypeBuilder<Estoque> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.QuantidadeAtual).IsRequired();

        builder
            .HasOne<Lote>()
            .WithOne()
            .HasForeignKey<Estoque>(l => l.LoteId)
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