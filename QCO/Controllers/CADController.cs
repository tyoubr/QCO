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

            var query = _context.TblCadConsMs
                .GroupJoin(
                    _context.TblCadConsDs,
                    m => m.Cadmid,
                    d => d.Cadmid,
                    (m, details) => new CadConsumptionViewModel
                    {
                        Master = m,
                        Details = details.ToList()
                    }
                );

            // 🔍 Search
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x =>
                    (x.Master.Opt01 != null && x.Master.Opt01.Contains(search)) ||
                    (x.Master.Styleref != null && x.Master.Styleref.Contains(search)) ||
                    (x.Master.Buyer != null && x.Master.Buyer.Contains(search)) ||
                    x.Details.Any(d =>
                        (d.Ptnnmbr != null && d.Ptnnmbr.Contains(search)) ||
                        (d.Gmntitem != null && d.Gmntitem.Contains(search)) ||
                        (d.Fabricdes != null && d.Fabricdes.Contains(search))
                    )
                );
            }

            // 📅 Order
            query = query.OrderByDescending(x => x.Master.Caddate);

            // ❗ FIX HERE (no async)
            var data = query.ToPagedList(pageNumber, pageSize);

            ViewBag.Search = search;

            return View(data);
        }
        // GET: LayoutMonitoringSheets/Create
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
        [HttpPost]
        public IActionResult Create(CadConsumptionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // ✅ ADD THIS BLOCK HERE
                foreach (var error in ModelState)
                {
                    foreach (var subError in error.Value.Errors)
                    {
                        Console.WriteLine($"{error.Key} : {subError.ErrorMessage}");
                    }
                }

                return View(model);
            }

            // Save Master
            _context.TblCadConsMs.Add(model.Master);
            _context.SaveChanges();

            // Save Details
            if (model.Details != null && model.Details.Count > 0)
            {
                foreach (var item in model.Details)
                {
                    item.Cadmid = model.Master.Cadmid;
                    item.Transdate = DateTime.Now;

                    _context.TblCadConsDs.Add(item);
                }

                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
