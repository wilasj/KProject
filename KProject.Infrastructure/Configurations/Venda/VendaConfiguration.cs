using KProject.Domain.Venda;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KProject.Infrastructure.Configurations.Venda
{
    public class VendaConfiguration : IEntityTypeConfiguration<Domain.Venda.Venda>
    {
        public void Configure(EntityTypeBuilder<Domain.Venda.Venda> builder)
        {
            builder.HasKey(v => v.Id);

            builder
                .Property(v => v.ClienteId)
                .IsRequired();
            
            builder
                .Property(v => v.CriadaPor)
                .IsRequired();

            builder
                .HasOne<IdentityUser<int>>()
                .WithMany()
                .HasForeignKey(v => v.CriadaPor)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Property(v => v.CriadaEm)
                .IsRequired();

            builder.Property(v => v.Status)
                .HasConversion<string>()
                .IsRequired();

            builder
                .HasMany(i => i.Itens)
                .WithOne()
                .HasForeignKey(i => i.VendaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Navigation(i => i.Itens)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
            
            builder.HasOne<Domain.Cliente.Cliente>()
                .WithMany()
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasIndex(v => v.ClienteId);
            builder.HasIndex(v => v.Status);
            builder.HasIndex(v => v.CriadaEm);
        }
    } 
}

