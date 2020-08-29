using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service.Common
{
    public class CreateUserAccountResult: OperationResult
    {
        public User NewUser { get; set; }
    }
}
