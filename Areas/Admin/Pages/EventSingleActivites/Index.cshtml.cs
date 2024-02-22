using Conference.Data;
using Conference.DataTables;
using Conference.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using QRCoder;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq.Dynamic.Core;

namespace Conference.Areas.Admin.Pages.EventSingleActivites
{
    public class IndexModel : PageModel
    {
        private ConferenceContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;


        public string url { get; set; }
        public static int eventId = 0;

        [BindProperty]
        public int EventIdVar { get; set; }
        [BindProperty]
        public EventActivity_Single EventActivitySingleRecord { get; set; }


        public List<EventActivity_Single> EventActivity_SingleList = new List<EventActivity_Single>();

        public EventActivity_Single EventActivitySingleObj { get; set; }
        [BindProperty]
        public DataTablesRequest DataTablesRequest { get; set; }
      

        public IndexModel(ConferenceContext context, IWebHostEnvironment hostEnvironment,
                                            IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
			EventActivitySingleRecord = new EventActivity_Single();
			EventActivitySingleObj = new EventActivity_Single();
        }

        public void OnGet(int id)
        {
            var eventObj = _context.Events.Where(e => e.EventId == id).FirstOrDefault();
            if (eventObj != null)
            {
                eventId = id;
                EventIdVar = id;
            }
        }
        public async Task<JsonResult> OnPostAsync()
        {
            var records = _context.EventActivity_Singles.Where(e=>e.EventId==eventId).ToList();
            //var records = _context.EventActivity_Singles.Where(a => a.EventActivityType == 1).Include(a => a.Event);
            //var recordsTotal = records.Count();
            var customersQuery = _context.EventActivity_Singles.Where(e => e.EventId == eventId).Include(c => c.Event).Select(c => new
            {
                EventActivityId = c.EventActivityId,
                EventTitle = c.Event.EventTitle,
                EventActivityTitle = c.EventActivityTitle,
                EventActivityDescription = c.EventActivityDescription,
                EventActivityType = c.EventActivityType == 1 ? "خريج" : "جمهور",
            }).AsQueryable();
            var recordsTotal = records.Count();
            //var customersQuery = records.AsQueryable();

            var searchText = DataTablesRequest.Search.Value?.ToUpper();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                customersQuery = customersQuery.Where(s =>
                    s.EventActivityTitle.ToUpper().Contains(searchText) ||
                    s.EventActivityDescription.ToUpper().Contains(searchText) ||
                   // s.Event.EventTitle.ToUpper().Contains(searchText)
                    s.EventTitle.ToUpper().Contains(searchText)
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

        public IActionResult OnGetSingleEventActivityForEdit(int EventActivityId)
        {
            EventActivitySingleRecord = _context.EventActivity_Singles.Where(c => c.EventActivityId == EventActivityId).FirstOrDefault();

            return new JsonResult(EventActivitySingleRecord);
        }

        public IActionResult OnGetSingleEventActivityForView(int EventActivityId)
        {
            var Result = _context.EventActivity_Singles.Include(c=>c.Event).Where(c => c.EventActivityId == EventActivityId).Select(i => new
            {
                EventActivityId= i.EventActivityId,
                EventActivityTitle = i.EventActivityTitle,
                EventActivityDescription = i.EventActivityDescription,
                EventTitle = i.Event.EventTitle,
                EventActivityType = i.EventActivityType==1?"خريج":"جمهور",

                
            }).FirstOrDefault();
            return new JsonResult(Result);
        }
        public IActionResult OnGetSingleEventQRCodeForView(int EventActivityId)
        {
            var Result = _context.EventActivity_Singles.Where(c => c.EventActivityId == EventActivityId).FirstOrDefault();
            return new JsonResult(Result);
        }

        public async Task<IActionResult> OnPostEditEventActivity(int EventActivityId)
        {
            try
            {
                var model = _context.EventActivity_Singles.Where(c => c.EventActivityId == EventActivityId).FirstOrDefault();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Event Not Found");

                    return Redirect($"/Admin/EventSingleActivites/Index?id={eventId}");
                }



                model.EventActivityTitle = EventActivitySingleRecord.EventActivityTitle;
                model.EventActivityDescription = EventActivitySingleRecord.EventActivityDescription;
                model.EventActivityType = EventActivitySingleRecord.EventActivityType;
                model.EventId = EventActivitySingleRecord.EventId;
               

                var UpdatedEvent = _context.EventActivity_Singles.Attach(model);

                UpdatedEvent.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                _context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("Evente Activity Edited successfully");

                return Redirect($"/Admin/EventSingleActivites/Index?id={eventId}");

            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Redirect($"/Admin/EventSingleActivites/Index?id={eventId}");
        }

        public async Task<IActionResult> OnPostAddEventActivity()
        {
            try
            {

                EventActivitySingleRecord.EventId = eventId;
                _context.EventActivity_Singles.Add(EventActivitySingleRecord);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Event Activity Added Successfully");


            }
            catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
            return Redirect($"/Admin/EventSingleActivites/Index?id={eventId}");
        }
      
        public IActionResult OnGetSingleEventActivityForDelete(int EventActivityId)
        {
            EventActivitySingleRecord = _context.EventActivity_Singles.Where(c => c.EventActivityId == EventActivityId).FirstOrDefault();
            return new JsonResult(EventActivitySingleRecord);
        }

        public async Task<IActionResult> OnPostEventActivityDelete(int EventActivityId)
        {
            try
            {
                EventActivity_Single EventAcObj = _context.EventActivity_Singles.Where(e => e.EventActivityId == EventActivityId).FirstOrDefault();
                if (EventAcObj != null)
                {
                  
                    _context.EventActivity_Singles.Remove(EventAcObj);
                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Event Activity Deleted successfully");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Something went wrong Try Again");
                }
            }
            catch (Exception)

            {
                _toastNotification.AddErrorToastMessage("Something went wrong");
            }

            return Redirect($"/Admin/EventSingleActivites/Index?id={eventId}");
        }


    }
}
