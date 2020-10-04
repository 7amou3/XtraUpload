using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace XtraUpload.Domain
{
    /// <summary>
    /// Represent a file or a folder
    /// </summary>
    public abstract class ItemInfo
    {
        /// <summary>
        /// Id of the item
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Last modification date
        /// </summary>
        public DateTime LastModified { get; set; }
        /// <summary>
        /// whether the item is pass protected or not
        /// </summary>
        public bool HasPassword { get; set; }
        /// <summary>
        /// item status
        /// </summary>
        public ItemStatus Status { get; set; }
        /// <summary>
        /// Id of the user who own this item
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// Navigation property for EF
        /// </summary>
        public virtual User User { get; set; }
    }
}
