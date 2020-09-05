using System.Linq;
using System.Threading.Tasks;
using MediatR;
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
        readonly IMediator _mediatr;

        public FolderController( IMediator mediatr)
        {
            _mediatr = mediatr;
        }

        [HttpGet(@"{folderid:regex(^[[a-zA-Z0-9]]*$)?}")]
        public async Task<IActionResult> GetFolderContent(string folderid)
        {
            GetFolderContentResult result = await _mediatr.Send(new GetFolderContentQuery(folderid));

            return HandleResult(result);
        }

        [AllowAnonymous]
        [HttpGet("publicfolder")]
        public async Task<IActionResult> GetPublicFolder([FromQuery]PublicFolderViewModel model)
        {
            GetFolderContentResult result = await _mediatr.Send(new GetPublicFolderQuery(model.MainFolderId, model.ChildFolderId));

            return HandleResult(result);
        }

        [HttpGet("folders")]
        public async Task<IActionResult> GetUserFolders()
        {
            GetFoldersResult result = await _mediatr.Send(new GetUserFoldersQuery());

            return HandleResult(result, result.Folders);
        }

        [AllowAnonymous]
        [HttpGet("folders/{parentId:regex(^[[a-zA-Z0-9]]*$)}")]
        public async Task<IActionResult> GetFolders(string parentId)
        {
            GetFoldersResult result = await _mediatr.Send(new GetInnerFoldersQuery(parentId));

            return HandleResult(result, result.Folders);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFolder(CreateFolderViewModel folder)
        {
            CreateFolderResult result = await _mediatr.Send(new CreateFolderCommand(folder.FolderName, folder.ParentFolder.Id));
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
            DeleteFolderResult Result = await _mediatr.Send(new DeleteFolderCommand(folderid));

            return HandleResult(Result, Result.Folders.First(s => s.Id == folderid));
        }

        [HttpPatch("rename")]
        public async Task<IActionResult> Rename(RenameFolderViewModel folder)
        {
            RenameFolderResult Result = await _mediatr.Send(new UpdateFolderNameCommand(folder.FileId, folder.NewName));

            return HandleResult(Result, Result.Folder);
        }

        [HttpPatch("folderavailability")]
        public async Task<IActionResult> FileAvailability(FolderAvailabilityViewModel folderAvailability)
        {
            FolderAvailabilityResult Result = await _mediatr.Send(new UpdateFolderAvailabilityCommand(folderAvailability.Folderid, folderAvailability.Available));

            return HandleResult(Result, Result.Folder);
        }
    }
}