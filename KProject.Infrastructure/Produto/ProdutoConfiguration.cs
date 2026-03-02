using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KProject.Infrastructure.Produto;

public class ProdutoConfiguration: IEntityTypeConfiguration<Domain.Produto.Produto>
{
    public void Configure(EntityTypeBuilder<Domain.Produto.Produto> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Nome).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Referencia).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Descricao).HasMaxLength(300).IsRequired();
        builder.Property(p => p.CodigoAnvisa).HasMaxLength(100).IsRequired();
        
        builder.Property(p => p.CriadoEm).IsRequired();
        builder.HasIndex(p => p.Nome);
        builder.HasIndex(p => p.Referencia);
        builder.HasIndex(p => p.CriadoEm);
    }
}