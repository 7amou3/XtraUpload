using MediatR;
using System.Collections.Generic;

namespace XtraUpload.Administration.Service.Common
{
    public class GetFileTypesCountQuery : IRequest<IEnumerable<FileTypeResult>>
    {
        public GetFileTypesCountQuery(DateRangeModel range)
        {
            Range = new DateRangeModel(range.Start, range.End);
        }
        public DateRangeModel Range { get; }
    }
}
