using DevExpress.XtraPrinting.Native;
using DevExpress.XtraRichEdit.Import.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Experimental.FileAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using QCO.Models;
using QCO.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;
using X.PagedList.Mvc.Core;




namespace QCO.Controllers
{
    public class CADController : Controller
    {
        private readonly QCOContext _context;
        private readonly OracleContext _oracleContext;
        //private readonly ILogger<LayoutMonitoringSheetsController> _logger;

        public CADController(QCOContext context, OracleContext oracleContext)
        {
            _context = context;
            _oracleContext = oracleContext;

        }
        //public async Task<IActionResult> Index(string search, int? page)
        //{
        //    int pageSize = 16; // Number of records per page
        //    int pageNumber = page ?? 1; // Default to page 1

        //    var query = _context.TblLayoutMonitoringSheets.AsQueryable();

        //    // Apply search filter
        //    if (!string.IsNullOrEmpty(search))
        //    {
        //        query = query.Where(x => x.Company.Contains(search) ||
        //                                 x.BookingNo.Contains(search) ||
        //                                 x.BuyerName.Contains(search) ||
        //                                 x.LineNo.Contains(search));
        //    }

        //    // Ensure the returned data is of type IPagedList
        //    IPagedList<TblLayoutMonitoringSheet> data = query
        //        .OrderByDescending(x => x.MonitoringDate)
        //        .ToPagedList(pageNumber, pageSize);

        //    ViewBag.Search = search; // Store search term in ViewBag

        //    return View(data); // Ensure View receives an IPagedList<T>
        //}

