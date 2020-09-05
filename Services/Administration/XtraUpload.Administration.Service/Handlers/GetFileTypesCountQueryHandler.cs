using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;

namespace XtraUpload.Administration.Service
{
    public class GetFileTypesCountQueryHandler : IRequestHandler<GetFileTypesCountQuery, IEnumerable<FileTypeResult>>
    {
        readonly Dictionary<FileType, List<string>> fileTypes = new Dictionary<FileType, List<string>>()
        {
            { FileType.Archives, new List<string>() {".rar", ".zip", ".tar", ".gzip", ".aaf", ".iso", ".bin" } },
            { FileType.Multimedia, new List<string>() {".mp4", ".flv", ".mov", ".avi", ".mp3", ".wav", ".png", ".gif", ".jpe" ,".jpg", ".jpeg"} },
            { FileType.Documents, new List<string>() {".docx", ".pdf", ".txt", ".xml", ".xlsx", ".csv", ".pptx"} }
        };
        readonly IUnitOfWork _unitOfWork;
        public GetFileTypesCountQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<FileTypeResult>> Handle(GetFileTypesCountQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<FileTypesCountResult> queryResult = await _unitOfWork.Files.FileTypesByDateRange(request.Range.Start, request.Range.End);
            List<FileTypeResult> result = new List<FileTypeResult>();
            // Collect common file types
            foreach (var item in fileTypes)
            {
                result.Add(new FileTypeResult()
                {
                    FileType = item.Key,
                    ItemCount = queryResult.Where(s => fileTypes[item.Key].Contains(s.Extension)).Sum(s => s.ItemCount)
                });
            }
            // other file type
            result.Add(new FileTypeResult()
            {
                FileType = FileType.Others,
                ItemCount = queryResult.Where(s => !fileTypes[FileType.Archives].Contains(s.Extension))
                                       .Where(s => !fileTypes[FileType.Multimedia].Contains(s.Extension))
                                       .Where(s => !fileTypes[FileType.Documents].Contains(s.Extension))
                                       .Sum(s => s.ItemCount)
            });
            return result;
        }
    }
}
