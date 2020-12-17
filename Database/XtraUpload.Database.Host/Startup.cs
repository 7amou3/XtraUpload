using System;
using XtraUpload.Database.Data;
using XtraUpload.Database.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace XtraUpload.Database.Host
{
    public static class Startup
    {
        public static void AddDatabase(this IServiceCollection services, IConfiguration config)
        {
            // Register provider
            string dbProvider = config["Database:Provider"].ToLower();
            if (string.CompareOrdinal(dbProvider, "sqlserver") == 0)
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                   options.UseSqlServer(
                       config["Database:ConnectionString"],
                       sqlServerOptions => sqlServerOptions.MigrationsAssembly("XtraUpload.Database.Migrations")));
            }
            else if (string.CompareOrdinal(dbProvider, "mysql") == 0)
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                   options.UseMySql(config["DataBase:ConnectionString"],
                       ServerVersion.AutoDetect(config["DataBase:ConnectionString"]),
                       mySqlServerOptions => mySqlServerOptions.MigrationsAssembly("XtraUpload.Database.Migrations")));
            }
            else
            {
                throw new Exception("Invalid Database provider, XtraUpload support: Sql or MySql as a provider.");
            }

            // Register repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleClaimsRepository, RoleClaimsRepository>();
            services.AddScoped<IFolderRepository, FolderRepository>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IDownloadRepository, DownloadRepository>();
            services.AddScoped<IConfirmationKeyRepository, ConfirmationKeyRepository>();
            services.AddScoped<IFileExtensionRepository, FileExtensionRepository>();
            services.AddScoped<IPageRepository, PageRepository>();
            services.AddScoped<IStorageServerRepository, StorageServerRepository>();
            services.AddScoped<ILanguageRepository, LanguageRepository>();

            // Register Health check
            services.AddHealthChecks().AddDbContextCheck<ApplicationDbContext>("Database Server");
        }
    }
}
