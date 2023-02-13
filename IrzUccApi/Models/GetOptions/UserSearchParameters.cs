using IrzUccApi.Models.PagingOptions;

namespace IrzUccApi.Models.GetOptions
{
    public class UserSearchParameters : SearchStringParameters
    {
        public Guid? PositionId { get; set; }

        public string? Role { get; set; }

        public bool? IsActive { get; set; }
    }
}
