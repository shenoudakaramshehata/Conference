using Conference.Data;
using Conference.Models;
using Humanizer.Localisation.DateToOrdinalWords;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Conference.Pages
{
	public class IndexModel : PageModel
	{
		private readonly ILogger<IndexModel> _logger;
		private ConferenceContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ILogger<IndexModel> logger, ConferenceContext context, UserManager<ApplicationUser> userManager)
		{
			_logger = logger;
			_context = context;
            _userManager = userManager;
        }
		[BindProperty]
		public int EventId { get; set; }
        [BindProperty]
        public Event record { get; set; }

        [BindProperty]
        public string EventBarcode { get; set; }

        public string eventVideo { get; set; }
        public string eventTitle { get; set; }

        [BindProperty, Required]
        public string SelectedOption { get; set; }

        public IActionResult OnGet(int id)
		
        {
			try
			{
                 record = _context.Events.Where(c => c.EventBarcode == id.ToString() && c.EventIsActive == true).FirstOrDefault();
                if (record!=null) 
				{
					EventId = record.EventId;
					EventBarcode = record.EventBarcode;
                    eventVideo = record.EventVideo;
                    eventTitle = record.EventTitle;
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
        //public async Task<IActionResult> OnGetCheckRegister()
        //{
        //    bool isRegister = false;
        //    var user = await _userManager.GetUserAsync(User);
        //    if(user == null)
        //    {
        //        return new JsonResult(isRegister);
        //    }
        //    else
        //    {
        //        isRegister = true;
        //        return new JsonResult(isRegister);
        //    }
        //}
        public IActionResult OnPost(string radioGroup,int EventId)
        {
            // Handle the selected option and redirect based on it
            if (radioGroup == "option1")
            {
                return RedirectToPage("/Individual", new { EventId = EventId });
            }
            else if (radioGroup == "option2")
            {
                return RedirectToPage("/Audience", new { EventId = EventId });
            }

            // Default redirect or error handling
            return RedirectToPage("/Index");
        }

        //public void OnPostSubmit(int fruit)
        //{
          
        //    if (SelectedOption == "option1")
        //    {
              
        //    }
        //    else if (SelectedOption == "option2")
        //    {
               
        //    }

        //}

    }
}
