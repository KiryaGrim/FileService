using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainFile = FileService.Domain.Entities.File;

namespace FileService.Infrastructure.Persistence.Configurations
{
    public class FileConfiguration : IEntityTypeConfiguration<DomainFile>
    {
        public void Configure(EntityTypeBuilder<DomainFile> builder)
        {
            builder.ToTable("files");

            builder.HasKey(f => f.Id);
            builder.Property(f => f.Id).HasColumnName("id");

            builder.Property(f => f.CourseId)
                .HasColumnName("course_id")
                .IsRequired();

            builder.Property(f => f.FileName)
                .HasColumnName("file_name")
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(f => f.StoredName)
                .HasColumnName("stored_name")
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(f => f.ContentType)
                .HasColumnName("content_type")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(f => f.SizeBytes)
                .HasColumnName("size_bytes")
                .IsRequired();

            builder.Property(f => f.FileType)
                .HasColumnName("file_type")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(f => f.UploadedById)
                .HasColumnName("uploaded_by_id")
                .IsRequired();

            builder.HasIndex(f => f.StoredName)
                .IsUnique();

            builder.HasIndex(f => f.CourseId)
                .HasDatabaseName("idx_files_course_id");

            builder.HasIndex(f => f.UploadedById)
                .HasDatabaseName("idx_files_uploaded_by");
        }
    }
}
