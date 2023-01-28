using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.Role
{
    public class AddRemoveRoleRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string UserId { get; set; } = string.Empty;
        [Required(AllowEmptyStrings = false)]
        public string Role { get; set; } = string.Empty;
    }
}
