using Conference.Data;
using Conference.DataTables;
using Conference.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Linq.Dynamic.Core;

namespace Conference.Areas.Admin.Pages.EventSubscriptions
{
    public class IndexModel : PageModel
    {
        private ConferenceContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;


        public string url { get; set; }
        public static int eventId = 0;


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
            var eventObj = _context.Events.Where(e=>e.EventId== id).FirstOrDefault();
            if (eventObj != null)
            {
                eventId = id;
            }
        }
        [BindProperty]
        public DataTablesRequest DataTablesRequest { get; set; }

        public async Task<JsonResult> OnPostAsync()
        {
            var recordsTotal = _context.EventSubscriptions.Where(e=>e.EventId==eventId).Count();

            var customersQuery = _context.EventSubscriptions.Where(e=>e.EventId == eventId).Select(c => new
            {
                EventSubscriptionId = c.EventSubscriptionId,
                EventSubscriptionQueue = c.EventSubscriptionQueue,
                EventSubscriptionDate = c.EventSubscriptionDate,
                EventSubscriptionMobile = c.EventSubscriptionMobile,
                EventSubscriptionFullName = c.EventSubscriptionFullName,
                EventId = c.EventId,
                EventSubscriptionType = c.EventSubscriptionType == 1 ? "خريج" : "جمهور",
            }).AsQueryable();

            var searchText = DataTablesRequest.Search.Value?.ToUpper();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                customersQuery = customersQuery.Where(s =>
                    s.EventSubscriptionFullName.ToUpper().Contains(searchText) ||
                    s.EventSubscriptionMobile.ToUpper().Contains(searchText)
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

        //    public IActionResult OnGetSingleCategoryForEdit(int EventId)
        //    {
        //        eventRecord = _context.Events.Where(c => c.EventId == EventId).FirstOrDefault();

        //        return new JsonResult(eventRecord);
        //    }

        //    public IActionResult OnGetSingleSiteCategoriesForView(int EventId)
        //    {
        //        var Result = _context.Events.Where(c => c.EventId == EventId).FirstOrDefault();
        //        return new JsonResult(Result);
        //    }

        //    public async Task<IActionResult> OnPostEditEvent(int EventId, IFormFile Editfile)
        //    {
        //        try
        //        {
        //            var model = _context.Events.Where(c => c.EventId == EventId).FirstOrDefault();
        //            if (model == null)
        //            {
        //                _toastNotification.AddErrorToastMessage("Event Not Found");

        //                return Redirect("/Admin/Events/Index");
        //            }


        //            if (Editfile != null)
        //            {


        //                string folder = "Images/Event/";

        //                model.EventVideo = UploadImage(folder, Editfile);
        //            }
        //            else
        //            {
        //                model.EventVideo = eventRecord.EventVideo;
        //            }


        //            model.EventIsActive = eventRecord.EventIsActive;
        //            model.EventTitle = eventRecord.EventTitle;
        //            model.EventLocation = eventRecord.EventLocation;
        //            model.EventDate = eventRecord.EventDate;
        //            model.EventBarcode = eventRecord.EventBarcode;

        //            var UpdatedEvent= _context.Events.Attach(model);

        //            UpdatedEvent.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

        //            _context.SaveChanges();

        //            _toastNotification.AddSuccessToastMessage("Category Edited successfully");

        //            return Redirect("/Admin/Events/Index");

        //        }
        //        catch (Exception)
        //        {
        //            _toastNotification.AddErrorToastMessage("Something went Error");

        //        }
        //        return Redirect("/Admin/Configurations/SiteCategories/Index");
        //    }

        //    public async Task<IActionResult> OnPostAddEvent(IFormFile file)
        //    {
        //        try
        //        {
        //if (file != null)
        //{


        //	string folder = "Images/Event/";

        //	eventRecord.EventVideo = UploadImage(folder, file);
        //}

        //_context.Events.Add(eventRecord);
        //            _context.SaveChanges();
        //            _toastNotification.AddSuccessToastMessage("Event Added Successfully");

        //        }
        //        catch (Exception)
        //        {

        //            _toastNotification.AddErrorToastMessage("Something went wrong");
        //        }
        //        return Redirect("/Admin/Events/Index");
        //    }

        //    public IActionResult OnGetSingleEventForDelete(int EventId)
        //    {
        //        eventRecord = _context.Events.Where(c => c.EventId == EventId).FirstOrDefault();
        //        return new JsonResult(eventRecord);
        //    }

        //    public async Task<IActionResult> OnPostCategoryDelete(int EventId)
        //    {
        //        try
        //        {
        //            Event CatObj = _context.Events.Where(e => e.EventId == EventId).FirstOrDefault();
        //            if (CatObj != null)
        //            {
        //                _context.Events.Remove(CatObj);
        //                await _context.SaveChangesAsync();
        //                _toastNotification.AddSuccessToastMessage("Event Deleted successfully");
        //            }
        //            else
        //            {
        //                _toastNotification.AddErrorToastMessage("Something went wrong Try Again");
        //            }
        //        }
        //        catch (Exception)

        //        {
        //            _toastNotification.AddErrorToastMessage("Something went wrong");
        //        }

        //        return Redirect("/Admin/Event/Index");
        //    }

        private string UploadImage(string folderPath, IFormFile file)
        {

            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }
    }
}
