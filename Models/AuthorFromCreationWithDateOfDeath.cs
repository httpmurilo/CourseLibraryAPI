using System;
namespace CourseLibrary.Models
{
    public class AuthorFromCreationWithDateOfDeath : AuthorForCreationDto
    {
        public DateTimeOffset? DateofDeath{get;set;}
    }
}