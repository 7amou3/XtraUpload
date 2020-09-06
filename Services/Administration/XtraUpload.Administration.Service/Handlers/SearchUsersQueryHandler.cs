using MediatR;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Search for users by name
    /// </summary>
    public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, SearchUserResult>
    {
        readonly IUnitOfWork _unitOfWork;
        public SearchUsersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SearchUserResult> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
        {
            SearchUserResult result = new SearchUserResult()
            {
                Users = await _unitOfWork.Users.SearchUsersByName(request.Name)
            };

            return result;
        }
    }
}
