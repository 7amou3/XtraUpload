using System;

namespace XtraUpload.Domain
{
    /// <summary>
    /// Used to paginate data
    /// </summary>
    public class PageSearchModel
    {
        public PageSearchModel()
        {

        }
        public PageSearchModel(PageSearchModel pageSearch)
        {
            PageIndex = pageSearch.PageIndex;
            PageSize = pageSearch.PageSize;
            Length = pageSearch.Length;
            PreviousPageIndex = pageSearch.PreviousPageIndex;
            Start = pageSearch.Start;
            End = pageSearch.End;
            UserId = pageSearch.UserId;
            FileExtension = pageSearch.FileExtension;
        }
        /// <summary>
        /// The current page index.
        /// </summary>
        public ushort PageIndex { get; set; }
        /// <summary>
        /// The current page size
        /// </summary>
        public ushort PageSize { get; set; }
        /// <summary>
        /// The current total number of items being paged
        /// </summary>
        public uint Length { get; set; }
        /// <summary>
        /// Index of the page that was selected previously
        /// </summary>
        public ushort? PreviousPageIndex { get; set; }
        /// <summary>
        /// Search start date 
        /// </summary>
        public DateTime? Start { get; set; }
        /// <summary>
        /// Search end date 
        /// </summary>
        public DateTime? End
        {
            get { return _end; }
            set 
            {
                if (value == null) return;
                _end = value.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
        }
        private DateTime? _end;
        /// <summary>
        /// Files owner 
        /// </summary>
        public Guid? UserId { get; set; }
        /// <summary>
        /// files extensions
        /// </summary>
        public string FileExtension { get; set; }

    }
}
