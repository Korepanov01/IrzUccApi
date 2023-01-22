using IrzUccApi.Models;
using IrzUccApi.Models.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace IrzUccApi.Controllers
{
    [Route("api/roles")]
    [ApiController]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class RolesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = new List<string>()
            {
                "Сhancellery",
                "Support"
            };

            if (User.IsInRole("SuperAdmin"))
                roles.Add("Admin");

            return Ok(roles);
        }
    }
}
