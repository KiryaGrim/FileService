using FileService.Application.Interfaces;
using FileService.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainFile = FileService.Domain.Entities.File;

namespace FileService.Application.Features.UploadFile
{
    public record UploadFileCommand(Guid CourseId, string FileName, string ContentType, long SizeBytes, string FileType, Stream FileStream, Guid UploadedById, Guid? TaskId = null, Guid? InternId = null) : IRequest<Guid>;

    public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, Guid>
    {
        private readonly IFileRepository _repository;
        private readonly IFileStorage _fileStorage;

        public UploadFileCommandHandler(IFileRepository repository, IFileStorage fileStorage)
        {
            _repository = repository;
            _fileStorage = fileStorage;
        }

        public async Task<Guid> Handle(UploadFileCommand request, CancellationToken ct)
        {
            var storedName = $"{Guid.NewGuid()}{Path.GetExtension(request.FileName)}";

            await _fileStorage.SaveAsync(storedName, request.FileStream, request.ContentType, ct);

            var file = new DomainFile
            {
                Id = Guid.NewGuid(),
                CourseId = request.CourseId,
                FileName = request.FileName,
                StoredName = storedName,
                ContentType = request.ContentType,
                SizeBytes = request.SizeBytes,
                FileType = request.FileType,
                UploadedById = request.UploadedById
            };

            await _repository.AddAsync(file, ct);

            if (request.FileType == "TaskSolution" && request.TaskId.HasValue && request.InternId.HasValue)
            {
                var taskSolution = new TaskSolutionFile
                {
                    FileId = file.Id,
                    TaskId = request.TaskId.Value,
                    InternId = request.InternId.Value
                };
                await _repository.AddTaskSolutionAsync(taskSolution, ct);
            }

            await _repository.SaveChangesAsync(ct);

            return file.Id;
        }
    }
}
