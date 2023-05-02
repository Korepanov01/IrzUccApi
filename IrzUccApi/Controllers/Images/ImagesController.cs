using IrzUccApi.Db;
using IrzUccApi.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace IrzUccApi.Controllers.Images
{
    [Route("api/images")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public ImagesController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var image = await _unitOfWork.Images.GetByIdAsync(id);
            if (image == null)
                return NotFound();

            var currentUser = await _unitOfWork.Users.GetByClaimsAsync(User);
            switch (image.Source)
            {
                case Enums.ImageSources.User:
                    if (currentUser == null)
                        return Unauthorized();
                    break;
                case Enums.ImageSources.NewsEntry:
                    var newEntry = await _unitOfWork.NewsEntries.GetByIdAsync(image.SourceId);
                    if (newEntry == null)
                        return NotFound();
                    if (currentUser == null && !newEntry.IsPublic)
                        return Unauthorized();
                    break;
                case Enums.ImageSources.Message:
                    if (currentUser == null)
                        return Unauthorized();
                    var message = await _unitOfWork.Messages.GetByIdAsync(image.SourceId);
                    if (message == null)
                        return NotFound();
                    if (!message.Chat.Participants.Contains(currentUser))
                        return Forbid();
                    break;
            }

            return Ok(new ImageDto(
                image.Id,
                image.Name,
                image.Extension,
                image.Data));
        }
    }
}
