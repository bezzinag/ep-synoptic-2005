using System;
using System.ComponentModel.DataAnnotations;

// <summary>
// This class represents an uploaded file in the system.
// It contains properties for the file's metadata such as title, stored file name, uploader's user ID, upload date, and original file name.
// </summary>
namespace ep_synoptic_2005.Models
{
    public class UploadFile
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string StoredFileName { get; set; }

        [Required]
        public string UploadedByUserId { get; set; }

        [Required]
        public DateTime UploadedDate { get; set; }

        [Required]
        public string OriginalFileName { get; set; }

        
    }
}
