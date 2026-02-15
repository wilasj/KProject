using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KProject.Infrastructure.Lote;

public class LoteConfiguration : IEntityTypeConfiguration<Domain.Lote.Lote>
{
    public void Configure(EntityTypeBuilder<Domain.Lote.Lote> builder)
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