using System.Linq;
using System.Runtime.Serialization.Json;
using System;
using System.Collections.Generic;
using AutoMapper;
using CourseLibrary.Helpers;
using CourseLibrary.Models;
using CourseLibrary.Repository;
using CourseLibrary.ResourceParameters;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using CourseLibrary.Services;
using CourseLibrary.Entities;

namespace CourseLibrary.Controllers {
    [ApiController]
    [Route ("api/authors")]
    public class AuthorsController : ControllerBase {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService  _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;

        public AuthorsController (ICourseLibraryRepository courseLibraryRepository,
            IMapper mapper, IPropertyMappingService propertyMapping, IPropertyCheckerService propertyCheckerService) {
            _courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException (nameof (courseLibraryRepository));
            _mapper = mapper ??
                throw new ArgumentNullException (nameof (mapper));
            _propertyMappingService = propertyMapping ??
            throw new ArgumentNullException (nameof(propertyMapping));
            _propertyCheckerService = propertyCheckerService ??
            throw new ArgumentNullException(nameof(propertyCheckerService));
        }

        [HttpGet (Name = "GetAuthors")]
        [HttpHead]
        public IActionResult GetAuthors (
            [FromQuery] AuthorsResourceParameters authorsResourceParameters) 
            {
            if(!_propertyMappingService.ValidMappingExistsFor<AuthorDto, Author>(authorsResourceParameters.OrderBy))
            {
            return BadRequest();
            }
            if(!_propertyCheckerService.TypeHasProperties<AuthorDto>
            (authorsResourceParameters.Fields))
            {
                return BadRequest();
            }
            var authorsFromRepo = _courseLibraryRepository.GetAuthors (authorsResourceParameters);
            var paginationMetadata = new 
            {
                totalCount = authorsFromRepo.TotalCount,
                pageSize  = authorsFromRepo.PageSize,
                currentPage = authorsFromRepo.CurrentPage,
                totalPages = authorsFromRepo.TotalPages
            };
            Response.Headers.Add("X-Pagination",
                 JsonSerializer.Serialize(paginationMetadata));
            var links = CreateLinksForAuthor(authorsResourceParameters,
            authorsFromRepo.HasNext,authorsFromRepo.HasPrevious);
            //retorna um IE do expandoObject
            var modelandoAutores = _mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo)
                .ShapeData(authorsResourceParameters.Fields);
            var modelandoAutoresComLinks = modelandoAutores.Select( author =>
            {
                //selecionamos cada um deles,e inclui um autor ao dicionario juntamente com o links, reutilizando o mesmo codigo do get author,passa um recurso de autores como valor para os links
                var authorAsDictionary = author as IDictionary<string,object>;
                var authorLinks = CreateLinksForAuthor((Guid) authorAsDictionary["Id"], null);
                authorAsDictionary.Add("links",authorLinks);
                return authorAsDictionary;
            });
            var linksCollectionResource = new
            {//criamos um tipo anonimo e devolvemos
                value = modelandoAutoresComLinks,
                links
            };
            return Ok(linksCollectionResource);
        }

