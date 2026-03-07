using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KProject.Infrastructure.Convite;

public class ConviteCOnfiguration : IEntityTypeConfiguration<Domain.Convite.Convite>
{
    public void Configure(EntityTypeBuilder<Domain.Convite.Convite> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Token)
            .IsRequired()
            .HasMaxLength(32);

        builder.HasIndex(i => i.Token).IsUnique();

        builder.Property(i => i.CriadoPorId).IsRequired();
        builder.Property(i => i.CriadoEm).IsRequired();
    }
}
