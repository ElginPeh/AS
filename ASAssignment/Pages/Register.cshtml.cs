using ASAssignment.Models;
using ASAssignment.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using Microsoft.AspNetCore.DataProtection;

namespace ASAssignment.Pages
{
    public class RegisterModel : PageModel
    {
        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        //Use for Data protection
        private readonly IDataProtector _protector;

        [BindProperty]
        public Register RModel { get; set; }
        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
                RoleManager<IdentityRole> roleManager,
                IDataProtectionProvider protectionProvider)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            /*this.passwordValidator = passwordValidator;*/
            _protector = protectionProvider.CreateProtector("Protect_About_Me_Field");
        }
        public void OnGet()
        { 
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {
                    FullName = RModel.FullName,
                    UserName = RModel.Email,
                    CreditCard = RModel.CreditCard,
                    Mobile = RModel.Mobile,
                    Delivery = RModel.Delivery,
                    Email = RModel.Email,
                    AboutMe = RModel.AboutMe,
                };

                try 
                {
                    //Encryption of user's AboutMe
                    var protectedData = _protector.Protect(RModel.CreditCard);
                    user.CreditCard = protectedData;
                }
                catch (FormatException ex)
                {
                    ModelState.AddModelError("", "The input is not a valid Base-64 string. Please enter a valid string.");
                    return Page();
                }

                /*        if (RModel.Photo != null)
                        {
                            user.ProfilePicture = RModel.Photo;
                        }*/

                // Create the User/Admin role if NOT exist (1)
                // By running the application, It creates a role of "user" if the role is not found 
                // If role is not found when trying to add them into the role (2)
                IdentityRole role = await roleManager.FindByIdAsync("Admin");
                if (role == null)
                {
                    IdentityResult result2 = await roleManager.CreateAsync(new IdentityRole("Admin"));
                    if (!result2.Succeeded)
                    {
                        ModelState.AddModelError("", "Create role user failed");
                    }
                }

                var result = await userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
                {
                    //Add users to Admin Role / User Role (2)
                    result = await userManager.AddToRoleAsync(user, "Admin");

                    await signInManager.SignInAsync(user, false);
                    return RedirectToPage("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return Page();
        }

    }
}
