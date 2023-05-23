using IrzUccApi.Db;
using IrzUccApi.Db.Models;
using IrzUccApi.Db.Repositories;
using IrzUccApi.Enums;
using IrzUccApi.ErrorDescribers;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.GetOptions;
using IrzUccApi.Models.Requests.News;
using IrzUccApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IrzUccApi.Controllers.News
{
    [Route("api/news")]
    [ApiController]
    [Authorize]
    public class NewsController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public NewsController(UserManager<AppUser> userManager, UnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetNewsAsync([FromQuery] NewsSearchParameters parameters)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var news = await _unitOfWork.NewsEntries.GetNewsEntryDtosAsync(currentUser, parameters);

            return Ok(news);
        }

        [HttpPost]
        public async Task<IActionResult> PostNewsEntryAsync([FromForm] PostNewsEntryRequest request)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            if (request.IsPublic && !User.IsInRole(RolesNames.Support))
                return Forbid();

            var newNewsEntryId = Guid.NewGuid();

            Image? image = null;
            if (request.Image != null)
            {
                try
                {
                    image = await _unitOfWork.Images.AddAsync(request.Image);
                }
                catch (FileTooBigException)
                {
                    return BadRequest(RequestErrorDescriber.FileTooBig);
                }
                catch (ForbiddenFileExtensionException)
                {
                    return BadRequest(RequestErrorDescriber.ForbiddenExtention);
                }
            }

            var newsEntry = new NewsEntry
            {
                Id = newNewsEntryId,
                Title = request.Title,
                Text = request.Text,
                Image = image,
                DateTime = DateTime.UtcNow,
                Author = currentUser,
                IsPublic = request.IsPublic
            };
            _unitOfWork.NewsEntries.Add(newsEntry);

            await _unitOfWork.SaveAsync();

            return Ok(newsEntry.Id);
        }

        [HttpGet("{id}/full_text")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNewsEntryTextAsync(Guid id)
        {
            var newsEntry = await _unitOfWork.NewsEntries.GetByIdAsync(id);
            if (newsEntry == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (!newsEntry.IsPublic && currentUser == null)
                return Forbid();

            return Ok(newsEntry.Text);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNewsEntryAsync(Guid id)
        {
            var newsEntry = await _unitOfWork.NewsEntries.GetByIdAsync(id);
            if (newsEntry == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (!newsEntry.IsPublic && currentUser == null)
                return Forbid();

            return Ok(new NewsEntryDto(newsEntry, currentUser));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewsEntryAsync(Guid id)
        {
            var currentUserId = ClaimsExtractor.ExtractId(User);
            if (currentUserId == null)
                return Unauthorized();

            var newsEntry = await _unitOfWork.NewsEntries.GetByIdAsync(id);
            if (newsEntry == null)
                return NotFound();
                        
            if (newsEntry.Author.Id != new Guid(currentUserId) && !newsEntry.IsPublic || !User.IsInRole(RolesNames.Support) && newsEntry.IsPublic)
                return Forbid();

            _unitOfWork.NewsEntries.Remove(newsEntry);
            await _unitOfWork.SaveAsync();

            return Ok();
        }
    }
}
