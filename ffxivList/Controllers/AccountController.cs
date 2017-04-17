﻿using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using ffxivList.Data;
using ffxivList.Models;
using ffxivList.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace ffxivList.Controllers
{
    public class AccountController : Controller
    {
        private readonly Auth0Settings _auth0Settings;
        private UserProfile userProfile;

        public AccountController(IOptions<Auth0Settings> auth0Settings)
        {
            _auth0Settings = auth0Settings.Value;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login vm, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AuthenticationApiClient client = new AuthenticationApiClient(new Uri($"https://{_auth0Settings.Domain}/"));

                    var result = await client.AuthenticateAsync(new AuthenticationRequest
                    {
                        ClientId = _auth0Settings.ClientId,
                        Scope = "openid",
                        Connection = "Username-Password-Authentication", // Specify the correct name of your DB connection
                        Username = vm.EmailAddress,
                        Password = vm.Password
                    });

                    // Get user info from token
                    var userInfo = await client.GetTokenInfoAsync(result.IdToken);
                    
                    User user = new User { UserId = userInfo.UserId, UserEmail = userInfo.Email, UserName = userInfo.NickName };

                    using (var context = new FFListContext())
                    {             
                        if (context.Users.Find(user.UserId) == null)
                        {
                            user.UserRole = "User";
                            context.Users.Add(user);
                            context.SaveChanges();
                        }
                        else
                        {
                            var userDetails = context.Users.Find(user.UserId);
                            user.UserRole = userDetails.UserRole;
                        }
                    }
                    
                    userProfile = new UserProfile() { ProfileEmail = user.UserEmail, ProfileName = user.UserName, ProfileRole = user.UserRole, ProfileImage = userInfo.Picture };
                    
                    // Create claims principal
                    var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userInfo.UserId),
                        new Claim(ClaimTypes.Name, userInfo.NickName),
                        new Claim(ClaimTypes.Email, userInfo.Email),
                        new Claim("picture", userInfo.Picture),
                        new Claim("role", user.UserRole)
                    }, CookieAuthenticationDefaults.AuthenticationScheme));

                    // Sign user into cookie middleware
                    await HttpContext.Authentication.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                    return RedirectToLocal(returnUrl);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }

            return View(vm);
        }

        [HttpGet]
        public IActionResult LoginExternal(string connection, string returnUrl = "/")
        {
            var properties = new AuthenticationProperties() { RedirectUri = returnUrl };

            if (!string.IsNullOrEmpty(connection))
                properties.Items.Add("connection", connection);

            return new ChallengeResult("Auth0", properties);
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
        public IActionResult ProfileView()
        {
            return View(new UserProfile() {
                ProfileEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value, 
                ProfileRole = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value,
                ProfileName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value
            });
        }

        /// <summary>
        /// This is just a helper action to enable you to easily see all claims related to a user. It helps when debugging your
        /// application to see the in claims populated from the Auth0 ID Token
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult Claims()
        {
            return View();
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
