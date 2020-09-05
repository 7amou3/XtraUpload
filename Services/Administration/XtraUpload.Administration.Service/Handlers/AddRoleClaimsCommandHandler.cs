using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Add a new role claims
    /// </summary>
    public class AddRoleClaimsCommandHandler : IRequestHandler<AddRoleClaimsCommand, RoleClaimsResult>
    {
        readonly IUnitOfWork _unitOfWork;
        public AddRoleClaimsCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<RoleClaimsResult> Handle(AddRoleClaimsCommand model, CancellationToken cancellationToken)
        {
            RoleClaimsResult result = new RoleClaimsResult();
            IEnumerable<RoleClaimsResult> allRoleClaims = await _unitOfWork.Users.GetUsersRoleClaims();
            // Check Role name is not duplicated
            if (allRoleClaims.Any(s => s.Role.Name == model.Role.Name))
            {
                result.ErrorContent = new ErrorContent($"A role with the name {model.Role.Name} already exists", ErrorOrigin.Client);
                return result;
            }

            // Add role
            var role = new Role
            {
                Id = Helpers.GenerateUniqueId(),
                Name = model.Role.Name
            };
            _unitOfWork.Roles.Add(role);
            // Add claims
            List<RoleClaim> claims = new List<RoleClaim>();
            if (model.Claims.AdminAreaAccess != null && model.Claims.AdminAreaAccess.Value)
            {
                claims.Add(new RoleClaim() { RoleId = role.Id, ClaimType = XtraUploadClaims.AdminAreaAccess.ToString(), ClaimValue = "1" });
            }
            if (model.Claims.FileManagerAccess != null && model.Claims.FileManagerAccess.Value)
            {
                claims.Add(new RoleClaim() { RoleId = role.Id, ClaimType = XtraUploadClaims.FileManagerAccess.ToString(), ClaimValue = "1" });
            }
            if (model.Claims.ConcurrentUpload != null)
            {
                claims.Add(new RoleClaim() { RoleId = role.Id, ClaimType = XtraUploadClaims.ConcurrentUpload.ToString(), ClaimValue = model.Claims.ConcurrentUpload.ToString() });
            }
            if (model.Claims.DownloadSpeed != null)
            {
                claims.Add(new RoleClaim() { RoleId = role.Id, ClaimType = XtraUploadClaims.DownloadSpeed.ToString(), ClaimValue = model.Claims.DownloadSpeed.ToString() });
            }
            if (model.Claims.DownloadTTW != null)
            {
                claims.Add(new RoleClaim() { RoleId = role.Id, ClaimType = XtraUploadClaims.DownloadTTW.ToString(), ClaimValue = model.Claims.DownloadTTW.ToString() });
            }
            if (model.Claims.FileExpiration != null)
            {
                claims.Add(new RoleClaim() { RoleId = role.Id, ClaimType = XtraUploadClaims.FileExpiration.ToString(), ClaimValue = model.Claims.FileExpiration.ToString() });
            }
            if (model.Claims.MaxFileSize != null)
            {
                claims.Add(new RoleClaim() { RoleId = role.Id, ClaimType = XtraUploadClaims.MaxFileSize.ToString(), ClaimValue = model.Claims.MaxFileSize.ToString() });
            }
            if (model.Claims.StorageSpace != null)
            {
                claims.Add(new RoleClaim() { RoleId = role.Id, ClaimType = XtraUploadClaims.StorageSpace.ToString(), ClaimValue = model.Claims.StorageSpace.ToString() });
            }
            if (model.Claims.WaitTime != null)
            {
                claims.Add(new RoleClaim() { RoleId = role.Id, ClaimType = XtraUploadClaims.WaitTime.ToString(), ClaimValue = model.Claims.WaitTime.ToString() });
            }

            _unitOfWork.RoleClaims.AddRange(claims);

            // Save to db
            result = await _unitOfWork.CompleteAsync(result);
            if (result.State == OperationState.Success)
            {
                result.Role = role;
                result.Claims = claims;
            }

            return result;
        }
    }
}
