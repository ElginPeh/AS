using ASAssignment.Models;
using ASAssignment.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace ASAssignment.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Login LModel { get; set; }

        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;
        //Failed attempts
        private readonly int MAX_FAILED_ACCESS_ATTEMPTS = 3;
        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<LoginModel> logger)
        {
            this.signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(LModel.Email);
                if (user != null)
                {
                    if (!await _userManager.IsLockedOutAsync(user))
                    {
                        var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password,
                        LModel.RememberMe, false);
                        if (identityResult.Succeeded)
                        {
                            //Session creation
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, user.UserName),
                                new Claim(ClaimTypes.Email, user.Email),
                                new Claim("Role", "Admin")
                            };
                            var i = new ClaimsIdentity(claims, "MyCookieAuth");
                            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(i);
                            await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

                            //Session Timeout
                            await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal, new AuthenticationProperties
                            {
                                IsPersistent = LModel.RememberMe,
                                ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                            });
                            return RedirectToPage("Index");
                        }
                        else
                        {
                            //Failed attempts
                            await _userManager.AccessFailedAsync(user);
                            int accessFailedCount = await _userManager.GetAccessFailedCountAsync(user);
                            if (accessFailedCount >= MAX_FAILED_ACCESS_ATTEMPTS)
                            {
                                await _userManager.SetLockoutEndDateAsync(user, new System.DateTimeOffset(System.DateTime.UtcNow.AddMinutes(5)));
                                _logger.LogWarning("User account locked out.");
                                ModelState.AddModelError("", "Your account is locked out");
                                return Page();
                            }
                        }
                    }
                    else
                    {
                        //Message showing failed attempts
                        _logger.LogWarning("User account locked out.");
                        ModelState.AddModelError("", "Your account is locked out");
                        return Page();
                    }
                }
                ModelState.AddModelError("", "Username or Password incorrect");
            }
            return Page();

            // Verify reCAPTCHA
            string secretKey = "<your-secret-key>";
            string response = HttpContext.Request.Form["g-recaptcha-response"];
            string remoteIp = HttpContext.Connection.RemoteIpAddress.ToString();

            string apiUrl = $"https://www.google.com/recaptcha/api/siteverify?secret=6LcAylMkAAAAAAl845TpuM_myb9jaA5F_L907vDJ&response={response}&remoteip={remoteIp}";

            WebRequest request = WebRequest.Create(apiUrl);
            WebResponse webResponse = await request.GetResponseAsync();

            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
            {
                string responseString = reader.ReadToEnd();
                JObject jsonResponse = JObject.Parse(responseString);

                if (jsonResponse["success"].Value<bool>())
                {
                    // reCAPTCHA verification passed, continue with login logic
        }
                else
                {
                    // reCAPTCHA verification failed, display error message
                    ModelState.AddModelError("", "reCAPTCHA verification failed, please try again.");
                    return Page();
                }
            }
        }
    }
}
