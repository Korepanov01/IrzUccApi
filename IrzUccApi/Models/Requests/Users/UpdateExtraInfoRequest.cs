using IrzUccApi.Models.Requests.Images;
using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.User
{
    public class UpdateExtraInfoRequest
    {
        public ImageRequest? Image { get; set; }
        [MaxLength(500)]
        public string? AboutMyself { get; set; }
        [MaxLength(500)]
        public string? MyDoings { get; set; }
        [MaxLength(250)]
        public string? Skills { get; set; }
    }
}
