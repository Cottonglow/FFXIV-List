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
        private readonly FfListContext _context;

        public AccountController( FfListContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return new ChallengeResult("Auth0", new AuthenticationProperties() { RedirectUri = Url.Action("LoginSuccessful", "Account") });
        }
        
        public async Task<IActionResult> LoginSuccessful()
        {
                // Get user info from token
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                var userName = User.Claims.FirstOrDefault(c => c.Type == "username").Value;

                User user = new User { UserId = userId, UserName = userName };

                List<Levemete> levemetes = await _context.Levemetes.AsNoTracking().ToListAsync();
                List<Quest> quests = await _context.Quest.AsNoTracking().ToListAsync();
                List<Craft> crafts = await _context.Craft.AsNoTracking().ToListAsync();

                if (_context.Users.Find(user.UserId) == null)
                {
                    user.UserRole = "User";
                    user.UserCraftsCompleted = 0;
                    user.UserLevemetesCompleted = 0;
                    user.UserQuestsCompleted = 0;

                    _context.Users.Add(user);

#if DEBUG
                    foreach (var leve in levemetes)
                    {
                        AllUserLevemete allUserLevemete = await _context.AllUserLevemete.LastAsync();

                        _context.AllUserLevemete.Add(new AllUserLevemete()
                        {
                            IsComplete = false,
                            LevemeteId = leve.LevemeteId,
                            LevemeteLevel = leve.LevemeteLevel,
                            LevemeteName = leve.LevemeteName,
                            UserId = userId,
                            UserLevemeteId = allUserLevemete.UserLevemeteId + 1
                        });

                        _context.SaveChanges();

                        _context.UserLevemete.Add(
                            new UserLevemete()
                            {
                                IsComplete = false,
                                LevemeteId = leve.LevemeteId,
                                UserId = user.UserId,
                                UserLevemeteId = allUserLevemete.UserLevemeteId + 1
                            });

                        _context.SaveChanges();
                    }

                    foreach (var quest in quests)
                    {
                        AllUserQuest allUserQuest = await _context.AllUserQuest.LastAsync();

                        _context.UserQuest.Add(
                            new UserQuest()
                            {
                                IsComplete = false,
                                QuestId = quest.QuestId,
                                UserId = user.UserId,
                                UserQuestId = allUserQuest.UserQuestId + 1
                            });

                        _context.SaveChanges();

                        _context.AllUserQuest.Add(new AllUserQuest()
                        {
                            IsComplete = false,
                            QuestId = quest.QuestId,
                            QuestLevel = quest.QuestLevel,
                            QuestName = quest.QuestName,
                            UserId = userId,
                            UserQuestId = allUserQuest.UserQuestId + 1
                        });

                        _context.SaveChanges();
                    }

                    foreach (var craft in crafts)
                    {
                        AllUserCraft allUserCraft = await _context.AllUserCraft.LastAsync();

                        _context.UserCraft.Add(
                            new UserCraft()
                            {
                                IsComplete = false,
                                CraftId = craft.CraftId,
                                UserId = user.UserId,
                                UserCraftId = allUserCraft.UserCraftId + 1
                            });

                        _context.SaveChanges();

                        _context.AllUserCraft.Add(new AllUserCraft()
                        {
                            IsComplete = false,
                            CraftId = craft.CraftId,
                            CraftLevel = craft.CraftLevel,
                            CraftName = craft.CraftName,
                            UserId = userId,
                            UserCraftId = allUserCraft.UserCraftId + 1
                        });

                        _context.SaveChanges();
                    }
#elif RELEASE
                    foreach (var leve in levemetes)
                    {
                        _context.UserLevemete.Add(
                            new UserLevemete()
                            {
                                IsComplete = false,
                                LevemeteId = leve.LevemeteId,
                                UserId = user.UserId
                            });
                    }

                    foreach (var quest in quests)
                    {
                        _context.UserQuest.Add(
                            new UserQuest()
                            {
                                IsComplete = false,
                                QuestId = quest.QuestId,
                                UserId = user.UserId
                            });
                    }

                    foreach (var craft in crafts)
                    {
                        _context.UserCraft.Add(
                            new UserCraft()
                            {
                                IsComplete = false,
                                CraftId = craft.CraftId,
                                UserId = user.UserId
                            });
                    }
#endif
                }
                else
                {
                    var userDetails = _context.Users.Find(user.UserId);
                    user.UserRole = userDetails.UserRole;
                    user.UserCraftsCompleted = userDetails.UserCraftsCompleted;
                    user.UserQuestsCompleted = userDetails.UserQuestsCompleted;
                    user.UserLevemetesCompleted = userDetails.UserLevemetesCompleted;
                }
                _context.SaveChanges();

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
            
                List<Levemete> levemetes = await _context.Levemetes.AsNoTracking().ToListAsync();
                List<Quest> quests = await _context.Quest.AsNoTracking().ToListAsync();
                List<Craft> crafts = await _context.Craft.AsNoTracking().ToListAsync();
                var user = _context.Users.Find(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
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
