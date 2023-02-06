using ASAssignment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ASAssignment.Pages
{
    public class UserListModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AuthDbContext _context;

        public UserListModel(UserManager<IdentityUser> userManager,
            AuthDbContext context)
        {
            _userManager = userManager;
            _context = context;
            Users = new List<IdentityUser>();
        }

        public IList<IdentityUser> Users { get; set; }

        public async Task OnGetAsync()
        {
            Users = await _userManager.Users.ToListAsync();

        }

        //Delete function
        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            //Find user according to id
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            //Found the user , Delete the user
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                //Save the changes
                await _context.SaveChangesAsync();
                return RedirectToPage("UserList");
            }
            else
            {
                // Add an error message to the page if the delete operation failed
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

        }
    }
}
