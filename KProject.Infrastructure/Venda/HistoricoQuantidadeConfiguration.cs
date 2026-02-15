using KProject.Domain.Venda;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KProject.Infrastructure.Venda;

public class HistoricoQuantidadeConfiguration: IEntityTypeConfiguration<HistoricoQuantidade>
{
    public void Configure(EntityTypeBuilder<HistoricoQuantidade> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(h => h.ItemConsignadoId).IsRequired();

        builder
            .HasOne<ItemConsignado>()
            .WithMany(i => i.Historico)
            .HasForeignKey(h => h.ItemConsignadoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(h => h.AlteradoEm).IsRequired();
        
        builder.Property(h => h.AlteradoPor).IsRequired();

        builder
            .HasOne<IdentityUser<int>>()
            .WithMany()
            .HasForeignKey(h => h.AlteradoPor)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(h => h.Vendido).IsRequired();
        builder.Property(h => h.Devolvido).IsRequired();

        builder.HasIndex(h => new { h.ItemConsignadoId, h.AlteradoEm});
    }
}