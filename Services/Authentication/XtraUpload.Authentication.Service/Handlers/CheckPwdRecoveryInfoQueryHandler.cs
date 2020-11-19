using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service
{
    /// <summary>
    /// Checks the status of a password by id (token), the response can be in one of the following state:
    /// - valid.
    /// - does not exist.
    /// - has expired.
    /// - already used.
    /// </summary>
    public class CheckPwdRecoveryInfoQueryHandler : IRequestHandler<CheckPwdRecoveryInfoQuery, OperationResult>
    {
        #region Fields
        readonly IUnitOfWork _unitOfWork;
        #endregion

        #region Constructor
        public CheckPwdRecoveryInfoQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Handler

        public async Task<OperationResult> Handle(CheckPwdRecoveryInfoQuery request, CancellationToken cancellationToken)
        {
            OperationResult Result = new OperationResult();
            ConfirmationKey recoveryInfo = await _unitOfWork.ConfirmationKeys.FirstOrDefaultAsync(s => s.Id == request.RecoeryId);

            // Check recovery info exists
            if (recoveryInfo == null)
            {
                Result.ErrorContent = new ErrorContent("The provided token does not exist", ErrorOrigin.Client);
                return Result;
            }

            // Generated link expires after 24h (by a background alien thread)
            if (DateTime.UtcNow > recoveryInfo.GenerateAt.AddDays(1))
            {
                Result.ErrorContent = new ErrorContent("The provided token was expired", ErrorOrigin.Client);
            }
            if (recoveryInfo.Status != RequestStatus.InProgress)
            {
                Result.ErrorContent = new ErrorContent("The provided token has already been used or expired", ErrorOrigin.Client);
            }

            return Result;
        }

        #endregion
    }
}
