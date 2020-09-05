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

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Updates claims of a role
    /// </summary>
    public class UpdateRoleClaimsCommandHandler : IRequestHandler<UpdateRoleClaimsCommand, RoleClaimsResult>
    {
        readonly IUnitOfWork _unitOfWork;
        public UpdateRoleClaimsCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<RoleClaimsResult> Handle(UpdateRoleClaimsCommand model, CancellationToken cancellationToken)
        {
            RoleClaimsResult result = new RoleClaimsResult();

            IEnumerable<RoleClaimsResult> allRoleClaims = await _unitOfWork.Users.GetUsersRoleClaims();
            IEnumerable<RoleClaim> roleclaims = allRoleClaims.SelectMany(s => s.Claims).Where(s => s.Role.Id != model.Role.Id);
            // Check at least one admin group exist
            if (roleclaims != null
                && !roleclaims.Any(s => s.ClaimType == XtraUploadClaims.AdminAreaAccess.ToString())
                && model.Claims.AdminAreaAccess == false)
            {
                result.ErrorContent = new ErrorContent("At least one Admin group should exist", ErrorOrigin.Client);
                return result;
            }

            Role role = allRoleClaims.SingleOrDefault(s => s.Role.Id == model.Role.Id).Role;
            if (role == null)
            {
                result.ErrorContent = new ErrorContent($"No role found with id {model.Role.Id}", ErrorOrigin.Client);
                return result;
            }
            // Check Role name is not duplicated
            if (allRoleClaims.Any(s => s.Role.Name == role.Name && s.Role.Id != role.Id))
            {
                result.ErrorContent = new ErrorContent($"A role with the name {model.Role.Name} already exists", ErrorOrigin.Client);
                return result;
            }

            // Update Role name
            role.Name = model.Role.Name;

            // Update Role claims
            IEnumerable<RoleClaim> claims = await _unitOfWork.RoleClaims.FindAsync(s => s.RoleId == model.Role.Id);
            _unitOfWork.RoleClaims.RemoveRange(claims);
            List<RoleClaim> updatedClaims = new List<RoleClaim>();

            if (claims.Any())
            {
                if (model.Claims.AdminAreaAccess != null && model.Claims.AdminAreaAccess.Value)
                {
                    updatedClaims.Add(new RoleClaim() { RoleId = model.Role.Id, ClaimType = XtraUploadClaims.AdminAreaAccess.ToString(), ClaimValue = "1" });
                }
                if (model.Claims.FileManagerAccess != null && model.Claims.FileManagerAccess.Value)
                {
                    updatedClaims.Add(new RoleClaim() { RoleId = model.Role.Id, ClaimType = XtraUploadClaims.FileManagerAccess.ToString(), ClaimValue = "1" });
                }
                if (model.Claims.ConcurrentUpload != null)
                {
                    updatedClaims.Add(new RoleClaim() { RoleId = model.Role.Id, ClaimType = XtraUploadClaims.ConcurrentUpload.ToString(), ClaimValue = model.Claims.ConcurrentUpload.ToString() });
                }
                if (model.Claims.DownloadSpeed != null)
                {
                    updatedClaims.Add(new RoleClaim() { RoleId = model.Role.Id, ClaimType = XtraUploadClaims.DownloadSpeed.ToString(), ClaimValue = model.Claims.DownloadSpeed.ToString() });
                }
                if (model.Claims.DownloadTTW != null)
                {
                    updatedClaims.Add(new RoleClaim() { RoleId = model.Role.Id, ClaimType = XtraUploadClaims.DownloadTTW.ToString(), ClaimValue = model.Claims.DownloadTTW.ToString() });
                }
                if (model.Claims.FileExpiration != null)
                {
                    updatedClaims.Add(new RoleClaim() { RoleId = model.Role.Id, ClaimType = XtraUploadClaims.FileExpiration.ToString(), ClaimValue = model.Claims.FileExpiration.ToString() });
                }
                if (model.Claims.MaxFileSize != null)
                {
                    updatedClaims.Add(new RoleClaim() { RoleId = model.Role.Id, ClaimType = XtraUploadClaims.MaxFileSize.ToString(), ClaimValue = model.Claims.MaxFileSize.ToString() });
                }
                if (model.Claims.StorageSpace != null)
                {
                    updatedClaims.Add(new RoleClaim() { RoleId = model.Role.Id, ClaimType = XtraUploadClaims.StorageSpace.ToString(), ClaimValue = model.Claims.StorageSpace.ToString() });
                }
                if (model.Claims.WaitTime != null)
                {
                    updatedClaims.Add(new RoleClaim() { RoleId = model.Role.Id, ClaimType = XtraUploadClaims.WaitTime.ToString(), ClaimValue = model.Claims.WaitTime.ToString() });
                }

                _unitOfWork.RoleClaims.AddRange(updatedClaims);

                // Save to db
                result = await _unitOfWork.CompleteAsync(result);
            }

            result.Role = role;
            result.Claims = updatedClaims;
            return result;
        }
    }
}
