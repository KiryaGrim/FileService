using FileService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = FileService.Domain.Entities.File;

namespace FileService.Application.Interfaces
{
    public interface IFileRepository
    {
        Task AddAsync(File file, CancellationToken cancellationToken);
        Task UpdateAsync(File File, CancellationToken cancellationToken);
        Task DeleteAsync(File file, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task AddTaskSolutionAsync(TaskSolutionFile taskSolutionFile, CancellationToken cancellationToken);
        Task<File?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<File>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken);
        Task<File?> GetTaskSolutionFileAsync(Guid taskId, Guid internId, CancellationToken ct);
    }
}
