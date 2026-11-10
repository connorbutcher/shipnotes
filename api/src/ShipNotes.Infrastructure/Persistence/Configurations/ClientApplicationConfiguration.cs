using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipNotes.Domain.Applications;
using ShipNotes.Domain.ReleaseNotes;

namespace ShipNotes.Infrastructure.Persistence.Configurations;

internal sealed class ClientApplicationConfiguration : IEntityTypeConfiguration<ClientApplication>
{
    public void Configure(EntityTypeBuilder<ClientApplication> builder)
    {
        builder.ToTable("Applications");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.Name).HasMaxLength(ApplicationConstraints.NameMaxLength).IsRequired();
        builder.HasIndex(a => a.Name).IsUnique();

        builder.Property(a => a.IconUrl).HasMaxLength(ApplicationConstraints.IconUrlMaxLength);

        builder.Property(a => a.ApiKeyHash)
            .HasMaxLength(ApplicationConstraints.ApiKeyHashLength)
            .IsUnicode(false)
            .IsFixedLength()
            .IsRequired();
        builder.HasIndex(a => a.ApiKeyHash).IsUnique();

        builder.Property(a => a.ApiKeyPrefix)
            .HasMaxLength(ApplicationConstraints.ApiKeyPrefixLength)
            .IsUnicode(false)
            .IsRequired();

        builder.HasMany<ReleaseNote>()
            .WithOne()
            .HasForeignKey(n => n.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
