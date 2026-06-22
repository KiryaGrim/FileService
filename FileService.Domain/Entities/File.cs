using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Domain.Entities
{
    public class File
    {
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string StoredName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long SizeBytes { get; set; }

        public string FileType { get; set; } = string.Empty;

        public Guid UploadedById { get; set; }

        public TaskSolutionFile? TaskSolution { get; set; }
    }
}
