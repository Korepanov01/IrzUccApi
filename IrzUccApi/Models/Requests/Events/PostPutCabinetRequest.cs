using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.Events
{
    public class PostPutCabinetRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; } = string.Empty;
    }
}
