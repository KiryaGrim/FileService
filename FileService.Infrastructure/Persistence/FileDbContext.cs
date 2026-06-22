using FileService.Domain.Entities;
using FileService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using File = FileService.Domain.Entities.File;

namespace FileService.Infrastructure.Persistence
{
    public class FileDbContext : DbContext, IUnitOfWork
    {
        public FileDbContext(DbContextOptions<FileDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<File> Files => Set<File>();
        public DbSet<TaskSolutionFile> TaskSolutionFiles => Set<TaskSolutionFile>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FileDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
