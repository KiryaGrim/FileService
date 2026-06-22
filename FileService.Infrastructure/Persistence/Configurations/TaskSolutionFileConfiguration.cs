using FileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Infrastructure.Persistence.Configurations
{
    public class TaskSolutionFileConfiguration : IEntityTypeConfiguration<TaskSolutionFile>
    {
        public void Configure(EntityTypeBuilder<TaskSolutionFile> builder)
        {
            builder.ToTable("task_solution_files");

            builder.HasKey(ts => ts.FileId);
            builder.Property(ts => ts.FileId).HasColumnName("file_id");

            builder.Property(ts => ts.TaskId)
                .HasColumnName("task_id")
                .IsRequired();

            builder.Property(ts => ts.InternId)
                .HasColumnName("intern_id")
                .IsRequired();

            builder.HasIndex(ts => new { ts.TaskId, ts.InternId })
                .IsUnique()
                .HasDatabaseName("idx_task_solution_files_lookup");

            builder.HasOne(ts => ts.File)
                .WithOne(f => f.TaskSolution)
                .HasForeignKey<TaskSolutionFile>(ts => ts.FileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
