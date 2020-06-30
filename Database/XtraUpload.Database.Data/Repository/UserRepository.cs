using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Setting.Service.Common;
using System.Linq.Expressions;
using XtraUpload.WebApp.Common;

namespace XtraUpload.Database.Data
{
    public class UserRepository: Repository<User>, IUserRepository
    {
        readonly ApplicationDbContext _context;
        
        public UserRepository(ApplicationDbContext dbContext): base(dbContext)
        {
            _context = dbContext;
        }

        #region IUserRepository members

        /// <summary>
        /// Gets the user confirmation key
        /// </summary>
        public async Task<ConfirmationKeyResult> GetConfirmationKeyInfo(string confirmationId)
        {
            return await _context.ConfirmationKeys
                    .Include(s => s.User)
                    .Select(s => new ConfirmationKeyResult() { Key = s, User = s.User  })
                    .SingleOrDefaultAsync(s => s.Key.Id == confirmationId);
        }
       
        /// <summary>
        /// Gets the user's role claims
        /// </summary>
        public async Task<RoleClaimsResult> GetUserRoleClaims(User user)
        {
            var query =  _context.Users
                                .Include(s => s.Role)
                                .ThenInclude(s => s.RoleClaims)
                                .Where(s => s.Id == user.Id)
                                .Select(s => new RoleClaimsResult { Role = s.Role, Claims = s.Role.RoleClaims });

            return await query.SingleOrDefaultAsync();
        }

        /// <summary>
        /// Get all role claims pair
        /// </summary>
        public async Task<IEnumerable<RoleClaimsResult>> GetUsersRoleClaims()
        {
            var query = _context.Roles
                            .Include(s => s.RoleClaims)
                            .Select(s => new RoleClaimsResult { Role = s, Claims = s.RoleClaims });

            return await query.ToListAsync();
        }

        /// <summary>
        /// Group users count by date range
        /// </summary>
        public async Task<IEnumerable<ItemCountResult>> UsersCountByDateRange(DateTime start, DateTime end)
        {
            var query = _context.Users
                        .Where(s => s.CreatedAt >= start && s.CreatedAt <= end)
                        .GroupBy(f => f.CreatedAt.Date)
                        .OrderBy(s => s.Key)
                        .Select(s => new ItemCountResult
                        {
                            Date = s.Key,
                            ItemCount = s.Count()
                        });

            return await query.ToListAsync();
        }

        /// <summary>
        /// Search users by name
        /// </summary>
        public async Task<IEnumerable<User>> SearchUsersByName(string name)
        {
           var query = _context.Users
                        .Where(s => s.UserName.Contains(name, StringComparison.OrdinalIgnoreCase))
                        .Take(10);
            return await query.ToListAsync();
        }

        /// <summary>
        /// Search and return a paging list of users
        /// </summary>
        public async Task<IEnumerable<UserExtended>> GetUsers(PageSearchViewModel model, Expression<Func<User, bool>> searchCriteria)
        {
            var query = _context.Users
                               .Include(s => s.Role)
                               .Where(searchCriteria)
                               .OrderBy(s => s.CreatedAt)
                               .Skip(model.PageIndex * model.PageSize)
                               .Take(model.PageSize)
                               .Select(s => new UserExtended()
                               {
                                   Id = s.Id,
                                   Avatar = s.Avatar,
                                   UserName = s.UserName,
                                   RoleName = s.Role.Name,
                                   Email = s.Email,
                                   EmailConfirmed = s.EmailConfirmed,
                                   CreatedAt = s.CreatedAt,
                                   AccountSuspended = s.AccountSuspended
                               });

            return await query.ToListAsync();

        }
        #endregion
    }
}
