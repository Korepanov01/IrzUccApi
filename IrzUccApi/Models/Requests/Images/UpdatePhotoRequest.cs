using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.Images
{
    public class UpdatePhotoRequest
    {
        [Required]
        public IFormFile? File { get; set; }
    }
}
