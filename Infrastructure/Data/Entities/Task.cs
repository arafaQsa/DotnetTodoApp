using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Infrastructure.Data.Entities
{
    public class Task
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The task title is required.")]
        [StringLength(25, ErrorMessage = "The task title cannot exceed 30 characters.")]
        public required string Title { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public Boolean isCompleted { get; set; } = false;
    }
}