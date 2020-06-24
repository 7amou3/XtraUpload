using System.ComponentModel.DataAnnotations;

namespace XtraUpload.ServerApp.Common
{
    /// <summary>
    /// Email configuration
    /// </summary>
    public class EmailSettings
    {
        /// <summary>
        /// The Smtp server configs
        /// (you can provide your own smtp's server credentials or choose a 3rd party provider like gmail smtp relay...)
        /// </summary>
        public SMTP Smtp { get; set; }
        /// <summary>
        /// The sender of the emails
        /// </summary>
        public Sender Sender { get; set; }
    }

    /// <summary>
    /// SMTP server configs
    /// </summary>
    public class SMTP
    {
        [Required]
        public string Server { get; set; }
        [Range(1, int.MaxValue)]
        public int Port { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }

    /// <summary>
    /// Sender of the emails (usualy the admin of the website)
    /// </summary>
    public class Sender
    {
        [Required]
        public string Name { get; set; }
        [Required, EmailAddress]
        public string Admin { get; set; }
        [Required, EmailAddress]
        public string Support { get; set; }
    }
}
