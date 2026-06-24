using FileService.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainFile = FileService.Domain.Entities.File;

namespace FileService.Application.Features.DownloadFile
{
    public record FileDownloadResult(string FileName, string ContentType, byte[] Content);

    public record DownloadFileCommand(Guid FileId) : IRequest<FileDownloadResult>;

    public class DownloadFileCommandHandler : IRequestHandler<DownloadFileCommand, FileDownloadResult>
    {
        private readonly IFileRepository _repository;
        private readonly IFileStorage _fileStorage;

        public DownloadFileCommandHandler(IFileRepository repository, IFileStorage fileStorage)
        {
            _repository = repository;
            _fileStorage = fileStorage;
        }

        public async Task<FileDownloadResult> Handle(DownloadFileCommand request, CancellationToken ct)
        {
            var file = await _repository.GetByIdAsync(request.FileId, ct);

            if (file == null)
            {
                return null;
            }

            using var fileStream = await _fileStorage.GetStreamAsync(file.StoredName, ct);

            if (fileStream == null)
            {
                throw new FileNotFoundException($"Файл {file.StoredName} отсутствует в хранилище.");
            }

            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream, ct);
            var contentBytes = memoryStream.ToArray();

            return new FileDownloadResult(
                file.FileName,
                file.ContentType,
                contentBytes
            );
        }
    }
}
