using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Conference.Data;
using Conference.Models;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Conference.Pages
{
    public class ThankYouModel : PageModel
    {
        private ConferenceContext _context;

        public ThankYouModel(ConferenceContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int InQe { get; set; }

        [BindProperty]
        public int EvetId { get; set; }
        public string EventImg { get; set; }
        public void OnGet(int QU, int EventId)
        {

            InQe = QU;
            EvetId = EventId;
            EventImg = _context.Events.Where(a => a.EventId == EventId).Select(a => a.QueueImage).FirstOrDefault();
        }
    }
}
