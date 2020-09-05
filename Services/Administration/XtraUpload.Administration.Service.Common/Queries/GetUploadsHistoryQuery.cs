using MediatR;
using System.Collections.Generic;

namespace XtraUpload.Administration.Service.Common
{
    public class GetUploadsHistoryQuery : IRequest<IEnumerable<ItemCountResult>>
    {
        public GetUploadsHistoryQuery(DateRangeModel range)
        {
            Range = new DateRangeModel(range.Start, range.End);
        }
        public DateRangeModel Range { get; }
    }
}
