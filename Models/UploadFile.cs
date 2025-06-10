using System;
using System.ComponentModel.DataAnnotations; // needed because i am using the display attrib from .DataAnnotations

// This is the database model - it defines what is savees in the db

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
    }
}