        [HttpGet]
        public IActionResult Index(string search, int? page)
        {
            int pageSize = 16;
            int pageNumber = page ?? 1;

            // Fetch masters with their details from database
            var query = _context.TblCadConsMs
                .Include(m => m.TblCadConsDs) // eager load details
                .AsEnumerable()               // switch to in-memory for LINQ that EF cannot translate
                .Select(m => new CadConsumptionViewModel
                {
                    Master = m,
                    Details = m.TblCadConsDs?.ToList() ?? new List<TblCadConsD>()
                })
                .ToList(); // optional: can skip if using AsEnumerable above

            // Filter if search is provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                //bool? isApprovedSearch = null;
                //var lowerSearch = search.ToLowerInvariant();

                //if (lowerSearch.Contains("1") || lowerSearch.Contains("true") || lowerSearch.Contains("approved"))
                //    isApprovedSearch = true;
                //else if (lowerSearch.Contains("0") || lowerSearch.Contains("false") || lowerSearch.Contains("waiting"))
                //    isApprovedSearch = false;

                query = query.Where(x =>
                    (x.Master.Opt01?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                    (x.Master.Styleref?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                    (x.Master.Styledes?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                    (x.Master.Buyer?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                    (x.Master.Patternmaster?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                    (x.Master.Season?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                    (x.Master.Seasonyear?.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                    (x.Master.Caddate.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    // ✅ convert Isapproved to string
                    (("Approved").Contains(search, StringComparison.OrdinalIgnoreCase) && x.Master.Isapproved == true) ||
                    (("Waiting for approval").Contains(search, StringComparison.OrdinalIgnoreCase) && x.Master.Isapproved == false) ||
                    x.Details.Any(d =>
                        (d.Ptnnmbr?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                        (d.Gmntitem?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                        (d.Fabricdes?.Contains(search, StringComparison.OrdinalIgnoreCase) == true)
                    )
                ).ToList();
            }

            // Order by date descending
            query = query.OrderByDescending(x => x.Master.Caddate).ToList();

            // Set search for ViewBag
            ViewBag.Search = search;

            // Paginate (X.PagedList)
            var data = query.ToPagedList(pageNumber, pageSize);

            return View(data);
        }
        [HttpGet]
        public IActionResult Create()
        {
            // Fetch distinct booking numbers
            var bookingNos = _oracleContext.NewView3
                                           .Where(b => !string.IsNullOrEmpty(b.BOOKING_NO)) // Exclude null or empty values
                                           .Select(b => b.BOOKING_NO.Trim()) // Trim whitespace
                                           .Distinct() // Ensure unique values
                                           .ToList();

            // Pass distinct booking numbers to the view using ViewBag
            ViewBag.BookingNos = new SelectList(bookingNos);

            return View();
        }

        [HttpGet]
        public IActionResult GetBookingNos(string term)
        {
            var searchTerm = term?.Trim().ToUpper();

            if (string.IsNullOrEmpty(searchTerm))
            {
                return Json(new { results = new List<object>() });
            }

            var bookingData = _oracleContext.VW_CAD
                .Where(x => !string.IsNullOrEmpty(x.STYLE_REF_NO) &&
                            x.STYLE_REF_NO.ToUpper().Contains(searchTerm))
                .Select(x => new
                {
                    id = x.STYLE_REF_NO ?? "",
                    text = x.STYLE_REF_NO ?? "",
                    buyerName = x.BUYER_NAME ?? "",
                    jobNo = x.JOB_NO_MST ?? "",
                    styleRef = x.STYLE_REF_NO ?? "",
                    irIb = x.IR_IB ?? "",
                    styleDescription = x.STYLE_DESCRIPTION ?? "",
                    season = x.SEASON_NAME ?? "",
                    seasonYear = x.SEASON_YEAR ?? "",
                    brand = x.BRAND_NAME ?? ""
                })
                .ToList();

            return Json(new { results = bookingData });
        }



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(TblLayoutMonitoringSheet tblLayoutMonitoringSheet)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(tblLayoutMonitoringSheet);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(tblLayoutMonitoringSheet);
        //}

        //[HttpPost]
        //public IActionResult SaveLayoutMonitoringSheet(CadConsumptionViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // ✅ Save Master first
        //    _context.TblCadConsMs.Add(model.Master);
        //    _context.SaveChanges(); // important to get ID

        //    // ✅ Save Details
        //    if (model.Details != null && model.Details.Count > 0)
        //    {
        //        foreach (var item in model.Details)
        //        {
        //            item.Cadmid = model.Master.Cadmid; // FK
        //            item.Transdate = DateTime.Now;

        //            _context.TblCadConsDs.Add(item);
        //        }

        //        _context.SaveChanges();
        //    }

        //    return RedirectToAction("Index");
        //}
        //[HttpPost]
        //public IActionResult Create(CadConsumptionViewModel model)
        //{
        //    //if (ModelState.IsValid)
        //    //{
        //    //    // ✅ ADD THIS BLOCK HERE
        //    //    foreach (var error in ModelState)
        //    //    {
        //    //        foreach (var subError in error.Value.Errors)
        //    //        {
        //    //            Console.WriteLine($"{error.Key} : {subError.ErrorMessage}");
        //    //        }
        //    //    }

        //    //    return View(model);
        //    //}

        //    // Save Master
        //    _context.TblCadConsMs.Add(model.Master);
        //    _context.SaveChanges();

        //    // Save Details
        //    if (model.Details != null && model.Details.Count > 0)
        //    {
        //        foreach (var item in model.Details)
        //        {
        //            item.Cadmid = model.Master.Cadmid;
        //            item.Transdate = DateTime.Now;

        //            _context.TblCadConsDs.Add(item);
        //        }

        //        _context.SaveChanges();
        //    }

        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CadConsumptionViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Save Master
                    _context.TblCadConsMs.Add(model.Master);
                    await _context.SaveChangesAsync();

                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    if (model.Details != null && model.Details.Count > 0)
                    {
                        foreach (var item in model.Details)
                        {
                            item.Cadmid = model.Master.Cadmid;
                            item.Transdate = DateTime.Now;

                            if (item.File != null && item.File.Length > 0)
                            {
                                var fileName = Path.GetFileName(item.File.FileName);
                                var filePath = Path.Combine(folderPath, fileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await item.File.CopyToAsync(stream);
                                }

                                item.Filename = fileName;
                                item.Filepath = $"/uploads/{fileName}";
                                item.Filesize = item.File.Length;
                                item.Contenttype = item.File.ContentType;
                            }

                            _context.TblCadConsDs.Add(item);
                        }

                        await _context.SaveChangesAsync();
                    }

                    TempData["Success"] = "Data saved successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    TempData["Error"] = "Something went wrong while saving data!";
                    return View(model);
                }
            }

            TempData["Error"] = "Validation failed!";
            return View(model);
        }
        // GET: Edit
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Fetch master record
            var master = _context.TblCadConsMs.FirstOrDefault(m => m.Cadmid == id);
            if (master == null)
                return NotFound();

            // Fetch related details
            var details = _context.TblCadConsDs
                                  .Where(d => d.Cadmid == id)
                                  .ToList();

            var model = new CadConsumptionViewModel
            {
                Master = master,
                Details = details
            };

            // Fetch distinct booking numbers for dropdown
            var bookingNos = _oracleContext.NewView3
                                           .Where(b => !string.IsNullOrEmpty(b.BOOKING_NO))
                                           .Select(b => b.BOOKING_NO.Trim())
                                           .Distinct()
                                           .ToList();
            ViewBag.BookingNos = new SelectList(bookingNos, master.Styleref);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CadConsumptionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Please fix validation errors!";
                return View(model);
            }

            try
            {
                // Update Master
                _context.TblCadConsMs.Update(model.Master);
                await _context.SaveChangesAsync();

                // Folder for uploaded files
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // Update Details
                if (model.Details != null && model.Details.Count > 0)
                {
                    foreach (var item in model.Details)
                    {
                        item.Cadmid = model.Master.Cadmid;
                        item.Transdate = DateTime.Now;

                        // File Upload
                        if (item.File != null && item.File.Length > 0)
                        {
                            var fileName = Guid.NewGuid() + Path.GetExtension(item.File.FileName);
                            var filePath = Path.Combine(folderPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await item.File.CopyToAsync(stream);
                            }

                            item.Filename = fileName;
                            item.Filepath = $"/uploads/{fileName}";
                            item.Filesize = item.File.Length;
                            item.Contenttype = item.File.ContentType;
                        }

                        var existingDetail = _context.TblCadConsDs
                                                    .FirstOrDefault(d => d.Caddid == item.Caddid);

                        if (existingDetail != null)
                        {
                            _context.Entry(existingDetail).CurrentValues.SetValues(item);
                        }
                        else
                        {
                            _context.TblCadConsDs.Add(item);
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                TempData["success"] = "Data updated successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = "Something went wrong!";
                // Optional debug
                Console.WriteLine(ex.Message);

                return View(model);
            }
        }

        public IActionResult Details(int id)
        {
            var data = new CadConsumptionViewModel();

            data.Master = _context.TblCadConsMs
                                  .FirstOrDefault(x => x.Cadmid == id);

            if (data.Master == null)
                return NotFound();

            data.Details = _context.TblCadConsDs
                                   .Where(x => x.Cadmid == id)
                                   .ToList();

            return View(data);
        }

        public IActionResult DownloadFile(int id)
        {
            // Find the detail record by its ID
            var fileRecord = _context.TblCadConsDs.FirstOrDefault(d => d.Caddid == id);

            if (fileRecord == null)
                return NotFound("File not found.");

            // Map relative path (/uploads/filename) to physical path
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileRecord.Filename);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File does not exist on server.");

            // Return file for download
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, fileRecord.Contenttype, fileRecord.Filename);
        }
    }
}
