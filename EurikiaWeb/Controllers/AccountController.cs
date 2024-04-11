using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace EukairiaWeb.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet("authenticate")]
        public IActionResult Authenticate(string returnUrl = "/")
        {
            return Challenge( new AuthenticationProperties { RedirectUri = returnUrl });
        }

        [HttpGet("acceptlogin")]
        [IgnoreAntiforgeryToken]
        public IActionResult acceptlogin(string returnUrl = "/")
        {
            return Challenge(new AuthenticationProperties { RedirectUri = returnUrl });
        }
    }
}
