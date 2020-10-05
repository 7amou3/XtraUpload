using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Protos;
using XtraUpload.StorageManager.Common;

namespace XtraUpload.StorageManager.Service
{
    /// <summary>
    /// Delete files on disk:
    /// - A request is made to the web api in order to get the list of files to delete.
    /// - Once the files are deleted, another request is made to delete those file from db
    /// </summary>
    public class DeleteFilesCommandHandler : IRequestHandler<DeleteFilesCommand, DeleteFilesResult>
    {
        readonly UploadOptions _uploadOpt;
        readonly gFileStorage.gFileStorageClient _storageClient;

        public DeleteFilesCommandHandler(IOptionsMonitor<UploadOptions> uploadOpt, gFileStorage.gFileStorageClient storageClient)
        {
            _uploadOpt = uploadOpt.CurrentValue;
            _storageClient = storageClient;
        }

        public async Task<DeleteFilesResult> Handle(DeleteFilesCommand request, CancellationToken cancellationToken)
        {
            DeleteFilesResult result = new DeleteFilesResult();

            var response = await _storageClient.GetFilesToDeleteAsync(new gGetFilesToDeleteRequest());

            if (response == null)
            {
                result.ErrorContent = new Domain.ErrorContent("Unknown error occured while waiting for files list", Domain.ErrorOrigin.Server);
                return result;
            }
            if (response.Status.Status != RequestStatus.Success)
            {
                result.ErrorContent = new Domain.ErrorContent(response.Status.Message, Domain.ErrorOrigin.Server);
                return result;
            }

            // Delete files from disk
            foreach (var file in response.FilesItem)
            {
                string folderPath = Path.Combine(_uploadOpt.UploadPath, file.UserId, file.Id);

                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                }
            }

            // Request to delete files from db
            gDeleteFilesRequest delRequest = new gDeleteFilesRequest();
            delRequest.FilesId.Add(response.FilesItem.Select(s => s.Id));
            var deleteResponse = await _storageClient.DeleteFilesFromDbAsync(delRequest);

            if (deleteResponse == null)
            {
                result.ErrorContent = new Domain.ErrorContent("Error occured while removing files from db", Domain.ErrorOrigin.Server);
                return result;
            }
            if (deleteResponse.Status.Status != RequestStatus.Success)
            {
                result.ErrorContent = new Domain.ErrorContent(deleteResponse.Status.Message, Domain.ErrorOrigin.Server);
                return result;
            }

            result.Files = response.FilesItem;
            return result;
        }
    }
}
