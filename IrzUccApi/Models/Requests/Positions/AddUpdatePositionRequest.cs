using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.Positions
{
    public class AddUpdatePositionRequest
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
