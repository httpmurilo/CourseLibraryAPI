using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.Entites
{
    public class Authors
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        [Required]
        public DateTimeOffset DateOfBirth { get; set; }          
        [Required]
        [MaxLength(50)]
        public string MainCategory { get; set; }
        public ICollection<Courses> Courses { get; set; }
            = new List<Courses>();
    }
}
