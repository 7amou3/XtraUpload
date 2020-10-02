using MediatR;

namespace XtraUpload.Authentication.Service.Common
{
    public class GetUserByIdQuery : IRequest<CreateUserResult>
    {
        public GetUserByIdQuery(string id)
        {
            Id = id;
        }
        public string Id { get; }
    }
}
