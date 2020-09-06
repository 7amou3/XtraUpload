using MediatR;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Search for users by name
    /// </summary>
    public class SearchUsersQuery : IRequest<SearchUserResult>
    {
        public SearchUsersQuery(string name)
        {
            Name = name;
        }
        public string Name { get; }
    }
}
