using AutoMapper;

namespace CourseLibrary.Profiles
{
    public class CoursesProfile : Profile
    {
        public CoursesProfile()
        {
            
            CreateMap<Entities.Course, Models.CourseDto>();
            CreateMap<Models.CourseForCreationDto, Entities.Course>();
            CreateMap<Models.CourseForUpdateDto, Entities.Course>();
            CreateMap<Entities.Course, Models.CourseForUpdateDto>();
        }
    }
}
