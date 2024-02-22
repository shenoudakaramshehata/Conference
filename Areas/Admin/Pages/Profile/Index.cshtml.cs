using Conference.Data;
using Conference.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NToastNotify;
using System.Diagnostics.Eventing.Reader;

namespace Conference.Areas.Admin.Pages.Profile
{
    public class IndexModel : PageModel
    {
        private ConferenceContext _context;
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        private readonly ILogger<LogoutModel> _logger;

        [BindProperty]
        public ChangePasswordVM ChangePasswordVM { get; set; }
        [BindProperty]
        public ApplicationUser? Admin { get; set; }

        public IndexModel(ConferenceContext context, IWebHostEnvironment hostEnvironment,
                                            IToastNotification toastNotification,  ILogger<LogoutModel> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            ChangePasswordVM = new ChangePasswordVM();
        }
        public async Task<IActionResult> OnGet()

        {

                Admin = await _userManager.GetUserAsync(User);
            

            if (Admin != null)
            {
                if (ChangePasswordVM.CurrentEmail != null)
                {
                    Admin.Email = ChangePasswordVM.CurrentEmail;
                    Admin.PasswordHash = ChangePasswordVM.CurrentPassword;

                }

                else
                {
                    ChangePasswordVM.CurrentEmail = Admin.Email;
                }
                return Page();

            }
            else
            {
                return Redirect("~/Identity/Account/login");

            }
           



        }

        public async Task<IActionResult> OnPostChangeAccountInfo(IFormFile fileImg)
        {
           
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _toastNotification.AddErrorToastMessage("Admin Not Found");

                return Redirect("/login");
            }

            if (fileImg != null)
            {


                string folder = "Images/Admin/";

                user.UserPic = await UploadImage(folder, fileImg);
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    _context.SaveChanges();


                }
            }
          
            

            if (ChangePasswordVM.CurrentEmail != null)
            {
                if (ChangePasswordVM.CurrentEmail != user.Email)
                {
                    var userExists = await _userManager.FindByEmailAsync(ChangePasswordVM.CurrentEmail);
                    if (userExists != null)
                    {
                        _toastNotification.AddErrorToastMessage("Email is already exist");
                        return Page();
                    }
                    user.Email = ChangePasswordVM.CurrentEmail;
                    var setEmailResult = await _userManager.SetEmailAsync(user, user.Email);
                    if (!setEmailResult.Succeeded)
                    {
                        foreach (var error in setEmailResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return Page();
                    }

                }

            }
            else
            {
                _toastNotification.AddErrorToastMessage("Please Enter New Email");
                return Page();
            }

            if (ChangePasswordVM.CurrentPassword != null && ChangePasswordVM.NewPassword != null && ChangePasswordVM.ConfirmPassword != null)
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(
                user,
                ChangePasswordVM.CurrentPassword,
                ChangePasswordVM.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(user, isPersistent: false);
                await _signInManager.RefreshSignInAsync(user);

            }

            return Redirect("/Admin/Profile/Index");
        }
        

        private async Task<string> UploadImage(string folderPath, IFormFile file)
        {

            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            await file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }
    }
}

