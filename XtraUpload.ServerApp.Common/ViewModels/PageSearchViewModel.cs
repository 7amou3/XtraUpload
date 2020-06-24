using System;

namespace XtraUpload.ServerApp.Common
{
    /// <summary>
    /// Used to paginate data
    /// </summary>
    public class PageSearchViewModel
    {
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
        public Nullable<ushort> PreviousPageIndex { get; set; }
        /// <summary>
        /// Search start date 
        /// </summary>
        public Nullable<DateTime> Start { get; set; }
        /// <summary>
        /// Search end date 
        /// </summary>
        public Nullable<DateTime> End
        {
            get { return _end; }
            set { _end = value.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59); }
        }
        private Nullable<DateTime> _end;
        /// <summary>
        /// Files owner 
        /// </summary>
        public Nullable<Guid> UserId { get; set; }
        /// <summary>
        /// files extensions
        /// </summary>
        public string FileExtension { get; set; }

    }
}
