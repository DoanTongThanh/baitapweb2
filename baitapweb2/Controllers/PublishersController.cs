using Microsoft.AspNetCore.Mvc;
using baitapweb2.Models.Domain;
using baitapweb2.Models.DTO;
using baitapweb2.Repositories;
using System.Collections.Generic;

namespace baitapweb2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherRepository _publisherRepository;

        // Dependency Injection
        public PublishersController(IPublisherRepository publisherRepository)
        {
            _publisherRepository = publisherRepository;
        }

        // =========================================================================
        // GET ALL PUBLISHERS (READ)
        // URL: GET /api/Publishers
        // =========================================================================
        [HttpGet]
        public IActionResult GetAll() // Giữ nguyên tên phương thức GetAll() theo code gốc của bạn
        {
            var publishers = _publisherRepository.GetAllPublishers();
            return Ok(publishers);
        }

        // =========================================================================
        // GET PUBLISHER BY ID (READ)
        // URL: GET /api/Publishers/{id}
        // =========================================================================
        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetPublisherById([FromRoute] int id)
        {
            var publisherDto = _publisherRepository.GetPublisherById(id);

            if (publisherDto == null)
            {
                return NotFound();
            }

            return Ok(publisherDto);
        }

        // =========================================================================
        // ADD NEW PUBLISHER (CREATE)
        // URL: POST /api/Publishers
        // =========================================================================
        [HttpPost]
        public IActionResult AddPublisher([FromBody] PublisherNoIDDTO addPublisherRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Publisher addedPublisherDomain = _publisherRepository.AddPublisher(addPublisherRequest);

            // Trả về 201 Created
            return CreatedAtAction(nameof(GetPublisherById), new { id = addedPublisherDomain.PublisherId }, addedPublisherDomain);
        }

        // =========================================================================
        // UPDATE PUBLISHER (UPDATE)
        // URL: PUT /api/Publishers/{id}
        // =========================================================================
        [HttpPut]
        [Route("{id:int}")]
        public IActionResult UpdatePublisherById([FromRoute] int id, [FromBody] PublisherNoIDDTO updatePublisherRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Publisher? updatedPublisherDomain = _publisherRepository.UpdatePublisherById(id, updatePublisherRequest);

            if (updatedPublisherDomain == null)
            {
                return NotFound();
            }

            return Ok(updatedPublisherDomain);
        }

        // =========================================================================
        // DELETE PUBLISHER (DELETE)
        // URL: DELETE /api/Publishers/{id}
        // =========================================================================
        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult DeletePublisherById([FromRoute] int id)
        {
            // Controller này chỉ gọi Repository. Mọi lỗi nghiệp vụ (như có sách liên quan) 
            // sẽ được xử lý trong Repository. Nếu Repository trả về null, Controller trả 404.

            Publisher? deletedPublisherDomain = _publisherRepository.DeletePublisherById(id);

            if (deletedPublisherDomain == null)
            {
                return NotFound();
            }

            return Ok(deletedPublisherDomain);
        }
    }
}