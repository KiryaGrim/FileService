using FileService.Application.Interfaces;
using Grpc.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Application.Features.DeleteFile
{
    public record DeleteFileCommand(Guid FileId, Guid RequestedById) : IRequest<bool>;

    public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand, bool>
    {
        private readonly IFileRepository _repository;
        private readonly IFileStorage _fileStorage;

        public DeleteFileCommandHandler(IFileRepository repository, IFileStorage fileStorage)
        {
            _repository = repository;
            _fileStorage = fileStorage;
        }

        public async Task<bool> Handle(DeleteFileCommand request, CancellationToken ct)
        {
            var file = await _repository.GetByIdAsync(request.FileId, ct);
            if (file == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Файл с ID {request.FileId} не найден."));
            }

            await _fileStorage.DeleteAsync(file.StoredName, ct);

            await _repository.DeleteAsync(file, ct);
            await _repository.SaveChangesAsync(ct);

            return true;
        }
    }
}
