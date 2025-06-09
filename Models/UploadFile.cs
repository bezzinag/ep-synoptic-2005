using System;
using System.ComponentModel.DataAnnotations; // needed because i am using the display attrib from .DataAnnotations

namespace ep_synoptic_2005.Models
{
    public class UploadFile
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "File Title")] // like this Title will show as a nice "File title"
        public string Title { get; set; }

        public string StoredFileName { get; set; }

        public string UploadedByUserId { get; set; }

        public DateTime UploadedDate { get; set; }

    }
}
