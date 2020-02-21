using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static CourseLibrary.Entites.Authors;

namespace CourseLibrary.Entites
{
    public class Courses
    {
        [Key]       
        public Guid Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(1500)]
        public string Description { get; set; }
        [ForeignKey("AuthorId")]
        public Authors Author { get; set; }
        public Guid AuthorId { get; set; }
    }
}
