using Conference.Data;
using Conference.DataTables;
using Conference.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Linq.Dynamic.Core;

namespace Conference.Areas.Admin.Pages.EventAudienceActivites
{
    public class IndexModel : PageModel
    {
        private ConferenceContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;


        public string url { get; set; }


        [BindProperty]
        public EventActivity_Single EventActivitySingleRecord { get; set; }


        public List<EventActivity_Single> EventActivity_SingleList = new List<EventActivity_Single>();

        public EventActivity_Single EventActivitySingleObj { get; set; }

        public IndexModel(ConferenceContext context, IWebHostEnvironment hostEnvironment,
                                            IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
			EventActivitySingleRecord = new EventActivity_Single();
			EventActivitySingleObj = new EventActivity_Single();
        }
       
        [BindProperty]
        public DataTablesRequest DataTablesRequest { get; set; }

        public async Task<JsonResult> OnPostAsync()
        {
            var records = _context.EventActivity_Singles;
            //var records = _context.EventActivity_Singles.Where(a => a.EventActivityType == 1).Include(a => a.Event);
            var recordsTotal = records.Count();

            var customersQuery = records.AsQueryable();

            var searchText = DataTablesRequest.Search.Value?.ToUpper();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                customersQuery = customersQuery.Where(s =>
                    s.EventActivityTitle.ToUpper().Contains(searchText) ||
                    s.EventActivityDescription.ToUpper().Contains(searchText) ||
                    s.Event.EventTitle.ToUpper().Contains(searchText)
                );
            }

            var recordsFiltered = customersQuery.Count();

            var sortColumnName = DataTablesRequest.Columns.ElementAt(DataTablesRequest.Order.ElementAt(0).Column).Name;
            var sortDirection = DataTablesRequest.Order.ElementAt(0).Dir.ToLower();

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
                recordsTotal,
                recordsFiltered,
                data
            });
        }

        

    }
}
