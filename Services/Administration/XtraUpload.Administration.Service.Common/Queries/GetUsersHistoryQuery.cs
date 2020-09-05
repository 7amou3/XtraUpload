using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace XtraUpload.Administration.Service.Common
{
    public class GetUsersHistoryQuery : IRequest<IEnumerable<ItemCountResult>>
    {
        public GetUsersHistoryQuery(DateRangeModel range)
        {
            Range = new DateRangeModel(range.Start, range.End);
        }
        public DateRangeModel Range { get; }
    }
}
