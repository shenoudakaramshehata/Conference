using Conference.Data;
using Conference.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Conference.Areas.Admin.Pages
{
    public class IndexModel : PageModel
    {
		


		public IndexModel(ConferenceContext context, UserManager<ApplicationUser> userManager, ApplicationDbContext db)
		{
			

		}
		public async Task<IActionResult> OnGet()
		{
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
               CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("ar-EG")),
               new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(1) }
               );
          return Page();

		}
	}
}
