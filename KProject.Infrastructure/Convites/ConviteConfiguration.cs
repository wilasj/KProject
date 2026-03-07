using KProject.Domain.Convites;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KProject.Infrastructure.Convites;

public class ConviteCOnfiguration : IEntityTypeConfiguration<Convite>
{
    public void Configure(EntityTypeBuilder<Convite> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Token)
            .IsRequired()
            .HasMaxLength(32);

        builder.HasIndex(i => i.Token).IsUnique();

        builder.Property(i => i.CriadoPorId).IsRequired();
        builder.Property(i => i.CriadoEm).IsRequired();
        
        builder.HasOne<IdentityUser<int>>()
            .WithMany()
            .HasForeignKey(i => i.CriadoPorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
