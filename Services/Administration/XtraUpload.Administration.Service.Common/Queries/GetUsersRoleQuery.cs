using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Get the users role
    /// </summary>
    public class GetUsersRoleQuery : IRequest<RolesResult>
    {
    }
}
