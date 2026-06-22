using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileService.Application.Interfaces;
using FileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using File = FileService.Domain.Entities.File;

namespace FileService.Infrastructure.Persistence.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly FileDbContext _context;

        public FileRepository(FileDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(File file, CancellationToken cancellationToken)
        {
            await _context.Files.AddAsync(file, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(File file, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(File file, CancellationToken cancellationToken)
        {
            _context.Files.Remove(file);
            await Task.CompletedTask;
        }

        public async Task AddTaskSolutionAsync(TaskSolutionFile taskSolutionFile, CancellationToken cancellationToken)
        {
            await _context.TaskSolutionFiles.AddAsync(taskSolutionFile, cancellationToken);
        }

        public async Task<File> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Files
                .Include(f => f.TaskSolution)
                .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        }

        public async Task<List<File>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken)
        {
            return await _context.Files
                .Where(f => f.CourseId == courseId)
                .ToListAsync(cancellationToken);
        }

        public async Task<File?> GetTaskSolutionFileAsync(Guid taskId, Guid internId, CancellationToken ct)
        {
            return await _context.Set<File>()
                .Include(f => f.TaskSolution)
                .FirstOrDefaultAsync(f =>
                    f.TaskSolution != null &&
                    f.TaskSolution.TaskId == taskId &&
                    f.TaskSolution.InternId == internId, ct);
        }
    }
}
