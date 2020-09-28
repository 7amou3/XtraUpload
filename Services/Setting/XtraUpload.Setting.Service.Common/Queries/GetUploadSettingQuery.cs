using MediatR;

namespace XtraUpload.Setting.Service.Common
{
    /// <summary>
    /// Get the upload setting for the current user
    /// </summary>
    public class GetUploadSettingQuery: IRequest<UploadSettingResult>
    {
    }
}
