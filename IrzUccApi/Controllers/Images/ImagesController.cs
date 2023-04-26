using IrzUccApi.Db;
using IrzUccApi.Models.Db;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi.Controllers.Images
{
    [Route("api/images")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public ImagesController(AppDbContext dbContext, UserManager<AppUser> userManager, EmailService emailService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var image = await _dbContext.Images.FirstOrDefaultAsync(i => i.Id == id);
            if (image == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            switch (image.Source)
            {
                case Enums.ImageSources.User:
                    if (currentUser == null)
                        return Unauthorized();
                    break;
                case Enums.ImageSources.NewsEntry:
                    var newEntry = await _dbContext.NewsEntries.FirstOrDefaultAsync(n => n.Id == image.SourceId);
                    if (newEntry == null)
                        return NotFound();
                    if (currentUser == null && !newEntry.IsPublic)
                        return Unauthorized();
                    break;
                case Enums.ImageSources.Message:
                    if (currentUser == null)
                        return Unauthorized();
                    var message = await _dbContext.Messages.FirstOrDefaultAsync(m => m.Id == image.SourceId);
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
