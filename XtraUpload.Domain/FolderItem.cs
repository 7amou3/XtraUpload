namespace XtraUpload.Domain
{
    /// <summary>
    /// Represent a folder
    /// </summary>
    public class FolderItem: ItemInfo
    {
        /// <summary>
        /// Id of the parent folder
        /// </summary>
        public string Parentid { get; set; }
    }
}
