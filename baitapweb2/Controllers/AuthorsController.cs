using Microsoft.AspNetCore.Mvc;
using baitapweb2.Models.Domain;
using baitapweb2.Models.DTO;
using baitapweb2.Repositories;
using System.Collections.Generic;

namespace baitapweb2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;

        // Dependency Injection
        public AuthorsController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        // =========================================================================
        // GET ALL AUTHORS (READ)
        // URL: GET /api/Authors
        // =========================================================================
        [HttpGet]
        public IActionResult GetAll() // Giữ nguyên tên GetAll()
        {
            var authors = _authorRepository.GetAllAuthors();
            return Ok(authors);
        }

        // =========================================================================
        // GET AUTHOR BY ID (READ)
        // URL: GET /api/Authors/{id}
        // =========================================================================
        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetAuthorById([FromRoute] int id)
        {
            var authorDto = _authorRepository.GetAuthorById(id);

            if (authorDto == null)
            {
                return NotFound();
            }

            return Ok(authorDto);
        }

        // =========================================================================
        // ADD NEW AUTHOR (CREATE)
        // URL: POST /api/Authors
        // =========================================================================
        [HttpPost]
        public IActionResult AddAuthor([FromBody] AuthorNoIDDTO addAuthorRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Author addedAuthorDomain = _authorRepository.AddAuthor(addAuthorRequest);

            return CreatedAtAction(nameof(GetAuthorById), new { id = addedAuthorDomain.AuthorId }, addedAuthorDomain);
        }

        // =========================================================================
        // UPDATE AUTHOR (UPDATE)
        // URL: PUT /api/Authors/{id}
        // =========================================================================
        [HttpPut]
        [Route("{id:int}")]
        public IActionResult UpdateAuthorById([FromRoute] int id, [FromBody] AuthorNoIDDTO updateAuthorRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Author? updatedAuthorDomain = _authorRepository.UpdateAuthorById(id, updateAuthorRequest);

            if (updatedAuthorDomain == null)
            {
                return NotFound();
            }

            return Ok(updatedAuthorDomain);
        }

        // =========================================================================
        // DELETE AUTHOR (DELETE)
        // URL: DELETE /api/Authors/{id}
        // =========================================================================
        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult DeleteAuthorById([FromRoute] int id)
        {
            Author? deletedAuthorDomain = _authorRepository.DeleteAuthorById(id);

            if (deletedAuthorDomain == null)
            {
                return NotFound();
            }

            return Ok(deletedAuthorDomain);
        }
    }
}