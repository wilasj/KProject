using KProject.Domain.Estoques;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KProject.Infrastructure.Estoques;

public class HistoricoEstoqueConfiguration: IEntityTypeConfiguration<HistoricoEstoque>
{
    public void Configure(EntityTypeBuilder<HistoricoEstoque> builder)
    {
        builder.HasKey(h => h.Id);

        builder
            .Property(h => h.EstoqueId)
            .IsRequired();

        builder
            .HasOne<Estoque>()
            .WithMany(e => e.Historico)
            .HasForeignKey(h => h.EstoqueId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(h => h.DeltaQuantidade).IsRequired();
        builder.Property(h => h.Tipo).HasConversion<string>().IsRequired();
        builder.Property(h => h.CriadoEm).IsRequired();

        builder.HasIndex(h => new { h.EstoqueId, h.CriadoEm });
    }
}