using IrzUccApi.Models.Db;
using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.Events
{
    public class PostEventRequest
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsPublic { get; set; } = false;
        public int? CabinetId { get; set; }
        public IEnumerable<string>? ListenersIds { get; set; }
    }
}