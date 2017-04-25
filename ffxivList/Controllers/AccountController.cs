using ffxivList.Data;
using ffxivList.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;


namespace ffxivList.Controllers
{
    public class AccountController : Controller
    {
        private readonly Auth0Settings _auth0Settings;
        IOptions<OpenIdConnectOptions> _options;

        public AccountController(IOptions<Auth0Settings> auth0Settings, IOptions<OpenIdConnectOptions> options)
        {
            _auth0Settings = auth0Settings.Value;
            _options = options;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return new ChallengeResult("Auth0", new AuthenticationProperties() { RedirectUri = Url.Action("LoginSuccessful", "Account") });
        }

        [HttpGet]
        public IActionResult LoginExternal(string connection, string returnUrl = "/")
        {
            var properties = new AuthenticationProperties() { RedirectUri = returnUrl };

            if (!string.IsNullOrEmpty(connection))
                properties.Items.Add("connection", connection);

            return new ChallengeResult("Auth0", properties);
        }

        public async Task<IActionResult> LoginSuccessful()
        {
            using (var context = new FfListContext())
            {
                // Get user info from token
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                var userName = User.Claims.FirstOrDefault(c => c.Type == "username").Value;

                User user = new User { UserId = userId, UserName = userName };

                List<Levemete> levemetes = await context.Levemetes.AsNoTracking().ToListAsync();
                List<Quest> quests = await context.Quest.AsNoTracking().ToListAsync();
                List<Craft> crafts = await context.Craft.AsNoTracking().ToListAsync();

                if (context.Users.Find(user.UserId) == null)
                {
                    user.UserRole = "User";
                    user.UserCraftsCompleted = 0;
                    user.UserLevemetesCompleted = 0;
                    user.UserQuestsCompleted = 0;

                    context.Users.Add(user);

                    foreach (var leve in levemetes)
                    {
                        context.UserLevemete.Add(
                            new UserLevemete()
                            {
                                IsComplete = false,
                                LevemeteId = leve.LevemeteId,
                                UserId = user.UserId
                            });
                    }

                    foreach (var quest in quests)
                    {
                        context.UserQuest.Add(
                            new UserQuest()
                            {
                                IsComplete = false,
                                QuestId = quest.QuestId,
                                UserId = user.UserId
                            });
                    }

                    foreach (var craft in crafts)
                    {
                        context.UserCraft.Add(
                            new UserCraft()
                            {
                                IsComplete = false,
                                CraftId = craft.CraftId,
                                UserId = user.UserId
                            });
                    }
                }
                else
                {
                    var userDetails = context.Users.Find(user.UserId);
                    user.UserRole = userDetails.UserRole;
                    user.UserCraftsCompleted = userDetails.UserCraftsCompleted;
                    user.UserQuestsCompleted = userDetails.UserQuestsCompleted;
                    user.UserLevemetesCompleted = userDetails.UserLevemetesCompleted;
                }
                context.SaveChanges();

                // Create claims principal
                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, userEmail),
                    new Claim("picture", User.Claims.FirstOrDefault(c => c.Type == "picture").Value),
                    new Claim("role", user.UserRole)
                }, CookieAuthenticationDefaults.AuthenticationScheme));

                // Sign user into cookie middleware
                await HttpContext.Authentication.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            }
            return RedirectToLocal("/");
        }
        
        [Authorize]
        public async Task Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Auth0", new AuthenticationProperties
            {
                // Indicate here where Auth0 should redirect the user after a logout.
                // Note that the resulting absolute Uri must be whitelisted in the 
                // **Allowed Logout URLs** settings for the client.
                RedirectUri = Url.Action("Index", "Home")
            });
            await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [Authorize]
        public async Task<IActionResult> ProfileView()
        {
            UserProfile userProfile;

            using (var context = new FfListContext())
            {
                List<Levemete> levemetes = await context.Levemetes.AsNoTracking().ToListAsync();
                List<Quest> quests = await context.Quest.AsNoTracking().ToListAsync();
                List<Craft> crafts = await context.Craft.AsNoTracking().ToListAsync();
                var user = context.Users.Find(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
                string userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                userProfile = new UserProfile()
                {
                    ProfileEmail = userEmail,
                    ProfileName = user.UserName,
                    ProfileRole = user.UserRole,
                    ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value,
                    LevemetesCompleted = user.UserLevemetesCompleted,
                    LevemetesTotal = levemetes.Count,
                    CraftsCompleted = user.UserCraftsCompleted,
                    CraftsTotal = crafts.Count,
                    QuestsCompleted = user.UserQuestsCompleted,
                    QuestsTotal = quests.Count
                };
            }
            return View(userProfile);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
