using KProject.Domain.Lotes;
using KProject.Domain.Vendas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KProject.Infrastructure.Vendas;

public class ItemConsignadoConfiguration: IEntityTypeConfiguration<ItemConsignado>
{
    public void Configure(EntityTypeBuilder<ItemConsignado> builder)
    {
        builder.HasKey(i => i.Id);

        builder
            .Property(i => i.LoteId)
            .IsRequired();
        
        builder
            .HasOne<Lote>()
            .WithMany()
            .HasForeignKey(i => i.LoteId);

        builder
            .Property(i => i.QuantidadeConsignada)
            .IsRequired();

        builder
            .HasMany(i => i.Historico)
            .WithOne()
            .HasForeignKey(i => i.ItemConsignadoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Navigation(i => i.Historico)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder
            .HasIndex("VendaId", nameof(ItemConsignado.LoteId))
            .IsUnique();
    }
}