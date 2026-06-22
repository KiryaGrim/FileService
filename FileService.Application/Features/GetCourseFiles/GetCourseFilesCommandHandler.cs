using FileService.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Application.Features.GetCourseFiles
{
    public record FileDto(Guid Id, string FileName, string ContentType, long SizeBytes, string FileType, Guid UploadedById);

    public record GetCourseFilesQuery(Guid CourseId) : IRequest<List<FileDto>>;

    public class GetCourseFilesCommandHandler : IRequestHandler<GetCourseFilesQuery, List<FileDto>>
    {
        private readonly IFileRepository _repository;

        public GetCourseFilesCommandHandler(IFileRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<FileDto>> Handle(GetCourseFilesQuery request, CancellationToken cancellationToken)
        {
            var files = await _repository.GetByCourseIdAsync(request.CourseId, cancellationToken);

            return files.Select(f => new FileDto(
                f.Id,
                f.FileName,
                f.ContentType,
                f.SizeBytes,
                f.FileType,
                f.UploadedById
            )).ToList();
        }
    }
}
