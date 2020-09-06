using MediatR;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Get a list of users based on the provided search criteria
    /// </summary>
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagingResult<UserExtended>>
    {
        readonly IUnitOfWork _unitOfWork;

        public GetUsersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagingResult<UserExtended>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            PagingResult<UserExtended> result = new PagingResult<UserExtended>();
            Expression<Func<User, bool>> criteria = s => true;

            if (request.PageSearch.Start != null && request.PageSearch.End != null)
            {
                criteria = criteria.And(s => s.CreatedAt > request.PageSearch.Start && s.CreatedAt < request.PageSearch.End);
            }
            if (request.PageSearch.UserId != null && request.PageSearch.UserId != Guid.Empty)
            {
                criteria = criteria.And(s => s.Id == request.PageSearch.UserId.ToString());
            }

            result.TotalItems = await _unitOfWork.Users.CountAsync(criteria);
            result.Items = await _unitOfWork.Users.GetUsers(request.PageSearch, criteria);

            return result;
        }
    }
}
