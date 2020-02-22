using AutoMapper;
using CourseLibrary.Helpers;

namespace CourseLibrary.Profiles
{
    public class AuthorsFrofile : Profile
    {
          public AuthorsFrofile()
        {
            CreateMap<Entities.Author, Models.AuthorDto>()
                .ForMember(
                    dest => dest.Name, 
                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(
                    dest => dest.Age, 
                    opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge()));

            CreateMap<Models.AuthorForCreationDto, Entities.Author>();
        }
    }
}
