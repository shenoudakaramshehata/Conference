using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Conference.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Conference.Data;
using Microsoft.Extensions.Options;

namespace Conference.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private ConferenceContext _context;
        //public static int EvId = 0;
        //public static int TypeId = 0;
        public List<int> OptionItems = new List<int>();
        [BindProperty]
        public int EvId { get; set; }
        [BindProperty]
        public int TypeId { get; set; }
        [BindProperty]
        public string StrOptions { get; set; }

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            ConferenceContext context,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
           
        }

        
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public IActionResult OnGet(string Options,int EventId, int EventSubscriptionType)

        {
            try
            {
                var EventObj = _context.Events.Where(e => e.EventId == EventId).FirstOrDefault();
                if (EventObj != null)
                {
                    EvId = EventId;
                    TypeId = EventSubscriptionType;
                    StrOptions = Options;
                    
                    return Page();
                }
                else
                {
                    return RedirectToPage("/NotFound", new { id = EventId });
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IActionResult> OnPostAsync(string userName,string userPhone, string StrOptions, int EvId, int TypeId)
        {

            var OptionList = StrOptions.Split(',');
            foreach (var item in OptionList)
            {
                int c = 0;
                bool checkParse = int.TryParse(item, out c);
                if (checkParse)
                {
                    OptionItems.Add(c);

                }
            }
            if (OptionItems.Count == 0)
            {
                if (TypeId == 1)
                {

                    return RedirectToPage("/Individual", new { EventId = EvId });

                }
                else if (TypeId == 2)
                {
                    return RedirectToPage("/Audience", new { EventId = EvId });
                }
            }
            var random = new Random();
                var email = "user" + random.Next(1, 1000000) + "@gmail.com";
                var password = "ssSsho@1996";
                var user = new ApplicationUser { UserName = email, Email = email, FullName = userName, PhoneNumber = userPhone };
                var result = await _userManager.CreateAsync(user, password);
                
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _signInManager.SignInAsync(user, isPersistent: true);
                List<SubscriptionActivity> SubscriptionActivityList = new List<SubscriptionActivity>();
                foreach (var item in OptionItems)
                {
                    var SubscriptionActivityObj = new SubscriptionActivity()
                    {
                        EventActivityId=item,
                        
                    };
                    SubscriptionActivityList.Add(SubscriptionActivityObj);
                }
                var EventSub = new EventSubscription()
                {
                    EventId = EvId,
                    EventSubscriptionDate = DateTime.Now,
                    EventSubscriptionType = TypeId,
                    EventSubscriptionFullName = user.FullName,
                    EventSubscriptionMobile = user.PhoneNumber,
                    SubscriptionActivities = SubscriptionActivityList
                };
                var SubList = _context.EventSubscriptions.Where(e => e.EventId == EvId).ToList();
                if (SubList.Count == 0)
                {
                    EventSub.EventSubscriptionQueue = 1;
                }
                else
                {
                    var SubQueue = _context.EventSubscriptions.Where(e => e.EventId == EvId).OrderByDescending(e => e.EventSubscriptionId).FirstOrDefault().EventSubscriptionQueue;
                    if (SubQueue == null)
                    {
                        EventSub.EventSubscriptionQueue = 1;

                    }
                    else
                    {
                        EventSub.EventSubscriptionQueue = SubQueue.Value + 1;

                    }

                }

                _context.EventSubscriptions.Add(EventSub);
                _context.SaveChanges();
                return RedirectToPage("/ThankYou", new { QU = EventSub.EventSubscriptionQueue });
                //return Redirect("/ThankYou");
            }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            // If we got this far, something failed, redisplay form
            return Page();
        }


        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}

        
  
