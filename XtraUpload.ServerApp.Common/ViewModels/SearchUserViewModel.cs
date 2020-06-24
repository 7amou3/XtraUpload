using System.ComponentModel.DataAnnotations;

namespace XtraUpload.ServerApp.Common
{
    public class SearchUserViewModel
    {
        [Required]
        [MinLength(1)]
        public string Name { get; set; }
    }
}
