using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ep_synoptic_2005.Models
{
    public class UploadFileViewModel
    {
        [Required]
        [Display(Name = "File Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Choose File")]
        public IFormFile File { get; set; }
    }
}
