using MediatR;

namespace XtraUpload.FileManager.Service.Common
{
    /// <summary>
    /// Get download option of the requested profil
    /// </summary>
    public class GetDownloadOptionsQuery : IRequest<DownloadOptionsResult>
    {
        public GetDownloadOptionsQuery(bool authenticatedUser)
        {
            AuthenticatedUser = authenticatedUser;
        }
        public bool AuthenticatedUser { get; }
    }
}
