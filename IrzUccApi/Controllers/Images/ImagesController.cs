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
        public async Task<IActionResult> Get(Guid id)
        {
            var image = await _dbContext.Images.FirstOrDefaultAsync(i => i.Id == id);
            if (image == null)
                return NotFound();

            return Ok(new ImageDto(
                image.Id,
                image.Name,
                image.Extension,
                image.Data));
        }
    }
}
