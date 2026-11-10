using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipNotes.Domain.Applications;
using ShipNotes.Domain.Media;
using ShipNotes.Domain.ReleaseNotes;

namespace ShipNotes.Infrastructure.Persistence.Configurations;

internal sealed class MediaDataConfiguration : IEntityTypeConfiguration<MediaData>
{
    public void Configure(EntityTypeBuilder<MediaData> builder)
    {
        builder.ToTable("MediaData");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedNever();

        builder.Property(m => m.FileName).HasMaxLength(MediaConstraints.FileNameMaxLength).IsRequired();
        builder.Property(m => m.ContentType)
            .HasMaxLength(MediaConstraints.ContentTypeMaxLength)
            .IsUnicode(false)
            .IsRequired();
        builder.Property(m => m.StoragePath)
            .HasMaxLength(MediaConstraints.StoragePathMaxLength)
            .IsRequired();

        // Deleting an application removes its media rows. Release notes detach their media in code first,
        // because SQL Server does not allow a second cascade path through ReleaseNotes.
        builder.HasOne<ClientApplication>()
            .WithMany()
            .HasForeignKey(m => m.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ReleaseNote>()
            .WithMany()
            .HasForeignKey(m => m.ReleaseNoteId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(m => m.ReleaseNoteId);
        builder.HasIndex(m => m.ApplicationId);
    }
}
