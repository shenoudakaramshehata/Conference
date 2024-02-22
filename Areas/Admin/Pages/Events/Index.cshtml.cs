using Conference.Data;
using Conference.DataTables;
using Conference.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Drawing;
using NToastNotify;
using System.Linq.Dynamic.Core;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Imaging;

namespace Conference.Areas.Admin.Pages.Configurations.SiteCategories
{
    public class IndexModel : PageModel
    {
        private ConferenceContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
		public string QRScan { get; set; }

		public string url { get; set; }
        public string QRCodeText { get; set; }


        [BindProperty]
        public Event eventRecord { get; set; }


        public List<Event> EventList = new List<Event>();

        public Event eventObj { get; set; }
        [BindProperty]
        public DataTablesRequest DataTablesRequest { get; set; }
        public IndexModel(ConferenceContext context, IWebHostEnvironment hostEnvironment,
                                            IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            eventRecord = new Event();
            eventObj = new Event();
        }


        public void OnGet()
        {
            url = $"{this.Request.Scheme}://{this.Request.Host}";
        }
        public async Task<JsonResult> OnPostAsync()
        {
            var recordsTotal = _context.Events.Count();

            var customersQuery = _context.Events.Select(c => new
            {
                EventId = c.EventId,
                EventLocation = c.EventLocation,
                EventTitle = c.EventTitle,
                EventVideo = c.EventVideo,
                EventDate = c.EventDate.Value.ToShortDateString().ToString(),
                EventBarcode = c.EventBarcode,
                EventIsActive = c.EventIsActive,
                QRCodeImage = c.QRCodeImage,
                QRCodeName = c.QRCodeName,
            }).AsQueryable();
          

            var searchText = DataTablesRequest.Search.Value?.ToUpper();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                customersQuery = customersQuery.Where(s =>
                    s.EventTitle.ToUpper().Contains(searchText) ||
                    s.EventLocation.ToUpper().Contains(searchText) 
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

        public IActionResult OnGetSingleEventForEdit(int EventId)
        {
            eventRecord = _context.Events.Where(c => c.EventId == EventId).FirstOrDefault();

            return new JsonResult(eventRecord);
        }

        public IActionResult OnGetSingleEventForView(int EventId)
        {
            var Result = _context.Events.Where(c => c.EventId == EventId).Select(c => new
            {
                EventId = c.EventId,
                EventLocation = c.EventLocation,
                EventTitle = c.EventTitle,
                EventVideo = c.EventVideo,
                EventDate = c.EventDate.Value.ToShortDateString().ToString(),
                EventBarcode = c.EventBarcode,
                EventIsActive = c.EventIsActive,
                QRCodeImage = c.QRCodeImage,
                QRCodeName = c.QRCodeName,
                AudianceImage = c.AudianceImage,
                IndividualImage = c.IndividualImage,
                QueueImage = c.QueueImage,
            }).FirstOrDefault();
            return new JsonResult(Result);
        }
        public IActionResult OnGetSingleEventQRCodeForView(int EventId)
        {
            var Result = _context.Events.Where(c => c.EventId == EventId).FirstOrDefault();
            return new JsonResult(Result);
        }

        public async Task<IActionResult> OnPostEditEvent(int EventId, IFormFile Editfile, IFormFile EditIndfile, IFormFile EditAudfile, IFormFile EditQuefile)
        {
            try
            {
                var model = _context.Events.Where(c => c.EventId == EventId).FirstOrDefault();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Event Not Found");

                    return Redirect("/Admin/Events/Index");
                }


                if (Editfile != null)
                {


                    string folder = "Images/Event/";

                    model.EventVideo = UploadImage(folder, Editfile);
                }
                else
                {
                    model.EventVideo = eventRecord.EventVideo;
                }
				if (EditIndfile != null)
				{


					string folder = "Images/Event/";

					model.IndividualImage = UploadImage(folder, EditIndfile);
				}
				else
				{
					model.IndividualImage = eventRecord.IndividualImage;
				}
				if (EditAudfile != null)
				{


					string folder = "Images/Event/";

					model.AudianceImage = UploadImage(folder, EditAudfile);
				}
				else
				{
					model.AudianceImage = eventRecord.AudianceImage;
				}
				if (EditQuefile != null)
				{


					string folder = "Images/Event/";

					model.QueueImage = UploadImage(folder, EditQuefile);
				}
				else
				{
					model.QueueImage = eventRecord.QueueImage;
				}


				model.EventIsActive = eventRecord.EventIsActive;
                model.EventTitle = eventRecord.EventTitle;
                model.EventLocation = eventRecord.EventLocation;
                model.EventDate = eventRecord.EventDate;
                model.EventBarcode = eventRecord.EventBarcode;
                model.QRCodeImage = eventRecord.QRCodeImage;
                model.QRCodeName = eventRecord.QRCodeName;

                var UpdatedEvent = _context.Events.Attach(model);

                UpdatedEvent.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                _context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("Event Edited successfully");

                return Redirect("/Admin/Events/Index");

            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Redirect("/Admin/Events/Index");
        }

        public async Task<IActionResult> OnPostAddEvent(IFormFile file, IFormFile Indfile, IFormFile Audfile, IFormFile Quefile)
        {
            try
            {
                if (file != null)
                {


                    string folder = "Images/Event/";

                    eventRecord.EventVideo = UploadImage(folder, file);
                }
                if (Indfile != null)
                {


                    string folder = "Images/Event/";

                    eventRecord.IndividualImage = UploadImage(folder, Indfile);
                }
                if (Audfile != null)
                {


                    string folder = "Images/Event/";

                    eventRecord.AudianceImage = UploadImage(folder, Audfile);
                }
                if (Quefile != null)
                {


                    string folder = "Images/Event/";

                    eventRecord.QueueImage = UploadImage(folder, Quefile);
                }
                //eventRecord.EventDate = DateTime.Now;
                _context.Events.Add(eventRecord);
				_context.SaveChanges();
                GenerateBarCode(eventRecord.EventId);
				_toastNotification.AddSuccessToastMessage("Event Added Successfully");


			}
			catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
            return Redirect("/Admin/Events/Index");
        }
		public void GenerateBarCode(int EventId)
		{
            var Event = _context.Events.Where(e => e.EventId == EventId).FirstOrDefault();
            if (Event != null)
            {
                Event.EventBarcode = EventId.ToString();

			}
			QRCodeText = $"{this.Request.Scheme}://{this.Request.Host}/?Id="+EventId;
			Event.QRCodeName = QRCodeText;
            //Start//
            //QRCodeGenerator QrGenerator = new QRCodeGenerator();
            //QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(QRCodeText, QRCodeGenerator.ECCLevel.Q);
            //QRCode QrCode = new QRCode(QrCodeInfo);
            //         Bitmap QrBitmap = QrCode.GetGraphic(60);
            //         byte[] BitmapArray = QrBitmap.BitmapToByteArray();
            //         QRScan = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));
            //         Event.QRCodeImage = QRScan;
            //         var UpdatedEvent = _context.Events.Attach(Event);
            //         UpdatedEvent.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            //         _context.SaveChanges();
            //End//

            //Try

            QRCodeGenerator QrGenerator = new QRCodeGenerator();
            QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(QRCodeText, QRCodeGenerator.ECCLevel.Q);
            QRCode QrCode = new QRCode(QrCodeInfo);
            Bitmap QrBitmap = QrCode.GetGraphic(60);
            byte[] BitmapArray = QrBitmap.BitmapToByteArray();
            string uploadFolder = Path.Combine(_hostEnvironment.WebRootPath, "Images/Event");
            string uniqePictureName = Guid.NewGuid() + ".jpeg";
            string uploadedImagePath = Path.Combine(uploadFolder, uniqePictureName);
            using (var imageFile = new FileStream(uploadedImagePath, FileMode.Create))
            {
                imageFile.Write(BitmapArray, 0, BitmapArray.Length);
                imageFile.Flush();
            }
            //nurseryMember.Banner = uniqePictureName;
            Event.QRCodeImage = "Images/Event/"+uniqePictureName;
            var UpdatedEvent = _context.Events.Attach(Event);
            UpdatedEvent.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
             _context.SaveChanges();



        }
        public IActionResult OnGetSingleEventForDelete(int EventId)
        {
            eventRecord = _context.Events.Where(c => c.EventId == EventId).FirstOrDefault();
            return new JsonResult(eventRecord);
        }

        public async Task<IActionResult> OnPostEventDelete(int EventId)
        {
            try
            {
                Event EventObj = _context.Events.Where(e => e.EventId == EventId).FirstOrDefault();
                if (EventObj != null)
                {
                    var SingleEvList = _context.EventActivity_Singles.Where(e => e.EventId == EventId).ToList();
                    if (SingleEvList != null)
                    {
                        _context.EventActivity_Singles.RemoveRange(SingleEvList);

                    }
                    var SubscriptionEvList = _context.EventSubscriptions.Where(e => e.EventId == EventId).ToList();
                    if (SubscriptionEvList != null)
                    {
                        foreach (var item in SubscriptionEvList)
                        {
                            var ActivitesList = _context.SubscriptionActivities.Where(e => e.EventSubscriptionId == item.EventSubscriptionId).ToList();
                            if(ActivitesList!=null)
                            {
                                _context.SubscriptionActivities.RemoveRange(ActivitesList);
                            }
                        }
                        _context.EventSubscriptions.RemoveRange(SubscriptionEvList);

                    }
                    _context.Events.Remove(EventObj);
                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Event Deleted successfully");
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

            return Redirect("/Admin/Events/Index");
        }


        private string UploadImage(string folderPath, IFormFile file)
        {

            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }

    }
}
