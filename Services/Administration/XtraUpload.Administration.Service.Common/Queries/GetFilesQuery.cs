using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Get a list of files based on search criteria
    /// </summary>
    public class GetFilesQuery : IRequest<PagingResult<FileItemExtended>>
    {
        public GetFilesQuery(PageSearchModel pageSearch)
        {
            PageSearch = new PageSearchModel(pageSearch); 
        }
        public PageSearchModel PageSearch { get; set; }
    }
}
