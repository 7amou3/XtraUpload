using MediatR;

namespace XtraUpload.StorageManager.Common
{
    /// <summary>
    /// Delete files on disk:
    /// - A request is made to the web api in order to get the list of files to delete.
    /// - Once the files are deleted, another request is made to delete those file from db
    /// </summary>
    public class DeleteFilesCommand: IRequest<DeleteFilesResult>
    {
    }
}
