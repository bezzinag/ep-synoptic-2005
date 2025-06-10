using System.ComponentModel.DataAnnotations;

// <summary>
// This class represents the view model for uploading files in the application.
// It contains properties for the file title and the file itself, with validation attributes to ensure they are provided.
// </summary>
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
