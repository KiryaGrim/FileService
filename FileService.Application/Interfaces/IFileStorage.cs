using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Application.Interfaces
{
    public interface IFileStorage
    {
        Task SaveAsync(string storedName, Stream fileStream, string contentType, CancellationToken ct);
        Task DeleteAsync(string storedName, CancellationToken ct);
    }
}
