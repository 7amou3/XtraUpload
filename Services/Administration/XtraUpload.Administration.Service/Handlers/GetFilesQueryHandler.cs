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
    /// Get a list of files based on search criteria
    /// </summary>
    public class GetFilesQueryHandler : IRequestHandler<GetFilesQuery, PagingResult<FileItemExtended>>
    {
        readonly IUnitOfWork _unitOfWork;
        public GetFilesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagingResult<FileItemExtended>> Handle(GetFilesQuery request, CancellationToken cancellationToken)
        {
            PagingResult<FileItemExtended> Result = new PagingResult<FileItemExtended>();

            Expression<Func<FileItem, bool>> criteria = s => true;

            if (request.PageSearch.Start != null && request.PageSearch.End != null)
            {
                criteria = criteria.And(s => s.CreatedAt > request.PageSearch.Start && s.CreatedAt < request.PageSearch.End);
            }
            if (request.PageSearch.UserId != null && request.PageSearch.UserId != Guid.Empty)
            {
                criteria = criteria.And(s => s.UserId == request.PageSearch.UserId.ToString());
            }
            if (request.PageSearch.FileExtension != null)
            {
                criteria = criteria.And(s => s.Extension == request.PageSearch.FileExtension);
            }

            Result.TotalItems = await _unitOfWork.Files.CountAsync(criteria);
            Result.Items = await _unitOfWork.Files.GetFiles(request.PageSearch, criteria);

            return Result;
        }
    }
}
