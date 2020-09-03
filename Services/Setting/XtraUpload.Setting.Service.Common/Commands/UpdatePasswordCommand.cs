using MediatR;

namespace XtraUpload.Setting.Service.Common
{
    public class UpdatePasswordCommand : IRequest<UpdatePasswordResult>
    {
        public UpdatePasswordCommand(string oldPassword, string newPassword)
        {
            OldPassword = oldPassword;
            NewPassword = newPassword;
        }
        public string OldPassword { get; }
        public string NewPassword { get; }
    }
}
