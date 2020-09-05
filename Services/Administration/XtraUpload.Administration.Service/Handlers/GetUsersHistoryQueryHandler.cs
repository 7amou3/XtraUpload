using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;

namespace XtraUpload.Administration.Service
{
    public class GetUsersHistoryQueryHandler : IRequestHandler<GetUsersHistoryQuery, IEnumerable<ItemCountResult>>
    {
        readonly IUnitOfWork _unitOfWork;
        public GetUsersHistoryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<ItemCountResult>> Handle(GetUsersHistoryQuery request, CancellationToken cancellationToken)
        {
            List<ItemCountResult> users = new List<ItemCountResult>(await _unitOfWork.Users.UsersCountByDateRange(request.Range.Start, request.Range.End));

            return AdministrationHelpers.FormatResult(request.Range, users);
        }
    }
}
