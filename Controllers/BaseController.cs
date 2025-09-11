using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MyDotNetProject.Controllers
{
    public class BaseController : ControllerBase
    {
        protected ClaimsPrincipal? GetCurrentUser()
        {
            return User;
        }

        protected string? GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        protected string? GetCurrentUserEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value;
        }

        protected string? GetCurrentUserName()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value;
        }
    }
}
