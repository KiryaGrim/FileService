using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Domain.Entities
{
    public class TaskSolutionFile
    {
        public Guid FileId { get; set; }

        public Guid TaskId { get; set; }

        public Guid InternId { get; set; }

        public File File { get; set; } = null!;
    }
}
