using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IrzUccApi.Models.Dtos.Role
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleDto : ControllerBase
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }
}
