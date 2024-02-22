using Conference.Data;
using Conference.DataTables;
using Conference.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Linq.Dynamic.Core;

namespace Conference.Areas.Admin.Pages.SubscriptionActivity
{
    public class IndexModel : PageModel
    {
        private ConferenceContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;


        public string url { get; set; }
        public static int eventSubId = 0;


        [BindProperty]
        public Event eventRecord { get; set; }


        public List<Event> EventList = new List<Event>();

        public Event eventObj { get; set; }

        public IndexModel(ConferenceContext context, IWebHostEnvironment hostEnvironment,
                                            IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            eventRecord = new Event();
            eventObj = new Event();
        }
        public void OnGet(int id)
        {
            var eventSubscriptionsObj = _context.EventSubscriptions.Where(e => e.EventSubscriptionId == id).FirstOrDefault();
            if (eventSubscriptionsObj != null)
            {
                eventSubId = id;
            }
        }
        [BindProperty]
        public DataTablesRequest DataTablesRequest { get; set; }

        public async Task<JsonResult> OnPostAsync()
        {
            var recordsTotal = _context.SubscriptionActivities.Where(e => e.EventSubscriptionId == eventSubId).Count();

            var customersQuery = _context.SubscriptionActivities.Where(e => e.EventSubscriptionId == eventSubId).Select(c => new
            {
                SubscriptionActivityId = c.SubscriptionActivityId,
                EventActivityTitle = _context.EventActivity_Singles.Where(e => e.EventActivityId == c.EventActivityId).FirstOrDefault().EventActivityTitle,
                EventActivityDescription = _context.EventActivity_Singles.Where(e => e.EventActivityId == c.EventActivityId).FirstOrDefault().EventActivityDescription,

            }).AsQueryable();

            var searchText = DataTablesRequest.Search.Value?.ToUpper();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                customersQuery = customersQuery.Where(s =>
                    s.EventActivityTitle.ToUpper().Contains(searchText) ||
                    s.EventActivityDescription.ToUpper().Contains(searchText)
                );
            }

            var recordsFiltered = customersQuery.Count();

            var sortColumnName = DataTablesRequest.Columns.ElementAt(DataTablesRequest.Order.ElementAt(0).Column).Name;
            var sortDirection = DataTablesRequest.Order.ElementAt(0).Dir.ToLower();

            // using System.Linq.Dynamic.Core
            customersQuery = customersQuery.OrderBy($"{sortColumnName} {sortDirection}");

            var skip = DataTablesRequest.Start;
            var take = DataTablesRequest.Length;
            var data = await customersQuery
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return new JsonResult(new
            {
                draw = DataTablesRequest.Draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            });
        }
    }
}
