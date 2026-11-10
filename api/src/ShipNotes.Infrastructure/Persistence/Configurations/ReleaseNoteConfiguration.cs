using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipNotes.Domain.ReleaseNotes;
using ShipNotes.Domain.Versioning;

namespace ShipNotes.Infrastructure.Persistence.Configurations;

internal sealed class ReleaseNoteConfiguration : IEntityTypeConfiguration<ReleaseNote>
{
    public void Configure(EntityTypeBuilder<ReleaseNote> builder)
    {
        builder.ToTable("ReleaseNotes");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).ValueGeneratedNever();

        builder.Property(n => n.Version)
            .HasMaxLength(SemanticVersion.MaxLength)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(n => n.VersionSortKey)
            .HasMaxLength(ReleaseNoteConstraints.SortKeyMaxLength)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(n => n.Title).HasMaxLength(ReleaseNoteConstraints.TitleMaxLength).IsRequired();
        builder.Property(n => n.ContentHtml).IsRequired();
        builder.Ignore(n => n.IsPublished);

        // One note per version (precedence ignores build metadata) and fast newest-first queries.
        builder.HasIndex(n => new { n.ApplicationId, n.VersionSortKey }).IsUnique();
        builder.HasIndex(n => new { n.ApplicationId, n.PublishedAt, n.VersionSortKey });
    }
}
