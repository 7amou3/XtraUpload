using MediatR;
using System;
using System.Linq.Expressions;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Get all pages header (no page content is retrieved)
    /// </summary>
    public class GetPagesHeaderQuery : IRequest<PagesHeaderResult>
    {
        public GetPagesHeaderQuery()
        {

        }
        public GetPagesHeaderQuery(Expression<Func<PageHeader, bool>> predicate)
        {
            Predicate = predicate;
        }
        public Expression<Func<PageHeader, bool>> Predicate { get; }
    }
}
