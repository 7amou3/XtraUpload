using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;
using XtraUpload.WebApp.Common;

namespace XtraUpload.WebApp.Controllers
{

    [Authorize(Policy = "User")]
    public class FolderController : BaseController
    {
        readonly IFileManagerService _FilemanagerService;

        public FolderController(IFileManagerService filemanagerService)
        {
            _FilemanagerService = filemanagerService;
        }

        [HttpGet(@"{folderid:regex(^[[a-zA-Z0-9]]*$)?}")]
        public async Task<IActionResult> GetFolderContent(string folderid)
        {
            GetFolderContentResult result = await _FilemanagerService.GetUserFolder(folderid);

            return HandleResult(result);
        }

        [AllowAnonymous]
        [HttpGet("publicfolder")]
        public async Task<IActionResult> GetPublicFolder([FromQuery]PublicFolderViewModel model)
        {
            GetFolderContentResult result = await _FilemanagerService.GetPublicFolder(model);

            return HandleResult(result);
        }

        [HttpGet("folders")]
        public async Task<IActionResult> GetUserFolders()
        {
            GetFoldersResult result = await _FilemanagerService.GetUserFolders();

            return HandleResult(result, result.Folders);
        }

        [AllowAnonymous]
        [HttpGet("folders/{parentId:regex(^[[a-zA-Z0-9]]*$)}")]
        public async Task<IActionResult> GetFolders(string parentId)
        {
            GetFoldersResult result = await _FilemanagerService.GetFolders(parentId);

            return HandleResult(result, result.Folders);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFolder(CreateFolderViewModel folder)
        {
            CreateFolderResult result = await _FilemanagerService.CreateFolder(folder);
            if (result.State == OperationState.Success)
            {
                return Created($"{BaseUrl}/folder={result.Folder.Id}", result.Folder);
            }
            else
            {
                return HandleResult(result);
            }
        }

        [HttpDelete("{folderid:regex(^[[a-zA-Z0-9]]*$)}")]
        public async Task<IActionResult> Delete(string folderid)
        {
            DeleteFolderResult Result = await _FilemanagerService.DeleteFolder(folderid);

            return HandleResult(Result, Result.Folders.First(s => s.Id == folderid));
        }

        [HttpPatch("rename")]
        public async Task<IActionResult> Rename(RenameFolderViewModel folder)
        {
            RenameFolderResult Result = await _FilemanagerService.UpdateFolderName(folder.FileId, folder.NewName);

            return HandleResult(Result, Result.Folder);
        }

        [HttpPatch("folderavailability")]
        public async Task<IActionResult> FileAvailability(FolderAvailabilityViewModel folderAvailability)
        {
            FolderAvailabilityResult Result = await _FilemanagerService.UpdateFolderAvailability(folderAvailability.Folderid, folderAvailability.Available);

            return HandleResult(Result, Result.Folder);
        }
    }
}