        [HttpGet ("{authorId}", Name = "GetAuthor")]
        public IActionResult GetAuthor (Guid authorId, string fields)
         {
             if(!_propertyCheckerService.TypeHasProperties<AuthorDto>(fields))
            {
                return BadRequest();
            }
            var authorFromRepo = _courseLibraryRepository.GetAuthor (authorId);

            if (authorFromRepo == null) {
                return NotFound ();
            }

            var links = CreateLinksForAuthor(authorId, fields);
            var linkedResourceToReturn = 
            _mapper.Map<AuthorDto>(authorFromRepo).ShapeData(fields)
            as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);
            return Ok(linkedResourceToReturn);
        }

        [HttpPost(Name="CreateAuthor")]
        public ActionResult<AuthorDto> CreateAuthor (AuthorForCreationDto author) {
            var authorEntity = _mapper.Map<Entities.Author> (author);
            _courseLibraryRepository.AddAuthor (authorEntity);
            _courseLibraryRepository.Save ();

            var authorToReturn = _mapper.Map<AuthorDto> (authorEntity);
            var links = CreateLinksForAuthor(authorToReturn.Id, null);
            var linkedResourceToReturn = authorToReturn.ShapeData(null)
            as IDictionary<string,object>;
            linkedResourceToReturn.Add("links", links);
            return CreatedAtRoute("GetAuthor",
            new {authorId = linkedResourceToReturn["Id"]},
            linkedResourceToReturn);
        }

        [HttpOptions]
        public IActionResult GetAuthorsOptions () {
            Response.Headers.Add ("Allow", "GET,OPTIONS,POST");
            return Ok ();
        }

        [HttpDelete ("{authorId}", Name="DeleteAuthor")]
        public ActionResult DeleteAuthor (Guid authorId) {
            var authorFromRepo = _courseLibraryRepository.GetAuthor (authorId);

            if (authorFromRepo == null) {
                return NotFound ();
            }

            _courseLibraryRepository.DeleteAuthor (authorFromRepo);

            _courseLibraryRepository.Save ();

            return NoContent ();
        }
        private string CreateAuthorsResourceUri (
            AuthorsResourceParameters resourceParameters,
            ResourceUriType type)
             {
            switch (type) {
                case ResourceUriType.PreviousPage:
                    return Url.Link ("GetAuthors",
                        new {
                            fields = resourceParameters.Fields,
                            orderBy = resourceParameters.OrderBy,
                            pageNumber = resourceParameters.PageNumber - 1,
                                pageSize = resourceParameters.PageSize,
                                mainCategory = resourceParameters.MainCategory,
                                searchQuery = resourceParameters.SearchQuery
                        });
                case ResourceUriType.NetxPage:
                    return Url.Link ("GetAuthors",
                        new {
                            fields = resourceParameters.Fields,
                            orderBy = resourceParameters.OrderBy,
                            pageNumber = resourceParameters.PageNumber + 1,
                                pageSize = resourceParameters.PageSize,
                                mainCategory = resourceParameters.MainCategory,
                                searchQuery = resourceParameters.SearchQuery
                        });
                case ResourceUriType.Current:
                default:
                    return Url.Link ("GetAuthors",
                        new {
                            fields = resourceParameters.Fields,
                            orderBy = resourceParameters.OrderBy,
                            pageNumber = resourceParameters.PageNumber,
                                pageSize = resourceParameters.PageSize,
                                mainCategory = resourceParameters.MainCategory,
                                searchQuery = resourceParameters.SearchQuery
                        });
            }
        }
        private IEnumerable<LinkDto> CreateLinksForAuthor(Guid authorId, string fields)
        {
            var links = new List<LinkDto>();
            if(string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkDto(Url.Link("GetAuthor",new{authorId}),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(
                    new LinkDto(Url.Link("GetAuthor", new {authorId, fields}),
                    "self",
                    "GET"));
            }
            links.Add(
                new LinkDto(Url.Link("DeleteAuthor", new {authorId}),
                "delete_author",
             "DELETE"));
             links.Add(
                 new LinkDto(Url.Link("CreateCourseForAuthor", new {authorId}),
                 "create_course_for_author",
                 "POST"));
            links.Add(
                new LinkDto(Url.Link("GetCoursesForAuthor", new {authorId}),
                "courses",
                "GET"));
            
            return links;
        }
        private IEnumerable<LinkDto> CreateLinksForAuthor(AuthorsResourceParameters authorsResourceParameters,
        bool HasNext, bool HasPrevious)
        {
            //nao pode ser excluido e atualizado em nossa implementacao
            var links = new List<LinkDto>();
            links.Add(
                new LinkDto(CreateAuthorsResourceUri(
                    authorsResourceParameters, ResourceUriType.Current)
                    ,"self","GET"));
            if(HasNext)
            {
                links.Add(
                    new LinkDto(CreateAuthorsResourceUri(
                        authorsResourceParameters, ResourceUriType.NetxPage),
                        "nextPage","GET"
                    ));
            }
            if(HasPrevious)
            {
                links.Add(
                    new LinkDto(CreateAuthorsResourceUri(
                        authorsResourceParameters, ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }
            return links;
        }
    }
}