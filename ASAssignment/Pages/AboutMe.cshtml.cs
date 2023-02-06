using ASAssignment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace ASAssignment.Pages
{
    public class AboutMeModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AboutMeModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public string AboutMe { get; set; }
        public string decodedAboutMe { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            AboutMe = user.AboutMe;
/*            decodedAboutMe = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(AboutMe));*/
        }
    }
}