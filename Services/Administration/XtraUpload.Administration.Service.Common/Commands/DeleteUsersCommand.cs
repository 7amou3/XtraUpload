using MediatR;
using System.Collections.Generic;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Delete a list of users
    /// </summary>
    public class DeleteUsersCommand : IRequest<OperationResult>
    {
        public DeleteUsersCommand(IEnumerable<string> usersId)
        {
            UsersId = new HashSet<string>(usersId);
        }

        public IEnumerable<string> UsersId { get; }
    }
}
