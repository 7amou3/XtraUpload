namespace XtraUpload.Administration.Service.Common
{
    public class FileTypesCountResult
    {
        public string Extension { get; set; }
        public int ItemCount { get; set; }
    };

    public class FileTypeResult
    {
        public FileType FileType { get; set; }
        public int ItemCount { get; set; }
    }

    public enum FileType
    {
        Others,
        Archives,
        Documents,
        Multimedia,
    }
}
