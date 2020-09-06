using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Get a list of users based on the provided search criteria
    /// </summary>
    public class GetUsersQuery : IRequest<PagingResult<UserExtended>>
    {
        public GetUsersQuery(PageSearchModel pageSearch)
        {
            PageSearch = new PageSearchModel(pageSearch);
        }
        public PageSearchModel PageSearch { get; }
    }
}
