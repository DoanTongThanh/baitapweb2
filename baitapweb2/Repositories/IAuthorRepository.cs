using baitapweb2.Models.Domain;
using baitapweb2.Models.DTO;
using System.Collections.Generic;

namespace baitapweb2.Repositories
{
    public interface IAuthorRepository
    {
        // READ
        List<AuthorDTO> GetAllAuthors();
        AuthorDTO GetAuthorById(int id);

        // CREATE
        Author AddAuthor(AuthorNoIDDTO addAuthorRequest);

        // UPDATE
        Author UpdateAuthorById(int id, AuthorNoIDDTO authorNoIdDto);

        // DELETE
        Author? DeleteAuthorById(int id);
    }
}