using FileService.Application.Features.DeleteFile;
using FileService.Application.Features.GetCourseFiles;
using FileService.Application.Features.GetTaskFile;
using FileService.Application.Features.ReplaceFile;
using FileService.Application.Features.UploadFile;
using FileService.Presentation.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using File = FileService.Presentation.Protos.File;

namespace FileService.Presentation.Services
{
    public class FileGrpcService : File.FileBase
    {
        private readonly IMediator _mediator;

        public FileGrpcService(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        public override async Task<UploadFileResponse> UploadFile(UploadFileRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.UploadedById, out var uploadedById))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Некорректный формат UploadedById."));
            }

            var fileStream = new MemoryStream(request.Content.ToByteArray());

            var command = new UploadFileCommand(
                CourseId: Guid.Parse(request.TargetId),
                FileName: request.FileName,
                ContentType: request.ContentType,
                SizeBytes: request.Content.Length,
                FileType: request.TargetType,
                FileStream: fileStream,
                UploadedById: uploadedById,
                TaskId: request.TargetType == "TaskSolution" ? Guid.Parse(request.TargetId) : null,
                InternId: request.TargetType == "TaskSolution" ? uploadedById : null
            );

            var fileId = await _mediator.Send(command, context.CancellationToken);

            return new UploadFileResponse
            {
                FileId = fileId.ToString(),
                FileUrl = $"/api/files/{fileId}",
                Success = true
            };
        }

        [Authorize]
        public override async Task<ReplaceFileResponse> ReplaceFile(ReplaceFileRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.FileId, out var fileId) || !Guid.TryParse(request.RequestedById, out var requestedById))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Некорректный формат ID."));
            }

            var fileStream = new MemoryStream(request.NewContent.ToByteArray());

            var command = new ReplaceFileCommand(
                FileId: fileId,
                NewFileName: request.NewFileName,
                ContentType: request.ContentType,
                SizeBytes: request.NewContent.Length,
                NewFileStream: fileStream,
                RequestedById: requestedById
            );

            var success = await _mediator.Send(command, context.CancellationToken);

            return new ReplaceFileResponse { Success = success };
        }

        [Authorize]
        public override async Task<GetCourseFilesResponse> GetCourseFiles(GetCourseFilesRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.CourseId, out var courseId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Некорректный формат CourseId."));
            }

            var query = new GetCourseFilesQuery(courseId);
            var files = await _mediator.Send(query, context.CancellationToken);

            var response = new GetCourseFilesResponse();
            response.Files.AddRange(files.Select(f => new FileItem
            {
                Id = f.Id.ToString(),
                FileName = f.FileName,
                ContentType = f.ContentType,
                SizeBytes = f.SizeBytes,
                FileType = f.FileType,
                UploadedById = f.UploadedById.ToString()
            }));

            return response;
        }

        [Authorize]
        public override async Task<DeleteFileResponse> DeleteFile(DeleteFileRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.FileId, out var fileId) || !Guid.TryParse(request.RequestedById, out var requestedById))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Некорректный формат ID."));
            }

            var command = new DeleteFileCommand(fileId, requestedById);

            try
            {
                var success = await _mediator.Send(command, context.CancellationToken);
                return new DeleteFileResponse { Success = success };
            }
            catch (Exception ex)
            {
                return new DeleteFileResponse { Success = false, ErrorMessage = ex.Message };
            }
        }

        [Authorize]
        public override async Task<GetTaskFileResponse> GetTaskFile(GetTaskFileRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.TaskId, out var taskId) || !Guid.TryParse(request.InternId, out var internId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Некорректный формат TaskId или InternId."));
            }

            var query = new GetTaskFileQuery(taskId, internId);
            var result = await _mediator.Send(query, context.CancellationToken);

            if (result == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Файл решения не найден."));
            }

            return new GetTaskFileResponse
            {
                FileId = result.FileId.ToString(),
                FileName = result.FileName,
                FileUrl = result.FileUrl,
                ContentType = result.ContentType
            };
        }
    }
}
