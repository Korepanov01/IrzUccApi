using IrzUccApi.Db;
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
        public async Task<IActionResult> GetImageAsync(Guid id)
        {
            var image = await _unitOfWork.Images.GetByIdAsync(id);
            if (image == null)
                return NotFound();
            
            return File(image.Content, image.ContentType);
        }
    }
}
