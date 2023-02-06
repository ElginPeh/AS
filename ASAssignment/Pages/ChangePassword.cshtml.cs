using ASAssignment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace ASAssignment.Pages
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ChangePasswordModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Add minimum/maximum password age restriction
            if (user.LastPasswordChangedDate.HasValue)
            {
                var timeSinceLastPasswordChange = DateTime.Now - user.LastPasswordChangedDate.Value;
                //Set minimum password age to 20 mins
                var minimumPasswordAgeInMinutes = 20;
                if (timeSinceLastPasswordChange.TotalMinutes < minimumPasswordAgeInMinutes)
                {
                    ModelState.AddModelError(string.Empty, $"You must wait at least {minimumPasswordAgeInMinutes} minutes from your last password change to change your password again.");
                    return Page();
                }
                var maximumPasswordAgeInDays = 60;
                if (timeSinceLastPasswordChange.TotalDays >= maximumPasswordAgeInDays)
                {
                    ModelState.AddModelError(string.Empty, $"Your password has expired. You must change your password.");
                    return Page();
                }
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.SignOutAsync();

            return RedirectToPage("Index");
        }
    }
}