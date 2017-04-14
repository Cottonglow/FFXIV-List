using ffxivList.ViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace ffxivList.Controllers
{
    public class AccountController : Controller
    {
        //IOptions<OpenIdConnectOptions> _options;

        //public AccountController(IOptions<OpenIdConnectOptions> options)
        //{
        //    _options = options;
        //}

        //// GET: /<controller>/
        //public IActionResult Login(string returnUrl = null)
        //{
        //    var lockContext = HttpContext.GenerateLockContext(_options.Value, returnUrl);

        //    return View(lockContext);
        //}

        public IActionResult Login(string returnUrl = "/")
        {
            return new ChallengeResult("Auth0", new AuthenticationProperties() { RedirectUri = returnUrl });
        }


        public async Task Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Auth0", new AuthenticationProperties
            {
                RedirectUri = Url.Action("Index", "Home")
            });
            await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [Authorize]
        public IActionResult ProfileView()
        {
            return View(new UserProfileViewModel()
            {
                //Name = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                EmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value
            });
        }
    }
}
