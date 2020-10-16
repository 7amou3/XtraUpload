using MediatR;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Gets a list of all storage servers
    /// </summary>
    public class GetStorageServersQuery : IRequest<StorageServersResult>
    {
    }
}
