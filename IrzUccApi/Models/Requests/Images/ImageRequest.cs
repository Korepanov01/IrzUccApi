using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.Images
{
    public class ImageRequest
    {
        [Required(AllowEmptyStrings = false), MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required(AllowEmptyStrings = false), MaxLength(10)]
        public string Extension { get; set; } = string.Empty;
        [Required(AllowEmptyStrings = false)]
        public string Data { get; set; } = string.Empty;
    }
}
