﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using tusdotnet.Models.Configuration;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service
{
    public class AvatarUploadService : BaseFileUpload
    {
        public AvatarUploadService(IServiceProvider serviceProvider) : base(serviceProvider, "/avatarupload")
        {
        }

        /// <summary>
        /// Handle the upload completion
        /// </summary>
        protected override async Task OnUploadCompleted(FileCompleteContext ctx)
        {
            try
            {
                MoveFilesToFolder(ctx);
                //update db
                await UpdateDb(ctx);
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
                throw _ex;
            }
        }

        /// <summary>
        ///  Update db
        /// </summary>
        private async Task UpdateDb(FileCompleteContext ctx)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            using IUnitOfWork unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            User user = await unitOfWork.Users.FirstOrDefaultAsync(s => s.Id == ctx.HttpContext.User.GetUserId());
            if (user != null)
            {
                var request = ctx.HttpContext.Request;
                user.LastModified = DateTime.Now;
                user.Avatar = request.Scheme + "://" + request.Host.ToString() + "/api/file/avatar/" + user.Id;
                await unitOfWork.CompleteAsync();
            }
        }

        /// <summary>
        /// Moves the uploaded files to the avatar folder
        /// tus protocol puts the uploaded files into the store, XtraUpload move those files to the user directory
        /// </summary>
        private void MoveFilesToFolder(FileCompleteContext ctx)
        {
            UploadOptions uploadOpts = _serviceProvider.GetService<IOptionsMonitor<UploadOptions>>().CurrentValue;

            string userid = ctx.HttpContext.User.GetUserId();
            string userFolder = Path.Combine(uploadOpts.UploadPath, userid);
            string avatarFolder = Path.Combine(userFolder, "avatar");
            // Create user root directory
            if (!Directory.Exists(userFolder))
            {
                Directory.CreateDirectory(userFolder);
            }
            // Creat avatar directory
            if (!Directory.Exists(avatarFolder))
            {
                Directory.CreateDirectory(avatarFolder);
            }
            // move avatar to the avatar folder
            DirectoryInfo directoryInfo = new DirectoryInfo(uploadOpts.UploadPath);
            foreach (FileInfo file in directoryInfo.GetFiles(ctx.FileId + "*"))
            {
                // Exemple of file names generated by tus are (...69375.metadata, ...69375.uploadlength ...)
                string[] subNames = file.Name.Split('.');
                string subName = subNames.Count() == 2 ? '.' + subNames[1] : "orig.avatar.png";
                File.Move(file.FullName, Path.Combine(avatarFolder, subName), true);
            }
            // crop image
            string avatarPath = Path.Combine(avatarFolder, "orig.avatar.png");
            using FileStream smallavatar = new FileStream(Path.Combine(avatarFolder, "avatar.png"), FileMode.Create);
            using Image image = Image.Load(File.ReadAllBytes(avatarPath), out IImageFormat format);

            Image smallthumbnail = image.Clone(i => i.Resize(128, 128).Crop(new Rectangle(0, 0, 128, 128)));
            smallthumbnail.Save(smallavatar, format);
        }

    }
}
