using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KProject.Infrastructure.Configurations.Produto;

public class ProdutoConfiguration: IEntityTypeConfiguration<Domain.Produto.Produto>
{
    public void Configure(EntityTypeBuilder<Domain.Produto.Produto> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Codigo).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Descricao).HasMaxLength(300).IsRequired();
        builder.Property(p => p.CodigoAnvisa).HasMaxLength(100).IsRequired();
        
        builder.Property(p => p.CriadoEm).IsRequired();
        
        builder.HasIndex(p => p.Codigo);
        builder.HasIndex(p => p.CriadoEm);
    }
}