using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.Extensions.Logging;
using XtraUpload.FileManager.Service.Common;
using System.Threading.Tasks;
using System.Threading;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using System.Security.Claims;
using System.Linq;
using XtraUpload.Domain.Infra;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace XtraUpload.FileManager.Service
{
    public class FileDownloadService: IFileDownloadService
    {
        #region Fields
        readonly ClaimsPrincipal _caller;
        readonly HttpContext _httpContext;
        readonly UploadOptions _uploadOpts;
        readonly IUnitOfWork _unitOfWork;
        readonly ILogger<FileDownloadService> _logger;
        #endregion

        #region Constructor
        public FileDownloadService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IOptionsMonitor<UploadOptions> uploadOpts, 
            ILogger<FileDownloadService> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _uploadOpts = uploadOpts.CurrentValue;
            _caller = httpContextAccessor.HttpContext.User;
            _httpContext = httpContextAccessor.HttpContext;
        }
        #endregion

        #region IFileDownloadService members

        
        /// <summary>
        /// Generate a templink
        /// </summary>
        public async Task<TempLinkResult> TempLink(string fileId)
        {
            TempLinkResult Result = new TempLinkResult();

            // Generate the download data to store
            Download download = new Download()
            {
                Id = Helpers.GenerateUniqueId(),
                FileId = fileId,
                IpAdress = _httpContext.Request.Host.Host,
                StartedAt = DateTime.Now
            };
            _unitOfWork.Downloads.Add(download);

            // Try to save in db
            Result = await _unitOfWork.CompleteAsync(Result);
            if (Result.State == OperationState.Success)
            {
                Result.FileDownload = download;
            }

            return Result;
        }

        /// <summary>
        /// Download a file 
        /// </summary>
        public async Task<StartDownloadResult> StartDownload(string downloadId)
        {
            StartDownloadResult Result = new StartDownloadResult();

            DownloadedFileResult dResult = await _unitOfWork.Downloads.GetDownloadedFile(downloadId);
            // Check download exist
            if (dResult.Download == null || dResult.File == null)
            {
                Result.ErrorContent = new ErrorContent("No file with the provided id was found", ErrorOrigin.Client);
                return Result;
            }
            // Check if it's the same requester
            if (_httpContext.Request.Host.Host != dResult.Download.IpAdress)
            {
                Result.ErrorContent = new ErrorContent("Hotlinking disabled by the administrator.", ErrorOrigin.Client);
                return Result;
            }

            string filePath = Path.Combine(_uploadOpts.UploadPath, dResult.File.UserId, dResult.File.Id, dResult.File.Id);
            await StartDownload(dResult.File, Result, filePath);
            // Increment download counter
            dResult.File.DownloadCount++;
            dResult.File.LastModified = DateTime.Now;

            // Try to save in db
            return await _unitOfWork.CompleteAsync(Result);
        }

        #endregion

        

        private async Task StartDownload(FileItem file, StartDownloadResult result, string filePath)
        {
            if (!IsFileExists(filePath))
            {
                _httpContext.Response.StatusCode = 404;
                result.ErrorContent = new ErrorContent("File not found!", ErrorOrigin.Client);
                return;
            }

            FileInfo fileInfo = new FileInfo(filePath);

            if (fileInfo.Length > int.MaxValue)
            {
                _httpContext.Response.StatusCode = 413;
                result.ErrorContent = new ErrorContent("File is too large!", ErrorOrigin.Server);
                return;
            }

            // Get the response header information by the http request.
            HttpResponseHeader responseHeader = GetResponseHeader(file);

            if (responseHeader == null)
            {
                result.ErrorContent = new ErrorContent("Invalid response header!", ErrorOrigin.Server);
                return;
            }

            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            try
            {
                await SendDownloadFile(responseHeader, fileStream);
            }
            catch (Exception ex)
            {
                _httpContext.Response.StatusCode = 500;
                #region Trace
                _logger.LogError(ex.Message);
                #endregion
            }
            finally
            {
                fileStream.Close();
                fileStream?.Dispose();
            }
        }

        /// <summary>
        /// Check whether the file exists.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private bool IsFileExists(string filePath)
        {
            bool fileExists = false;

            if (!string.IsNullOrEmpty(filePath))
            {
                if (File.Exists(filePath))
                {
                    fileExists = true;
                }
            }

            return fileExists;
        }

        /// <summary>
        /// Get the response header by the http request.
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private HttpResponseHeader GetResponseHeader(FileItem file)
        {
            if (_httpContext.Request == null)
            {
                return null;
            }

            if (file == null)
            {
                return null;
            }

            long startPosition = 0;
            string contentRange = "";

            string fileName = file.Name;
            long fileLength = file.Size;
            string lastUpdateTimeStr = file.LastModified.ToString();

            string eTag = HttpUtility.UrlEncode(fileName, Encoding.UTF8) + " " + lastUpdateTimeStr;
            string contentDisposition = "attachment;filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8).Replace("+", "%20");
            
            if (_httpContext.Request.Headers["Range"] != StringValues.Empty)
            {
                string[] range = _httpContext.Request.Headers["Range"].ToString().Split(new char[] { '=', '-' });
                startPosition = Convert.ToInt64(range[1]);
                if (startPosition < 0 || startPosition >= fileLength)
                {
                    return null;
                }
            }

            if (_httpContext.Request.Headers["If-Range"].ToString() != null)
            {
                if (_httpContext.Request.Headers["If-Range"].ToString().Replace("\"", "") != eTag)
                {
                    startPosition = 0;
                }
            }

            string contentLength = (fileLength - startPosition).ToString();

            if (startPosition > 0)
            {
                contentRange = string.Format(" bytes {0}-{1}/{2}", startPosition, fileLength - 1, fileLength);
            }

            HttpResponseHeader responseHeader = new HttpResponseHeader
            {
                AcceptRanges = "bytes",
                Connection = "Keep-Alive",
                ContentDisposition = contentDisposition,
                ContentEncoding = Encoding.UTF8,
                ContentLength = contentLength,
                ContentRange = contentRange,
                ContentType = "application/octet-stream",
                Etag = eTag,
                LastModified = lastUpdateTimeStr
            };

            return responseHeader;
        }

        /// <summary>
        /// Send the download file to the client.
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <param name="responseHeader"></param>
        /// <param name="fileStream"></param>
        private async Task SendDownloadFile(HttpResponseHeader responseHeader, Stream fileStream)
        {
            if (_httpContext.Response == null || responseHeader == null)
            {
                return;
            }
            DownloadOption downloadOption = await GetDownloadOption();
            if (!string.IsNullOrEmpty(responseHeader.ContentRange))
            {
                _httpContext.Response.StatusCode = 206;

                // Set the start position of the reading files.
                string[] range = responseHeader.ContentRange.Split(new char[] { ' ', '=', '-' });
                fileStream.Position = Convert.ToInt64(range[2]);
            }
            _httpContext.Response.Clear();
            _httpContext.Features.Get<IHttpResponseBodyFeature>().DisableBuffering();
            _httpContext.Response.Headers.Add("Accept-Ranges", responseHeader.AcceptRanges);
            _httpContext.Response.Headers.Add("Connection", responseHeader.Connection);
            _httpContext.Response.Headers.Add("Content-Disposition", responseHeader.ContentDisposition);
            // httpContext.Response.ContentEncoding = responseHeader.ContentEncoding;
            _httpContext.Response.Headers.Add("Content-Length", responseHeader.ContentLength);
            if (!string.IsNullOrEmpty(responseHeader.ContentRange))
            {
                _httpContext.Response.Headers.Add("Content-Range", responseHeader.ContentRange);
            }
            _httpContext.Response.ContentType = responseHeader.ContentType;
            _httpContext.Response.Headers.Add("Etag", "\"" + responseHeader.Etag + "\"");
            _httpContext.Response.Headers.Add("Last-Modified", responseHeader.LastModified);

            byte[] buffer = new byte[10240];
            long fileLength = Convert.ToInt64(responseHeader.ContentLength);

            // Send file to client.
            while (fileLength > 0)
            {
                if (!_httpContext.RequestAborted.IsCancellationRequested)
                {
                    int length = fileStream.Read(buffer, 0, 10240);

                    await _httpContext.Response.Body.WriteAsync(buffer, 0, length);

                    await _httpContext.Response.Body.FlushAsync();

                    fileLength -= length;

                    // Throttle write speed
                    var sleep = Math.Ceiling((buffer.Length / (1000.0 * downloadOption.Speed)) * 1000.0);
                    Thread.Sleep(int.Parse(sleep.ToString()));
                }
                else
                {
                    fileLength = -1;
                }
            }
        }

        /// <summary>
        /// Get the download option for the client
        /// </summary>
        /// <returns></returns>
        private async Task<DownloadOption> GetDownloadOption()
        {
            DownloadOption option = new DownloadOption()
            {
                Speed = 500,
                TTW = 5,
                WaitTime = 60
            };
            if (_httpContext.User.Identity.IsAuthenticated)
            {
                if (double.TryParse(_caller.Claims.Single(c => c.Type == "DownloadSpeed").Value, out double downloadSpeed))
                {
                    option.Speed = downloadSpeed;
                }
                if (int.TryParse(_caller.Claims.Single(c => c.Type == "DownloadTTW").Value, out int downloadTTW))
                {
                    option.TTW = downloadTTW;
                }
                if (int.TryParse(_caller.Claims.Single(c => c.Type == "WaitTime").Value, out int waitTime))
                {
                    option.WaitTime = waitTime;
                }
            }
            // Guest user
            else
            {
                IEnumerable<RoleClaim> roleClaims = await _unitOfWork.RoleClaims.FindAsync(s => s.RoleId == "3"); // Guest have Id = 3 in RoleClaims table

                if (roleClaims.Any())
                {
                    if (double.TryParse(roleClaims.First(s => s.ClaimType == XtraUploadClaims.DownloadSpeed.ToString()).ClaimValue, out double downloadSpeed))
                    {
                        option.Speed = downloadSpeed;
                    }
                    if (int.TryParse(roleClaims.First(s => s.ClaimType == XtraUploadClaims.DownloadTTW.ToString()).ClaimValue, out int downloadTTW))
                    {
                        option.TTW = downloadTTW;
                    }
                    if (int.TryParse(roleClaims.First(s => s.ClaimType == XtraUploadClaims.WaitTime.ToString()).ClaimValue, out int downloadWaitTime))
                    {
                        option.WaitTime = downloadWaitTime;
                    }
                }
                else
                {
                    #region Trace
                    _logger.LogError("No role claims found for guest user.");
                    #endregion
                }
            }

            return option;
        }
    }

    /// <summary>
    /// Respresent the HttpResponse header information.
    /// </summary>
    internal class HttpResponseHeader
    {
        public string AcceptRanges { get; set; }
        public string Connection { get; set; }
        public string ContentDisposition { get; set; }
        public Encoding ContentEncoding { get; set; }
        public string ContentLength { get; set; }
        public string ContentRange { get; set; }
        public string ContentType { get; set; }
        public string Etag { get; set; }
        public string LastModified { get; set; }
    }

    /// <summary>
    /// Download client option
    /// </summary>
    internal class DownloadOption
    {
        public double Speed { get; set; }
        public int TTW { get; set; }
        public int WaitTime { get; set; }
    }
}
