using System.ComponentModel.DataAnnotations;

namespace XtraUpload.WebApi
{
    public class SearchUserViewModel
    {
        [Required]
        [MinLength(1)]
        public string Name { get; set; }
    }
}
