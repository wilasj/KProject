using KProject.Domain.Lote;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KProject.Infrastructure.Configurations.LoteConfiguration;

public class LoteConfiguration : IEntityTypeConfiguration<Lote>
{
    public void Configure(EntityTypeBuilder<Lote> builder)
    {
        builder.HasKey(l => l.Id);
        
        builder
            .HasOne<Domain.Produto.Produto>()
            .WithMany()
            .HasForeignKey(l => l.ProdutoId);
        
        builder.Property(l => l.Numero).IsRequired();
        builder.Property(l => l.Validade).IsRequired();
    }
}