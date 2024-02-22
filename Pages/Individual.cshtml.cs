using Conference.Data;
using Conference.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Conference.Pages
{
    public class IndividualModel : PageModel
    {
        private ConferenceContext _context;
        public List<EventActivity_Single> EventActivity_SingleList = new List<EventActivity_Single>();
        public static int EvId = 0;
        public List<int> OptionItems = new List<int>();
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;


        public int TypeId = 1;
        [BindProperty]
        public string StrOptions { get; set; }
        public Event EventObj { get; set; }
        public string url { get; set; }

        public IndividualModel(ConferenceContext context, UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,

            IEmailSender emailSender)
        {
            _userManager = userManager;
            _context = context;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        public IActionResult OnGet(int EventId)
        {
             EventObj = _context.Events.Where(e => e.EventId == EventId).FirstOrDefault();
            if (EventObj != null)
            {
                EvId = EventId;
                url = $"{this.Request.Scheme}://{this.Request.Host}";
                EventActivity_SingleList = _context.EventActivity_Singles.Where(e => e.EventActivityType == 1&&e.EventId== EventId).ToList();
                return Page();
            }
            else
            {
                return RedirectToPage("/NotFound", new { id = EventId });
            }



        }
        public IActionResult OnGetActivitesDetails(int EventActivityId)
        {
            var activites = _context.EventActivity_Singles.Where(e=>e.EventActivityId== EventActivityId).Select(e => new
            {
                EventActivityDescription=   e.EventActivityDescription,
                EventActivityTitle=  e.EventActivityTitle
            }).FirstOrDefault();
            return new JsonResult(activites);
        }
        
        public IActionResult OnPost(List<int> Options)
        {
            if (Options != null)
            {
                if (Options.Count == 0)
                {
                    return RedirectToPage("/Individual", new { EventId = EvId });
                }

                string str = "";
                for (int i = 0; i < Options.Count; i++)
                {
                    if (i != 0)
                    {
                        str = str + ",";

                    }
                    str = str + Options[i].ToString();

                }

                return RedirectToPage("/Register", new { Options = str, EventId = EvId, EventSubscriptionType = 1 });

            }
            return RedirectToPage("/Individual", new { EventId = EvId });



        }




        public async Task<IActionResult> OnPostRegister(string userName, string userPhone, string StrOptions)
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
                        EventActivityId = item,

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
                return RedirectToPage("/ThankYou", new { QU = EventSub.EventSubscriptionQueue, EventId = EventSub.EventId });
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
