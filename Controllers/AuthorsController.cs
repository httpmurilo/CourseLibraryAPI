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

namespace CourseLibrary.Controllers {
    [ApiController]
    [Route ("api/authors")]
    public class AuthorsController : ControllerBase {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public AuthorsController (ICourseLibraryRepository courseLibraryRepository,
            IMapper mapper) {
            _courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException (nameof (courseLibraryRepository));
            _mapper = mapper ??
                throw new ArgumentNullException (nameof (mapper));
        }

        [HttpGet (Name = "GetAuthors")]
        [HttpHead]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors (
            [FromQuery] AuthorsResourceParameters authorsResourceParameters) {
            var authorsFromRepo = _courseLibraryRepository.GetAuthors (authorsResourceParameters);
            var previousPageLink = authorsFromRepo.HasPrevious ?
            CreateAuthorsResourceUri(authorsResourceParameters,
            ResourceUriType.PreviousPage) : null;
            var nextPageLink = authorsFromRepo.HasNext ?
            CreateAuthorsResourceUri(authorsResourceParameters,
            ResourceUriType.NetxPage) : null;
            var paginationMetadata = new 
            {
                totalCount = authorsFromRepo.TotalCount,
                pageSize  = authorsFromRepo.PageSize,
                currentPage = authorsFromRepo.CurrentPage,
                totalPages = authorsFromRepo.TotalPages,
                 previousPageLink,
                 nextPageLink
            };
            Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(paginationMetadata));
            return Ok (_mapper.Map<IEnumerable<AuthorDto>> (authorsFromRepo));
        }

        [HttpGet ("{authorId}", Name = "GetAuthor")]
        public IActionResult GetAuthor (Guid authorId) {
            var authorFromRepo = _courseLibraryRepository.GetAuthor (authorId);

            if (authorFromRepo == null) {
                return NotFound ();
            }

            return Ok (_mapper.Map<AuthorDto> (authorFromRepo));
        }

        [HttpPost]
        public ActionResult<AuthorDto> CreateAuthor (AuthorForCreationDto author) {
            var authorEntity = _mapper.Map<Entities.Author> (author);
            _courseLibraryRepository.AddAuthor (authorEntity);
            _courseLibraryRepository.Save ();

            var authorToReturn = _mapper.Map<AuthorDto> (authorEntity);
            return CreatedAtRoute ("GetAuthor",
                new { authorId = authorToReturn.Id },
                authorToReturn);
        }

        [HttpOptions]
        public IActionResult GetAuthorsOptions () {
            Response.Headers.Add ("Allow", "GET,OPTIONS,POST");
            return Ok ();
        }

        [HttpDelete ("{authorId}")]
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
                            pageNumber = resourceParameters.PageNumber - 1,
                                pageSize = resourceParameters.PageSize,
                                mainCategory = resourceParameters.MainCategory,
                                searchQuery = resourceParameters.SearchQuery
                        });
                case ResourceUriType.NetxPage:
                    return Url.Link ("GetAuthors",
                        new {
                            pageNumber = resourceParameters.PageNumber + 1,
                                pageSize = resourceParameters.PageSize,
                                mainCategory = resourceParameters.MainCategory,
                                searchQuery = resourceParameters.SearchQuery
                        });
                default:
                    return Url.Link ("GetAuthors",
                        new {
                            pageNumber = resourceParameters.PageNumber,
                                pageSize = resourceParameters.PageSize,
                                mainCategory = resourceParameters.MainCategory,
                                searchQuery = resourceParameters.SearchQuery
                        });
            }
        }
    }
}