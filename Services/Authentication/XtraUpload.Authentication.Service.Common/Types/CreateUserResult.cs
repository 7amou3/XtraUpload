using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service.Common
{
    public class CreateUserResult: OperationResult
    {
        public User User { get; set; }
    }
}
