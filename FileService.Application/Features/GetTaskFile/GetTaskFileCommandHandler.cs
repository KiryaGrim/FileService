using FileService.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Application.Features.GetTaskFile
{
    public record FileMetadataDto(Guid FileId, string FileName, string FileUrl, string ContentType);

    public record GetTaskFileQuery(Guid TaskId, Guid InternId) : IRequest<FileMetadataDto?>;

    public class GetTaskFileCommandHandler : IRequestHandler<GetTaskFileQuery, FileMetadataDto?>
    {
        private readonly IFileRepository _repository;

        public GetTaskFileCommandHandler(IFileRepository repository)
        {
            _repository = repository;
        }

        public async Task<FileMetadataDto?> Handle(GetTaskFileQuery request, CancellationToken ct)
        {
            var file = await _repository.GetTaskSolutionFileAsync(request.TaskId, request.InternId, ct);

            if (file == null)
            {
                return null;
            }

            return new FileMetadataDto(
                file.Id,
                file.FileName,
                $"/api/files/{file.Id}",
                file.ContentType
            );
        }
    }
}
