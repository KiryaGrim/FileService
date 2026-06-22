using FileService.Application.Interfaces;
using Grpc.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Application.Features.ReplaceFile
{
    public record ReplaceFileCommand(Guid FileId, string NewFileName, string ContentType, long SizeBytes, Stream NewFileStream, Guid RequestedById) : IRequest<bool>;

    public class ReplaceFileCommandHandler : IRequestHandler<ReplaceFileCommand, bool>
    {
        private readonly IFileRepository _repository;
        private readonly IFileStorage _fileStorage;

        public ReplaceFileCommandHandler(IFileRepository repository, IFileStorage fileStorage)
        {
            _repository = repository;
            _fileStorage = fileStorage;
        }

        public async Task<bool> Handle(ReplaceFileCommand request, CancellationToken cancellationToken)
        {
            var file = await _repository.GetByIdAsync(request.FileId, cancellationToken);
            if (file == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Файл с ID {request.FileId} не найден."));
            }

            if (file.UploadedById != request.RequestedById)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "У вас нет прав на замену этого файла."));
            }

            await _fileStorage.DeleteAsync(file.StoredName, cancellationToken);

            var newStoredName = $"{Guid.NewGuid()}{Path.GetExtension(request.NewFileName)}";

            await _fileStorage.SaveAsync(newStoredName, request.NewFileStream, request.ContentType, cancellationToken);

            file.FileName = request.NewFileName;
            file.StoredName = newStoredName;
            file.ContentType = request.ContentType;
            file.SizeBytes = request.SizeBytes;

            await _repository.UpdateAsync(file, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
