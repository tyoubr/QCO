using DevExpress.XtraPrinting.Native;
using DevExpress.XtraRichEdit.Import.Html;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        //private readonly ILogger<LayoutMonitoringSheetsController> _logger;

        public CADController(QCOContext context, OracleContext oracleContext, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _oracleContext = oracleContext;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index(string search, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            // Fetch masters with their details from database
            //var query = _context.TblCadConsMs
            //    .Include(m => m.TblCadConsDs)
            //    .AsEnumerable()
            //    .Select(m => new CadConsumptionViewModel
            //    {
            //        Master = m,
            //        Details = m.TblCadConsDs?.ToList() ?? new List<TblCadConsD>()
            //    })
            //    .ToList();
            var query = _context.TblCadConsMs
                .Include(m => m.TblCadConsDs)
                .Select(m => new CadConsumptionViewModel
                {
                    Master = m,
                    Details = m.TblCadConsDs.ToList()
                })
                .ToList();

            // Filter if search is provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    (x.Master.Opt01?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                    (x.Master.Styleref?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                    (x.Master.Styledes?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                    (x.Master.Buyer?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                    (x.Master.Patternmaster?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                    //(x.Master.Season?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                    //(x.Master.Seasonyear?.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                    (x.Master.Caddate.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    // ✅ convert Isapproved to string
                    (("Approved").Contains(search, StringComparison.OrdinalIgnoreCase) && x.Master.Isapproved == true) ||
                    (("Pending Approval").Contains(search, StringComparison.OrdinalIgnoreCase) && x.Master.Isapproved == false) ||
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
            .Where(x =>
                (!string.IsNullOrEmpty(x.STYLE_REF_NO) && x.STYLE_REF_NO.ToUpper().Contains(searchTerm)) ||
                (!string.IsNullOrEmpty(x.IR_IB) && x.IR_IB.ToUpper().Contains(searchTerm)) ||
                (!string.IsNullOrEmpty(x.JOB_NO_MST) && x.JOB_NO_MST.ToUpper().Contains(searchTerm))
            )
            .Select(x => new
            {
                id = (x.IR_IB ?? "") + " - " + (x.JOB_NO_MST ?? "") + " - " + (x.STYLE_REF_NO ?? ""),

                text = (x.IR_IB ?? "") + " - " + (x.JOB_NO_MST ?? "") + " - " + (x.STYLE_REF_NO ?? ""),

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

        [HttpGet]
        public IActionResult GetBookingDetails(string jobNo)
        {
            var data = _oracleContext.VW_BOOKING_DETAILS
                        .Where(x => x.JOB_NO == jobNo)
                        .Select(x => new
                        {
                            garmentsItem = x.GARMENTS_ITEM,
                            color = x.COLOR_NAME,
                            fabricDescription = x.FABRIC_DESCRIPTION,
                            gsm = x.GSM_WEIGHT,
                            fabricUsage = x.BODY_PARTS

                        })
                        .ToList();

            return Json(data);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(CadConsumptionViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            // Save Master
        //            _context.TblCadConsMs.Add(model.Master);
        //            await _context.SaveChangesAsync();

        //            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        //            if (!Directory.Exists(folderPath))
        //                Directory.CreateDirectory(folderPath);

        //            if (model.Details != null && model.Details.Count > 0)
        //            {
        //                foreach (var item in model.Details)
        //                {
        //                    item.Cadmid = model.Master.Cadmid;
        //                    item.Transdate = DateTime.Now;

        //                    if (item.File != null && item.File.Length > 0)
        //                    {
        //                        var fileName = Path.GetFileName(item.File.FileName);
        //                        var filePath = Path.Combine(folderPath, fileName);

        //                        using (var stream = new FileStream(filePath, FileMode.Create))
        //                        {
        //                            await item.File.CopyToAsync(stream);
        //                        }

        //                        item.Filename = fileName;
        //                        item.Filepath = $"/uploads/{fileName}";
        //                        item.Filesize = item.File.Length;
        //                        item.Contenttype = item.File.ContentType;
        //                    }

        //                    _context.TblCadConsDs.Add(item);
        //                }

        //                await _context.SaveChangesAsync();
        //            }

        //            TempData["Success"] = "Data saved successfully!";
        //            return RedirectToAction("Index");
        //        }
        //        catch (Exception)
        //        {
        //            TempData["Error"] = "Something went wrong while saving data!";
        //            return View(model);
        //        }
        //    }

        //    TempData["Error"] = "Validation failed!";
        //    return View(model);
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

                    // ✅ Use wwwroot/UploadFiles instead of network path
                    var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "UploadFiles");

                    // Ensure directory exists
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
                                // Make filename unique
                                var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(item.File.FileName);

                                var filePath = Path.Combine(folderPath, fileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await item.File.CopyToAsync(stream);
                                }

                                item.Filename = fileName;

                                // ✅ Store relative path (better for web access)
                                item.Filepath = "/UploadFiles/" + fileName;

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

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(CadConsumptionViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        TempData["error"] = "Please fix validation errors!";
        //        return View(model);
        //    }

        //    try
        //    {
        //        // Update Master
        //        _context.TblCadConsMs.Update(model.Master);
        //        await _context.SaveChangesAsync();

        //        // Folder for uploaded files
        //        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        //        if (!Directory.Exists(folderPath))
        //            Directory.CreateDirectory(folderPath);

        //        // Update Details
        //        if (model.Details != null && model.Details.Count > 0)
        //        {
        //            foreach (var item in model.Details)
        //            {
        //                item.Cadmid = model.Master.Cadmid;
        //                item.Transdate = DateTime.Now;

        //                // File Upload
        //                if (item.File != null && item.File.Length > 0)
        //                {
        //                    var fileName = Guid.NewGuid() + Path.GetExtension(item.File.FileName);
        //                    var filePath = Path.Combine(folderPath, fileName);

        //                    using (var stream = new FileStream(filePath, FileMode.Create))
        //                    {
        //                        await item.File.CopyToAsync(stream);
        //                    }

        //                    item.Filename = fileName;
        //                    item.Filepath = $"/uploads/{fileName}";
        //                    item.Filesize = item.File.Length;
        //                    item.Contenttype = item.File.ContentType;
        //                }

        //                var existingDetail = _context.TblCadConsDs
        //                                            .FirstOrDefault(d => d.Caddid == item.Caddid);

        //                if (existingDetail != null)
        //                {
        //                    _context.Entry(existingDetail).CurrentValues.SetValues(item);
        //                }
        //                else
        //                {
        //                    _context.TblCadConsDs.Add(item);
        //                }
        //            }

        //            await _context.SaveChangesAsync();
        //        }

        //        TempData["success"] = "Data updated successfully!";
        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["error"] = "Something went wrong!";
        //        // Optional debug
        //        Console.WriteLine(ex.Message);

        //        return View(model);
        //    }
        //}

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
                // =========================
                // UPDATE MASTER
                // =========================
                model.Master.UpdatedAt = DateTime.Now;
                model.Master.UpdatedBy = User.Identity?.Name ?? "System";

                _context.TblCadConsMs.Update(model.Master);
                await _context.SaveChangesAsync();

                // =========================
                // LOCAL wwwroot PATH
                // =========================
                var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "UploadFiles");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // =========================
                // UPDATE DETAILS
                // =========================
                if (model.Details != null && model.Details.Count > 0)
                {
                    foreach (var item in model.Details)
                    {
                        var existingDetail = _context.TblCadConsDs
                            .FirstOrDefault(d => d.Caddid == item.Caddid);

                        if (existingDetail != null)
                        {
                            // =========================
                            // UPDATE FIELDS
                            // =========================
                            existingDetail.Cadmid = model.Master.Cadmid;
                            existingDetail.Transdate = DateTime.Now;

                            existingDetail.Ptnnmbr = item.Ptnnmbr;
                            existingDetail.Gmntitem = item.Gmntitem;
                            existingDetail.Gmntcolor = item.Gmntcolor;
                            existingDetail.Fabricdes = item.Fabricdes;
                            existingDetail.Fabricusage = item.Fabricusage;
                            existingDetail.Gsm = item.Gsm;
                            existingDetail.Opt01 = item.Opt01;
                            existingDetail.Fullwidth = item.Fullwidth;
                            existingDetail.Cutwidth = item.Cutwidth;
                            existingDetail.Efficiency = item.Efficiency;
                            existingDetail.Sizeratio = item.Sizeratio;
                            existingDetail.Markerqty = item.Markerqty;
                            existingDetail.Conspcs = item.Conspcs;
                            existingDetail.Consdzn = item.Consdzn;
                            existingDetail.Wastage = item.Wastage;
                            existingDetail.Comments = item.Comments;

                            // =========================
                            // FILE UPDATE
                            // =========================
                            if (item.File != null && item.File.Length > 0)
                            {
                                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(item.File.FileName)}";
                                var filePath = Path.Combine(folderPath, fileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await item.File.CopyToAsync(stream);
                                }

                                existingDetail.Filename = fileName;

                                // ✅ Store relative path
                                existingDetail.Filepath = "/UploadFiles/" + fileName;

                                existingDetail.Filesize = item.File.Length;
                                existingDetail.Contenttype = item.File.ContentType;
                            }
                            // else → old file থাকবে
                        }
                        else
                        {
                            // =========================
                            // INSERT NEW ROW
                            // =========================
                            item.Cadmid = model.Master.Cadmid;
                            item.Transdate = DateTime.Now;

                            if (item.File != null && item.File.Length > 0)
                            {
                                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(item.File.FileName)}";
                                var filePath = Path.Combine(folderPath, fileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await item.File.CopyToAsync(stream);
                                }

                                item.Filename = fileName;

                                // ✅ Store relative path
                                item.Filepath = "/UploadFiles/" + fileName;

                                item.Filesize = item.File.Length;
                                item.Contenttype = item.File.ContentType;
                            }

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
                Console.WriteLine(ex.Message);
                return View(model);
            }
        }

        [HttpGet]
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

        [HttpGet]
        public IActionResult DownloadFile(int id)
        {
            // Find the detail record
            var fileRecord = _context.TblCadConsDs.FirstOrDefault(d => d.Caddid == id);

            if (fileRecord == null)
                return NotFound("File not found.");

            // =========================
            // NETWORK FILE PATH
            // =========================
            var folderPath = @"\\103.9.134.216\UploadFiles";
            var filePath = Path.Combine(folderPath, fileRecord.Filename);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File does not exist on server.");

            // Return file for download
            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, fileRecord.Contenttype, fileRecord.Filename);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await _context.TblCadConsMs
                .Include(m => m.TblCadConsDs)
                .FirstOrDefaultAsync(m => m.Cadmid == id);

            if (data == null)
            {
                TempData["Error"] = "Record not found!";
                return RedirectToAction("Index");
            }

            var model = new CadConsumptionViewModel
            {
                Master = data,
                Details = data.TblCadConsDs.ToList()
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var master = await _context.TblCadConsMs
                    .Include(m => m.TblCadConsDs)
                    .FirstOrDefaultAsync(m => m.Cadmid == id);

                if (master == null)
                {
                    TempData["Error"] = "Record not found!";
                    return RedirectToAction("Index");
                }

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (master.TblCadConsDs != null && master.TblCadConsDs.Count > 0)
                {
                    foreach (var detail in master.TblCadConsDs)
                    {
                        if (!string.IsNullOrEmpty(detail.Filepath))
                        {
                            var fullPath = Path.Combine(
                                Directory.GetCurrentDirectory(),
                                "wwwroot",
                                detail.Filepath.TrimStart('/'));

                            if (System.IO.File.Exists(fullPath))
                            {
                                System.IO.File.Delete(fullPath);
                            }
                        }
                    }

                    _context.TblCadConsDs.RemoveRange(master.TblCadConsDs);
                }

                _context.TblCadConsMs.Remove(master);

                await _context.SaveChangesAsync();

                TempData["Success"] = "Data deleted successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error occurred while deleting data!";
                Console.WriteLine(ex.Message);
                return RedirectToAction("Index");
            }
        }
    }
}
