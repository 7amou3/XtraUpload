using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace XtraUpload.Domain
{
    /// <summary>
    /// Confirmation key token (reset password token, email verification token..)
    /// </summary>
    public class ConfirmationKey
    {
        /// <summary>
        /// Id of the key
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        /// <summary>
        /// Status of the request
        /// </summary>
        public RequestStatus Status { get; set; }
        /// <summary>
        /// The geneated token creation date (usualy any confirmation key will expire after x amount of time)
        /// </summary>
        public DateTime GenerateAt { get; set; }
        /// <summary>
        /// Ip of the requester
        /// </summary>
        public string IpAdress { get; set; }
        /// <summary>
        /// User id Navigation property
        /// </summary>
        public virtual string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
