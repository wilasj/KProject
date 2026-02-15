using KProject.Domain.Estoque;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KProject.Infrastructure.Estoque;

public class HistoricoEstoqueConfiguration: IEntityTypeConfiguration<HistoricoEstoque>
{
    public void Configure(EntityTypeBuilder<HistoricoEstoque> builder)
    {
        builder.HasKey(h => h.Id);

        builder
            .Property(h => h.EstoqueId)
            .IsRequired();

        builder
            .HasOne<Domain.Estoque.Estoque>()
            .WithMany(e => e.Historico)
            .HasForeignKey(h => h.EstoqueId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(h => h.DeltaQuantidade).IsRequired();
        builder.Property(h => h.Tipo).HasConversion<string>().IsRequired();
        builder.Property(h => h.CriadoEm).IsRequired();

        builder.HasIndex(h => new { h.EstoqueId, h.CriadoEm });
    }
}