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
        public async Task<IActionResult> Index(string? search, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            // =====================================================
            // BASE QUERY
            // =====================================================
            var query = _context.TblCadConsMs
                .AsNoTracking()
                .Include(m => m.TblCadConsDs)
                .AsQueryable();


            // =====================================================
            // SEARCH
            // =====================================================
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(x =>
                    // ---------------------------------------------
                    // MASTER SEARCH
                    // ---------------------------------------------
                    (x.Opt01 != null &&
                     x.Opt01.Contains(search)) ||

                    (x.Styleref != null &&
                     x.Styleref.Contains(search)) ||

                    (x.Styledes != null &&
                     x.Styledes.Contains(search)) ||

                    (x.Buyer != null &&
                     x.Buyer.Contains(search)) ||

                    (x.Patternmaster != null &&
                     x.Patternmaster.Contains(search)) ||

                    // ---------------------------------------------
                    // DATE SEARCH
                    // ---------------------------------------------
                    (x.Caddate != null &&
                     x.Caddate.ToString().Contains(search)) ||

                    // ---------------------------------------------
                    // APPROVAL STATUS
                    // ---------------------------------------------
                    (search.ToLower() == "approved" &&
                     x.Isapproved == true) ||

                    (search.ToLower() == "pending approval" &&
                     x.Isapproved == false) ||

                    // ---------------------------------------------
                    // DETAIL SEARCH
                    // ---------------------------------------------
                    x.TblCadConsDs.Any(d =>
                        (d.Ptnnmbr != null &&
                         d.Ptnnmbr.Contains(search)) ||

                        (d.Gmntitem != null &&
                         d.Gmntitem.Contains(search)) ||

                        (d.Fabricdes != null &&
                         d.Fabricdes.Contains(search))
                    )
                );
            }


            // =====================================================
            // ORDER BY DATE
            // =====================================================
            query = query
                .OrderByDescending(x => x.Caddate);


            // =====================================================
            // FETCH DATA
            // =====================================================
            var masters = await query.ToListAsync();


            // =====================================================
            // CREATE VIEW MODELS
            // =====================================================
            var data = masters.Select(master => new CadConsumptionViewModel
            {
                Master = master,

                Details = master.TblCadConsDs
                    .Select(detail => new CadConsumptionDetailViewModel
                    {
                        Caddid = detail.Caddid,
                        Cadmid = detail.Cadmid,
                        Transdate = detail.Transdate,

                        Ptnnmbr = detail.Ptnnmbr,
                        Gmntitem = detail.Gmntitem,
                        Gmntcolor = detail.Gmntcolor,
                        Fabricdes = detail.Fabricdes,
                        Fabricusage = detail.Fabricusage,

                        Gsm = detail.Gsm,
                        Opt01 = detail.Opt01,
                        Fullwidth = detail.Fullwidth,
                        Cutwidth = detail.Cutwidth,
                        Efficiency = detail.Efficiency,

                        Sizeratio = detail.Sizeratio,

                        Markerqty = detail.Markerqty,
                        Conspcs = detail.Conspcs,
                        Consdzn = detail.Consdzn,

                        Wastage = detail.Wastage,

                        Comments = detail.Comments
                    })
                    .ToList()

            }).ToList();


            // =====================================================
            // SEARCH VALUE FOR VIEW
            // =====================================================
            ViewBag.Search = search;


            // =====================================================
            // PAGINATION
            // =====================================================
            var pagedData = data.ToPagedList(
                pageNumber,
                pageSize
            );


            return View(pagedData);
        }


        [HttpGet]
        public IActionResult Create()
        {
            // =========================
            // FETCH DISTINCT BOOKING NOS
            // =========================
            var bookingNos = _oracleContext.NewView3
                .Where(b => !string.IsNullOrEmpty(b.BOOKING_NO))
                .Select(b => b.BOOKING_NO.Trim())
                .Distinct()
                .ToList();

            ViewBag.BookingNos = new SelectList(bookingNos);

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CadConsumptionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Validation failed!";
                return View(model);
            }

            try
            {
                // =====================================================
                // 1. SAVE MASTER
                // =====================================================
                _context.TblCadConsMs.Add(model.Master);

                await _context.SaveChangesAsync();


                // =====================================================
                // 2. UPLOAD FOLDER
                // =====================================================
                var folderPath = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "UploadFiles"
                );

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }


                // =====================================================
                // 3. SAVE DETAILS
                // =====================================================
                if (model.Details != null && model.Details.Count > 0)
                {
                    foreach (var item in model.Details)
                    {
                        // =================================================
                        // CREATE DETAIL
                        // =================================================
                        var detail = new TblCadConsD
                        {
                            Cadmid = model.Master.Cadmid,
                            Transdate = DateTime.Now,

                            Ptnnmbr = item.Ptnnmbr,
                            Gmntitem = item.Gmntitem,
                            Gmntcolor = item.Gmntcolor,
                            Fabricdes = item.Fabricdes,
                            Fabricusage = item.Fabricusage,

                            Gsm = item.Gsm,

                            Opt01 = item.Opt01,

                            Fullwidth = item.Fullwidth,
                            Cutwidth = item.Cutwidth,
                            Efficiency = item.Efficiency,

                            Sizeratio = item.Sizeratio,

                            Markerqty = item.Markerqty,
                            Conspcs = item.Conspcs,
                            Consdzn = item.Consdzn,

                            Wastage = item.Wastage,

                            Comments = item.Comments
                        };


                        // =================================================
                        // ADD DETAIL
                        // =================================================
                        _context.TblCadConsDs.Add(detail);

                        // =================================================
                        // IMPORTANT:
                        // Save first to generate CADDID
                        // =================================================
                        await _context.SaveChangesAsync();


                        // =================================================
                        // 4. SAVE MULTIPLE FILES
                        // =================================================
                        if (item.Files != null && item.Files.Count > 0)
                        {
                            foreach (var file in item.Files)
                            {
                                // Ignore empty file
                                if (file == null || file.Length == 0)
                                    continue;


                                // =================================================
                                // UNIQUE FILE NAME
                                // =================================================
                                var fileName =
                                    $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";


                                // =================================================
                                // PHYSICAL FILE PATH
                                // =================================================
                                var physicalFilePath =
                                    Path.Combine(folderPath, fileName);


                                // =================================================
                                // SAVE FILE TO WWWROOT
                                // =================================================
                                using (var stream = new FileStream(
                                    physicalFilePath,
                                    FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }


                                // =================================================
                                // SAVE FILE INFORMATION
                                // TBL_CAD_CONS_FILES
                                // =================================================
                                var fileRecord = new TblCadConsFiles
                                {
                                    // Link with TblCadConsD
                                    Caddid = detail.Caddid,

                                    Filename = fileName,

                                    Filepath = "/UploadFiles/" + fileName,

                                    Filesize = file.Length,

                                    Contenttype = file.ContentType,

                                    Createddate = DateTime.Now,

                                    Createdby = User.Identity?.Name ?? "System"
                                };


                                _context.TblCadConsFiles.Add(fileRecord);
                            }


                            // =================================================
                            // SAVE ALL FILE RECORDS
                            // =================================================
                            await _context.SaveChangesAsync();
                        }
                    }
                }


                // =====================================================
                // SUCCESS
                // =====================================================
                TempData["Success"] = "Data saved successfully!";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // =====================================================
                // ERROR
                // =====================================================
                TempData["Error"] =
                    "Something went wrong while saving data!";

                Console.WriteLine(ex);

                return View(model);
            }
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


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // =====================================================
            // 1. FETCH MASTER
            // =====================================================
            var master = await _context.TblCadConsMs
                .FirstOrDefaultAsync(m => m.Cadmid == id);

            if (master == null)
                return NotFound();


            // =====================================================
            // 2. FETCH DETAILS
            // =====================================================
            var details = await _context.TblCadConsDs
                .Where(d => d.Cadmid == id)
                .ToListAsync();


            // =====================================================
            // 3. CREATE VIEW MODEL
            // =====================================================
            var model = new CadConsumptionViewModel
            {
                Master = master,
                Details = new List<CadConsumptionDetailViewModel>()
            };


            // =====================================================
            // 4. LOAD DETAILS + EXISTING FILES
            // =====================================================
            foreach (var detail in details)
            {
                var existingFiles = await _context.TblCadConsFiles
                    .Where(f => f.Caddid == detail.Caddid)
                    .ToListAsync();

                var detailViewModel = new CadConsumptionDetailViewModel
                {
                    Caddid = detail.Caddid,
                    Cadmid = detail.Cadmid,
                    Transdate = detail.Transdate,

                    Ptnnmbr = detail.Ptnnmbr,
                    Gmntitem = detail.Gmntitem,
                    Gmntcolor = detail.Gmntcolor,
                    Fabricdes = detail.Fabricdes,
                    Fabricusage = detail.Fabricusage,

                    Gsm = detail.Gsm,
                    Opt01 = detail.Opt01,
                    Fullwidth = detail.Fullwidth,
                    Cutwidth = detail.Cutwidth,
                    Efficiency = detail.Efficiency,

                    Sizeratio = detail.Sizeratio,

                    Markerqty = detail.Markerqty,
                    Conspcs = detail.Conspcs,
                    Consdzn = detail.Consdzn,

                    Wastage = detail.Wastage,

                    Comments = detail.Comments,

                    // Existing uploaded files
                    ExistingFiles = existingFiles
                };

                model.Details.Add(detailViewModel);
            }


            // =====================================================
            // 5. BOOKING NOS
            // =====================================================
            var bookingNos = _oracleContext.NewView3
                .Where(b => !string.IsNullOrEmpty(b.BOOKING_NO))
                .Select(b => b.BOOKING_NO.Trim())
                .Distinct()
                .ToList();

            ViewBag.BookingNos = new SelectList(
                bookingNos,
                master.Styleref
            );


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
                // =====================================================
                // 1. UPDATE MASTER
                // =====================================================

                var existingMaster = await _context.TblCadConsMs
                    .FirstOrDefaultAsync(m => m.Cadmid == model.Master.Cadmid);

                if (existingMaster == null)
                    return NotFound();


                existingMaster.Caddate = model.Master.Caddate;
                existingMaster.Opt01 = model.Master.Opt01;
                existingMaster.Styleref = model.Master.Styleref;
                existingMaster.Job = model.Master.Job;
                existingMaster.Ir = model.Master.Ir;
                existingMaster.Buyer = model.Master.Buyer;
                existingMaster.Brand = model.Master.Brand;
                existingMaster.Season = model.Master.Season;
                existingMaster.Seasonyear = model.Master.Seasonyear;
                existingMaster.Styledes = model.Master.Styledes;
                existingMaster.Patternmaster = model.Master.Patternmaster;
                existingMaster.Consfor = model.Master.Consfor;
                existingMaster.Comments = model.Master.Comments;
                existingMaster.Isapproved = model.Master.Isapproved;

                existingMaster.UpdatedAt = DateTime.Now;
                existingMaster.UpdatedBy =
                    User.Identity?.Name ?? "System";


                await _context.SaveChangesAsync();


                // =====================================================
                // 2. UPLOAD FOLDER
                // =====================================================

                var folderPath = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "UploadFiles"
                );

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }


                // =====================================================
                // 3. UPDATE DETAILS
                // =====================================================

                if (model.Details != null && model.Details.Count > 0)
                {
                    foreach (var item in model.Details)
                    {
                        // =================================================
                        // FIND EXISTING DETAIL
                        // =================================================

                        TblCadConsD? existingDetail = null;

                        if (item.Caddid > 0)
                        {
                            existingDetail = await _context.TblCadConsDs
                                .FirstOrDefaultAsync(
                                    d => d.Caddid == item.Caddid
                                );
                        }


                        // =================================================
                        // EXISTING DETAIL
                        // =================================================

                        if (existingDetail != null)
                        {
                            // =============================================
                            // UPDATE DETAIL FIELDS
                            // =============================================

                            existingDetail.Cadmid =
                                model.Master.Cadmid;

                            existingDetail.Transdate =
                                DateTime.Now;

                            existingDetail.Ptnnmbr =
                                item.Ptnnmbr;

                            existingDetail.Gmntitem =
                                item.Gmntitem;

                            existingDetail.Gmntcolor =
                                item.Gmntcolor;

                            existingDetail.Fabricdes =
                                item.Fabricdes;

                            existingDetail.Fabricusage =
                                item.Fabricusage;

                            existingDetail.Gsm =
                                item.Gsm;

                            existingDetail.Opt01 =
                                item.Opt01;

                            existingDetail.Fullwidth =
                                item.Fullwidth;

                            existingDetail.Cutwidth =
                                item.Cutwidth;

                            existingDetail.Efficiency =
                                item.Efficiency;

                            existingDetail.Sizeratio =
                                item.Sizeratio;

                            existingDetail.Markerqty =
                                item.Markerqty;

                            existingDetail.Conspcs =
                                item.Conspcs;

                            existingDetail.Consdzn =
                                item.Consdzn;

                            existingDetail.Wastage =
                                item.Wastage;

                            existingDetail.Comments =
                                item.Comments;


                            // =============================================
                            // DELETE EXISTING FILES
                            // =============================================

                            if (item.DeletedFileIds != null &&
                                item.DeletedFileIds.Count > 0)
                            {
                                foreach (var fileId in item.DeletedFileIds.Distinct())
                                {
                                    var existingFile =
                                        await _context.TblCadConsFiles
                                            .FirstOrDefaultAsync(
                                                f =>
                                                    f.Fileid == fileId &&
                                                    f.Caddid == existingDetail.Caddid
                                            );

                                    if (existingFile != null)
                                    {
                                        // =================================
                                        // DELETE PHYSICAL FILE
                                        // =================================

                                        if (!string.IsNullOrWhiteSpace(
                                            existingFile.Filepath))
                                        {
                                            var relativePath =
                                                existingFile.Filepath
                                                    .TrimStart('/')
                                                    .Replace(
                                                        "/",
                                                        Path.DirectorySeparatorChar
                                                            .ToString()
                                                    );

                                            var physicalFilePath =
                                                Path.Combine(
                                                    _webHostEnvironment.WebRootPath,
                                                    relativePath
                                                );

                                            if (System.IO.File.Exists(
                                                physicalFilePath))
                                            {
                                                try
                                                {
                                                    System.IO.File.Delete(
                                                        physicalFilePath
                                                    );
                                                }
                                                catch (Exception fileEx)
                                                {
                                                    Console.WriteLine(
                                                        "Physical file delete failed: "
                                                        + fileEx.Message
                                                    );
                                                }
                                            }
                                        }


                                        // =================================
                                        // DELETE DATABASE RECORD
                                        // =================================

                                        _context.TblCadConsFiles
                                            .Remove(existingFile);
                                    }
                                }
                            }


                            // =============================================
                            // NEW FILES UPLOAD - OPTIONAL
                            // =============================================

                            if (item.Files != null &&
                                item.Files.Count > 0)
                            {
                                foreach (var file in item.Files)
                                {
                                    if (file == null ||
                                        file.Length == 0)
                                    {
                                        continue;
                                    }


                                    // =====================================
                                    // UNIQUE FILE NAME
                                    // =====================================

                                    var fileName =
                                        $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";


                                    var physicalFilePath =
                                        Path.Combine(
                                            folderPath,
                                            fileName
                                        );


                                    // =====================================
                                    // SAVE PHYSICAL FILE
                                    // =====================================

                                    using (var stream =
                                           new FileStream(
                                               physicalFilePath,
                                               FileMode.Create))
                                    {
                                        await file.CopyToAsync(stream);
                                    }


                                    // =====================================
                                    // SAVE DATABASE RECORD
                                    // =====================================

                                    var fileRecord =
                                        new TblCadConsFiles
                                        {
                                            Caddid =
                                                existingDetail.Caddid,

                                            Filename =
                                                fileName,

                                            Filepath =
                                                "/UploadFiles/" +
                                                fileName,

                                            Filesize =
                                                file.Length,

                                            Contenttype =
                                                file.ContentType,

                                            Createddate =
                                                DateTime.Now,

                                            Createdby =
                                                User.Identity?.Name ??
                                                "System"
                                        };


                                    _context.TblCadConsFiles
                                        .Add(fileRecord);
                                }
                            }
                        }


                        // =================================================
                        // NEW DETAIL
                        // =================================================

                        else
                        {
                            // =============================================
                            // CREATE DETAIL
                            // =============================================

                            var newDetail =
                                new TblCadConsD
                                {
                                    Cadmid =
                                        model.Master.Cadmid,

                                    Transdate =
                                        DateTime.Now,

                                    Ptnnmbr =
                                        item.Ptnnmbr,

                                    Gmntitem =
                                        item.Gmntitem,

                                    Gmntcolor =
                                        item.Gmntcolor,

                                    Fabricdes =
                                        item.Fabricdes,

                                    Fabricusage =
                                        item.Fabricusage,

                                    Gsm =
                                        item.Gsm,

                                    Opt01 =
                                        item.Opt01,

                                    Fullwidth =
                                        item.Fullwidth,

                                    Cutwidth =
                                        item.Cutwidth,

                                    Efficiency =
                                        item.Efficiency,

                                    Sizeratio =
                                        item.Sizeratio,

                                    Markerqty =
                                        item.Markerqty,

                                    Conspcs =
                                        item.Conspcs,

                                    Consdzn =
                                        item.Consdzn,

                                    Wastage =
                                        item.Wastage,

                                    Comments =
                                        item.Comments
                                };


                            _context.TblCadConsDs.Add(newDetail);


                            // =============================================
                            // SAVE DETAIL FIRST
                            // GET CADDID
                            // =============================================

                            await _context.SaveChangesAsync();


                            // =============================================
                            // NEW FILES FOR NEW DETAIL - OPTIONAL
                            // =============================================

                            if (item.Files != null &&
                                item.Files.Count > 0)
                            {
                                foreach (var file in item.Files)
                                {
                                    if (file == null ||
                                        file.Length == 0)
                                    {
                                        continue;
                                    }


                                    // =====================================
                                    // UNIQUE FILE NAME
                                    // =====================================

                                    var fileName =
                                        $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";


                                    var physicalFilePath =
                                        Path.Combine(
                                            folderPath,
                                            fileName
                                        );


                                    // =====================================
                                    // SAVE PHYSICAL FILE
                                    // =====================================

                                    using (var stream =
                                           new FileStream(
                                               physicalFilePath,
                                               FileMode.Create))
                                    {
                                        await file.CopyToAsync(stream);
                                    }


                                    // =====================================
                                    // SAVE DATABASE RECORD
                                    // =====================================

                                    var fileRecord =
                                        new TblCadConsFiles
                                        {
                                            Caddid =
                                                newDetail.Caddid,

                                            Filename =
                                                fileName,

                                            Filepath =
                                                "/UploadFiles/" +
                                                fileName,

                                            Filesize =
                                                file.Length,

                                            Contenttype =
                                                file.ContentType,

                                            Createddate =
                                                DateTime.Now,

                                            Createdby =
                                                User.Identity?.Name ??
                                                "System"
                                        };


                                    _context.TblCadConsFiles
                                        .Add(fileRecord);
                                }
                            }
                        }
                    }


                    // =====================================================
                    // SAVE ALL CHANGES
                    // =====================================================

                    await _context.SaveChangesAsync();
                }


                // =====================================================
                // SUCCESS
                // =====================================================

                TempData["success"] =
                    "Data updated successfully!";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] =
                    "Something went wrong!";

                Console.WriteLine(ex);

                return View(model);
            }
        }




        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            // =====================================================
            // 1. FETCH MASTER
            // =====================================================
            var master = await _context.TblCadConsMs
                .FirstOrDefaultAsync(x => x.Cadmid == id);

            if (master == null)
                return NotFound();


            // =====================================================
            // 2. FETCH DETAILS
            // =====================================================
            var details = await _context.TblCadConsDs
                .Where(x => x.Cadmid == id)
                .ToListAsync();


            // =====================================================
            // 3. CREATE VIEW MODEL
            // =====================================================
            var model = new CadConsumptionViewModel
            {
                Master = master,
                Details = new List<CadConsumptionDetailViewModel>()
            };


            // =====================================================
            // 4. LOAD DETAILS + FILES
            // =====================================================
            foreach (var detail in details)
            {
                // ---------------------------------------------
                // Get files for this detail
                // ---------------------------------------------
                var files = await _context.TblCadConsFiles
                    .Where(f => f.Caddid == detail.Caddid)
                    .OrderBy(f => f.Fileid)
                    .ToListAsync();


                // ---------------------------------------------
                // Create Detail ViewModel
                // ---------------------------------------------
                var detailViewModel = new CadConsumptionDetailViewModel
                {
                    Caddid = detail.Caddid,
                    Cadmid = detail.Cadmid,
                    Transdate = detail.Transdate,

                    Ptnnmbr = detail.Ptnnmbr,
                    Gmntitem = detail.Gmntitem,
                    Gmntcolor = detail.Gmntcolor,
                    Fabricdes = detail.Fabricdes,
                    Fabricusage = detail.Fabricusage,

                    Gsm = detail.Gsm,
                    Opt01 = detail.Opt01,
                    Fullwidth = detail.Fullwidth,
                    Cutwidth = detail.Cutwidth,
                    Efficiency = detail.Efficiency,

                    Sizeratio = detail.Sizeratio,

                    Markerqty = detail.Markerqty,
                    Conspcs = detail.Conspcs,
                    Consdzn = detail.Consdzn,

                    Wastage = detail.Wastage,

                    Comments = detail.Comments,

                    // -----------------------------------------
                    // Existing Files
                    // -----------------------------------------
                    ExistingFiles = files
                };


                model.Details.Add(detailViewModel);
            }


            // =====================================================
            // 5. RETURN VIEW
            // =====================================================
            return View(model);
        }



        [HttpGet]
        public IActionResult ViewFile(int id)
        {
            try
            {
                var item = _context.TblCadConsFiles.FirstOrDefault(x => x.Fileid == id);

                if (item == null || string.IsNullOrEmpty(item.Filepath))
                    return NotFound();

                var relativePath = item.Filepath.TrimStart('/', '\\');
                var path = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

                if (!System.IO.File.Exists(path))
                    return NotFound();

                var fileName = Path.GetFileName(path);
                var ext = Path.GetExtension(path).ToLower();

                string contentType = "application/octet-stream";

                if (ext == ".pdf") contentType = "application/pdf";
                if (ext == ".jpg" || ext == ".jpeg") contentType = "image/jpeg";
                if (ext == ".png") contentType = "image/png";

                Response.Headers["Content-Disposition"] = $"inline; filename=\"{fileName}\"";

                return PhysicalFile(path, contentType);
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.Message);
            }
        }



        //public IActionResult ViewFile(int id)
        //{
        //    var item = _context.TblCadConsDs.FirstOrDefault(x => x.Caddid == id);

        //    if (item == null || string.IsNullOrEmpty(item.Filepath))
        //        return NotFound();

        //    var relativePath = item.Filepath.TrimStart('/', '\\');
        //    var path = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

        //    if (!System.IO.File.Exists(path))
        //        return NotFound();

        //    // ✅ Get original file name
        //    var fileName = Path.GetFileName(path);

        //    // (Optional: remove prefix like "123_abc.pdf" → "abc.pdf")
        //    if (fileName.Contains("_"))
        //    {
        //        fileName = fileName.Substring(fileName.IndexOf("_") + 1);
        //    }

        //    // Detect content type
        //    var ext = Path.GetExtension(path).ToLower();
        //    string contentType = "application/octet-stream";

        //    switch (ext)
        //    {
        //        case ".pdf":
        //            contentType = "application/pdf";
        //            break;
        //        case ".jpg":
        //        case ".jpeg":
        //            contentType = "image/jpeg";
        //            break;
        //        case ".png":
        //            contentType = "image/png";
        //            break;
        //    }

        //    // ✅ KEY LINE: set download name (even for preview)
        //    Response.Headers["Content-Disposition"] = $"inline; filename=\"{fileName}\"";

        //    return PhysicalFile(path, contentType);
        //}

        [HttpGet]
        public IActionResult DownloadFile(int id)
        {
            // Find the detail record
            var fileRecord = _context.TblCadConsFiles.FirstOrDefault(d => d.Caddid == id);

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

        // =========================================================
        // GET: Delete
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // =====================================================
            // 1. FETCH MASTER
            // =====================================================
            var master = await _context.TblCadConsMs
                .FirstOrDefaultAsync(m => m.Cadmid == id);

            if (master == null)
            {
                TempData["Error"] = "Record not found!";
                return RedirectToAction("Index");
            }


            // =====================================================
            // 2. FETCH DETAILS
            // =====================================================
            var details = await _context.TblCadConsDs
                .Where(d => d.Cadmid == id)
                .ToListAsync();


            // =====================================================
            // 3. GET DETAIL IDS
            // =====================================================
            var detailIds = details
                .Select(d => d.Caddid)
                .ToList();


            // =====================================================
            // 4. FETCH FILES
            // =====================================================
            var files = new List<TblCadConsFiles>();

            if (detailIds.Count > 0)
            {
                files = await _context.TblCadConsFiles
                    .Where(f => detailIds.Contains(f.Caddid))
                    .ToListAsync();
            }


            // =====================================================
            // 5. CREATE VIEW MODEL
            // =====================================================
            var model = new CadConsumptionViewModel
            {
                Master = master,
                Details = new List<CadConsumptionDetailViewModel>()
            };


            // =====================================================
            // 6. MAP DETAILS + EXISTING FILES
            // =====================================================
            foreach (var detail in details)
            {
                var detailFiles = files
                    .Where(f => f.Caddid == detail.Caddid)
                    .ToList();


                var detailViewModel = new CadConsumptionDetailViewModel
                {
                    Caddid = detail.Caddid,
                    Cadmid = detail.Cadmid,
                    Transdate = detail.Transdate,

                    Ptnnmbr = detail.Ptnnmbr,
                    Gmntitem = detail.Gmntitem,
                    Gmntcolor = detail.Gmntcolor,
                    Fabricdes = detail.Fabricdes,
                    Fabricusage = detail.Fabricusage,

                    Gsm = detail.Gsm,
                    Opt01 = detail.Opt01,
                    Fullwidth = detail.Fullwidth,
                    Cutwidth = detail.Cutwidth,
                    Efficiency = detail.Efficiency,

                    Sizeratio = detail.Sizeratio,

                    Markerqty = detail.Markerqty,
                    Conspcs = detail.Conspcs,
                    Consdzn = detail.Consdzn,

                    Wastage = detail.Wastage,

                    Comments = detail.Comments,

                    ExistingFiles = detailFiles
                };


                model.Details.Add(detailViewModel);
            }


            return View(model);
        }


        // =========================================================
        // POST: Delete
        // =========================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // =====================================================
                // 1. FIND MASTER
                // =====================================================
                var master = await _context.TblCadConsMs
                    .FirstOrDefaultAsync(m => m.Cadmid == id);

                if (master == null)
                {
                    TempData["Error"] = "Record not found!";
                    return RedirectToAction("Index");
                }


                // =====================================================
                // 2. FIND DETAILS
                // =====================================================
                var details = await _context.TblCadConsDs
                    .Where(d => d.Cadmid == id)
                    .ToListAsync();


                // =====================================================
                // 3. GET DETAIL IDS
                // =====================================================
                var detailIds = details
                    .Select(d => d.Caddid)
                    .ToList();


                // =====================================================
                // 4. FIND ALL FILE RECORDS
                // =====================================================
                var files = new List<TblCadConsFiles>();

                if (detailIds.Count > 0)
                {
                    files = await _context.TblCadConsFiles
                        .Where(f => detailIds.Contains(f.Caddid))
                        .ToListAsync();
                }


                // =====================================================
                // 5. WWWROOT UPLOAD FOLDER
                // =====================================================
                var uploadFolder = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "UploadFiles"
                );


                // =====================================================
                // 6. DELETE PHYSICAL FILES
                // =====================================================
                foreach (var file in files)
                {
                    if (string.IsNullOrWhiteSpace(file.Filepath))
                        continue;


                    // Get only filename
                    var fileName = Path.GetFileName(file.Filepath);

                    if (string.IsNullOrWhiteSpace(fileName))
                        continue;


                    // Physical path
                    var physicalPath = Path.Combine(
                        uploadFolder,
                        fileName
                    );


                    // Delete physical file
                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                    }
                }


                // =====================================================
                // 7. DELETE FILE DATABASE RECORDS
                // =====================================================
                if (files.Count > 0)
                {
                    _context.TblCadConsFiles.RemoveRange(files);
                }


                // =====================================================
                // 8. DELETE DETAIL RECORDS
                // =====================================================
                if (details.Count > 0)
                {
                    _context.TblCadConsDs.RemoveRange(details);
                }


                // =====================================================
                // 9. DELETE MASTER
                // =====================================================
                _context.TblCadConsMs.Remove(master);


                // =====================================================
                // 10. SAVE CHANGES
                // =====================================================
                await _context.SaveChangesAsync();


                // =====================================================
                // SUCCESS
                // =====================================================
                TempData["Success"] =
                    "Data and all associated files deleted successfully!";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "Error occurred while deleting data!";

                Console.WriteLine(ex);

                return RedirectToAction("Index");
            }
        }

    }
}
