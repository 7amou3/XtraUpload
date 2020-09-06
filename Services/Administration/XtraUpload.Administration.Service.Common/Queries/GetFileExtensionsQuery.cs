using MediatR;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Get available file extensions
    /// </summary>
    public class GetFileExtensionsQuery : IRequest<FileExtensionsResult>
    {
    }
}
