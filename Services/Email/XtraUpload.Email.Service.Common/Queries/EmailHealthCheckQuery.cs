using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace XtraUpload.Email.Service.Common
{
    public class EmailHealthCheckQuery: IRequest<HealthCheckResult>
    {
    }
}
