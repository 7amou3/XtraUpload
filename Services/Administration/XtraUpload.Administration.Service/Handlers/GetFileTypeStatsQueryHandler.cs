using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Get file type count grouped by the given period of time
    /// </summary>
    public class GetFileTypeStatsQueryHandler : IRequestHandler<GetFileTypeStatsQuery, AdminOverViewResult>
    {
        readonly Dictionary<FileType, List<string>> fileTypes = new Dictionary<FileType, List<string>>()
        {
            { FileType.Archives, new List<string>() {".rar", ".zip", ".tar", ".gzip", ".aaf", ".iso", ".bin" } },
            { FileType.Multimedia, new List<string>() {".mp4", ".flv", ".mov", ".avi", ".mp3", ".wav", ".png", ".gif", ".jpe" ,".jpg", ".jpeg"} },
            { FileType.Documents, new List<string>() {".docx", ".pdf", ".txt", ".xml", ".xlsx", ".csv", ".pptx"} }
        };
        readonly IUnitOfWork _unitOfWork;
        public GetFileTypeStatsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AdminOverViewResult> Handle(GetFileTypeStatsQuery request, CancellationToken cancellationToken)
        {
            AdminOverViewResult Result = new AdminOverViewResult();
            // Check date range is valid
            if (request.Range.Start.Date > request.Range.End.Date)
            {
                Result.ErrorContent = new ErrorContent("Invalid date range.", ErrorOrigin.Client);
                return Result;
            }
            // Query db
            IEnumerable<FileTypesCountResult> queryResult = await _unitOfWork.Files.FileTypesByDateRange(request.Range.Start, request.Range.End);

            Result.FileTypesCount = FormatResult(queryResult);

            return Result;
        }

        private IEnumerable<FileTypeResult> FormatResult(IEnumerable<FileTypesCountResult> queryResult)
        {
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
