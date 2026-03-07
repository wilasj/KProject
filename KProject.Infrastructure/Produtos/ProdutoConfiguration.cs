using KProject.Domain.Produtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KProject.Infrastructure.Produtos;

public class ProdutoConfiguration: IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
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