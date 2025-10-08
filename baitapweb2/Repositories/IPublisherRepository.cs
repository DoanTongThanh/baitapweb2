using baitapweb2.Models.Domain;
using baitapweb2.Models.DTO;
using System.Collections.Generic;

namespace baitapweb2.Repositories
{
    public interface IPublisherRepository
    {
        // READ
        List<PublisherDTO> GetAllPublishers();
        PublisherDTO GetPublisherById(int id);

        // CREATE
        Publisher AddPublisher(PublisherNoIDDTO addPublisherRequest);

        // UPDATE
        Publisher UpdatePublisherById(int id, PublisherNoIDDTO publisherNoIdDto);

        // DELETE
        Publisher? DeletePublisherById(int id);
    }
}