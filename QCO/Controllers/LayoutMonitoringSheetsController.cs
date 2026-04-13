
using Microsoft.AspNetCore.Mvc;
using QCO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QCO.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Experimental.FileAccess;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using X.PagedList;
using X.PagedList.Mvc.Core;
using X.PagedList.Extensions;

namespace QCO.Controllers
{
    public class LayoutMonitoringSheetsController : Controller
    {
        private readonly QCOContext _context;
        private readonly OracleContext _oracleContext;
        private readonly ILogger<LayoutMonitoringSheetsController> _logger;

        public LayoutMonitoringSheetsController(QCOContext context, OracleContext oracleContext, ILogger<LayoutMonitoringSheetsController> logger)
        {
            _context = context;
            _oracleContext = oracleContext;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string search, int? page)
        {
            int pageSize = 16; // Number of records per page
            int pageNumber = page ?? 1; // Default to page 1

            var query = _context.TblLayoutMonitoringSheets.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Company.Contains(search) ||
                                         x.BookingNo.Contains(search) ||
                                         x.BuyerName.Contains(search) ||
                                         x.LineNo.Contains(search));
            }

            // Ensure the returned data is of type IPagedList
            IPagedList<TblLayoutMonitoringSheet> data = query
                .OrderByDescending(x => x.MonitoringDate)
                .ToPagedList(pageNumber, pageSize);

            ViewBag.Search = search; // Store search term in ViewBag

            return View(data); // Ensure View receives an IPagedList<T>
        }

        // GET: LayoutMonitoringSheets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var data = await (from a in _context.TblLayoutMonitoringSheets
                              where a.Slno == id
                              select new LayoutMonitoringSheetViewModel
                              {
                                  Slno = a.Slno,
                                  Company = a.Company,
                                  MonitoringDate = a.MonitoringDate,
                                  BookingNo = a.BookingNo,
                                  BuyerName = a.BuyerName,
                                  LineNo = a.LineNo,
                                  FeedStart = a.FeedStart,
                                  FeedFinish = a.FeedFinish,
                                  PreStyleFinish = a.PreStyleFinish,
                                  Status = a.Status,

                                  DetailRecords = (from b in _context.TblLayoutMonitoringSheetDs
                                                   where b.Slno == a.Slno
                                                   select new DetailViewModel
                                                   {
                                                       Trnsid = b.Trnsid,
                                                       SequenceNo = b.SequenceNo,
                                                       ProcessName = b.ProcessName,
                                                       ResourceName = b.ResourceName,
                                                       McSetupStart = b.McSetupStart,
                                                       McSetupFinish = b.McSetupFinish,
                                                       McSetupDuration = b.McSetupDuration,
                                                       Remarks = b.Remarks,

                                                       // Fetch the cause IDs and store in SelectedCauses (as you did before)
                                                       SelectedCauses = _context.tbl_Cause_Details
                                                           .Where(c => c.TRNSID == b.Trnsid)
                                                           .Select(c => c.CAUSEID ?? 0)
                                                           .ToList(),

                                                       CauseTimes = _context.tbl_Cause_Details
                                                       .Where(c => c.TRNSID == b.Trnsid)
                                                       .Select(c => c.TIME ?? 0)  // Assuming "CauseTime" is the column storing time
                                                       .ToList(),

                                                       // Fetch the Cause Names and store in Causes (as you did before)
                                                       Causes = _context.tbl_Cause_Details
                                                           .Where(c => c.TRNSID == b.Trnsid)
                                                           .Join(_context.tbl_Cause_Of_Delays,
                                                                 cd => cd.CAUSEID,
                                                                 c => c.CAUSEID,
                                                                 (cd, c) => c) // This will return the full `tbl_Cause_Of_Delay` object
                                                           .ToList() // Ensure this is a List<tbl_Cause_Of_Delay>
                                                   }).ToList()
                              }).FirstOrDefaultAsync();

            if (data == null)
            {
                return NotFound();
            }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TblLayoutMonitoringSheet tblLayoutMonitoringSheet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblLayoutMonitoringSheet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(tblLayoutMonitoringSheet);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Log the received Slno (id)
            _logger.LogInformation($"Fetching record with Slno = {id}");

            // Fetch the master record along with its related detail records
            var masterRecord = _context.TblLayoutMonitoringSheets
                .Include(m => m.DetailRecords) // Include the related detail records
                .FirstOrDefault(m => m.Slno == id); // Look for record by Slno (primary key)

            if (masterRecord == null)
            {
                _logger.LogWarning($"No record found for Slno = {id}");
                return NotFound(); // Return 404 if record not found
            }

            // Fetch causes from the database
            var causes = _context.tbl_Cause_Of_Delays.ToList();

            // Create a view model to pass to the view
            var viewModel = new LayoutMonitoringSheetViewModel
            {
                Slno = masterRecord.Slno,
                Company = masterRecord.Company,
                MonitoringDate = masterRecord.MonitoringDate,
                BookingNo = masterRecord.BookingNo,
                BuyerName = masterRecord.BuyerName,
                LineNo = masterRecord.LineNo,
                Total_SMV = masterRecord.Total_SMV,
                FeedStart = masterRecord.FeedStart,
                FeedFinish = masterRecord.FeedFinish,
                PreStyleFinish = masterRecord.PreStyleFinish,
                NewStyleFinish = masterRecord.NewStyleFinish,
                Status = masterRecord.Status,
                // Map the related detail records to the view model
                DetailRecords = masterRecord.DetailRecords.Select(d => new DetailViewModel
                {
                    Trnsid = d.Trnsid,
                    SequenceNo = d.SequenceNo,
                    ProcessName = d.ProcessName,
                    ResourceName = d.ResourceName,
                    McSetupStart = d.McSetupStart,
                    McSetupFinish = d.McSetupFinish,
                    McSetupDuration = d.McSetupDuration,
                    Remarks = d.Remarks,
                    Causes = causes, // Pass the full list of causes
                    SelectedCauses = new List<int>() // Initialize an empty list for selected causes
                }).ToList()
            };

            return View(viewModel); // Return the view with the view model
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, LayoutMonitoringSheetViewModel model)
        {
            if (id != model.Slno)
            {
                return NotFound();
            }

            // Debugging: Log the values of SelectedCauses and CauseTimes
            foreach (var detail in model.DetailRecords)
            {
                Console.WriteLine($"DetailRecord {detail.Trnsid}:");
                Console.WriteLine($"SelectedCauses: {string.Join(",", detail.SelectedCauses)}");
                Console.WriteLine($"CauseTimes: {string.Join(",", detail.CauseTimes)}");
            }

            // Ensure DetailRecords is initialized
            if (model.DetailRecords == null)
            {
                model.DetailRecords = new List<DetailViewModel>();
            }
            if (string.IsNullOrWhiteSpace(model.Status))
            {
                ModelState.AddModelError("Status", "Status field is required.");
            }
            // Initialize SelectedCauses and CauseTimes if null
            foreach (var detail in model.DetailRecords)
            {
                detail.SelectedCauses ??= new List<int>();
                detail.CauseTimes ??= new List<int>();
            }

            // If model is valid, proceed to save changes
            if (ModelState.IsValid)
            {
                var masterRecord = _context.TblLayoutMonitoringSheets
                    .Include(m => m.DetailRecords)
                    .FirstOrDefault(m => m.Slno == model.Slno);

                if (masterRecord == null)
                {
                    return NotFound();
                }

                // Update master record
                masterRecord.Company = model.Company;
                masterRecord.MonitoringDate = model.MonitoringDate;
                masterRecord.BookingNo = model.BookingNo;
                masterRecord.BuyerName = model.BuyerName;
                masterRecord.LineNo = model.LineNo;
                masterRecord.Total_SMV = model.Total_SMV;
                masterRecord.FeedStart = model.FeedStart;
                masterRecord.FeedFinish = model.FeedFinish;
                masterRecord.PreStyleFinish = model.PreStyleFinish;
                masterRecord.NewStyleFinish = model.NewStyleFinish;
                masterRecord.Status = model.Status;
                masterRecord.Usrid = User.Identity.Name; // Updating UserID

                // Update detail records
                foreach (var detail in model.DetailRecords)
                {
                    var existingDetail = masterRecord.DetailRecords
                        .FirstOrDefault(d => d.Trnsid == detail.Trnsid);

                    if (existingDetail == null)
                    {
                        // Create new detail record
                        var newDetail = new TblLayoutMonitoringSheetD
                        {
                            Trnsid = detail.Trnsid,
                            SequenceNo = detail.SequenceNo,
                            ProcessName = detail.ProcessName,
                            ResourceName = detail.ResourceName,
                            McSetupStart = detail.McSetupStart,
                            McSetupFinish = detail.McSetupFinish,
                            McSetupDuration = detail.McSetupDuration,
                            Remarks = detail.Remarks,
                        };

                        masterRecord.DetailRecords.Add(newDetail);
                    }
                    else
                    {
                        // Update existing detail record
                        existingDetail.SequenceNo = detail.SequenceNo;
                        existingDetail.ProcessName = detail.ProcessName;
                        existingDetail.ResourceName = detail.ResourceName;
                        existingDetail.McSetupStart = detail.McSetupStart;
                        existingDetail.McSetupFinish = detail.McSetupFinish;
                        existingDetail.McSetupDuration = detail.McSetupDuration;
                        existingDetail.Remarks = detail.Remarks;
                    }

                    // Handle causes update if selected causes and times are available
                    if (detail.SelectedCauses.Any() && detail.CauseTimes.Any())
                    {
                        var existingCauses = _context.tbl_Cause_Details
                            .Where(cd => cd.TRNSID == detail.Trnsid)
                            .ToList();

                        var existingCauseIds = existingCauses.Select(c => c.CAUSEID).ToList();

                        foreach (var cause in detail.SelectedCauses.Select((causeId, index) => new { causeId, time = detail.CauseTimes[index] }))
                        {
                            var existingCause = existingCauses.FirstOrDefault(c => c.CAUSEID == cause.causeId);

                            if (existingCause == null)
                            {
                                _context.tbl_Cause_Details.Add(new tbl_Cause_Detail
                                {
                                    TRNSID = detail.Trnsid,
                                    CAUSEID = cause.causeId,
                                    TIME = cause.time
                                });
                            }
                            else
                            {
                                existingCause.TIME = cause.time; // Update time if it already exists
                            }
                        }

                        // Remove only causes that are not present in the newly selected causes
                        var causesToRemove = existingCauses
                            .Where(c => !detail.SelectedCauses.Contains(c.CAUSEID.Value))
                            .ToList();

                        if (causesToRemove.Any())
                        {
                            _context.tbl_Cause_Details.RemoveRange(causesToRemove);
                        }
                    }

                }

                try
                {
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "Concurrency error occurred while saving data.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving data: " + ex.Message);
                }
            }

            return View(model);
        }


        // GET: LayoutMonitoringSheets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblLayoutMonitoringSheet = await _context.TblLayoutMonitoringSheets
                .FirstOrDefaultAsync(m => m.Slno == id);
            if (tblLayoutMonitoringSheet == null)
            {
                return NotFound();
            }

            return View(tblLayoutMonitoringSheet);
        }

        // POST: LayoutMonitoringSheets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblLayoutMonitoringSheet = await _context.TblLayoutMonitoringSheets.FindAsync(id);
            if (tblLayoutMonitoringSheet != null)
            {
                _context.TblLayoutMonitoringSheets.Remove(tblLayoutMonitoringSheet);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblLayoutMonitoringSheetExists(int id)
        {
            return _context.TblLayoutMonitoringSheets.Any(e => e.Slno == id);
        }


        [HttpPost]
        public IActionResult SaveLayoutMonitoringSheet(LayoutMonitoringSheetViewModel model)
        {
            if (model.DetailRecords == null || !model.DetailRecords.Any())
            {
                ModelState.AddModelError(string.Empty, "Detail records are required.");
            }

            foreach (var detail in model.DetailRecords)
            {
                if (detail.McSetupStart.HasValue && detail.McSetupFinish.HasValue)
                {
                    var startTime = detail.McSetupStart.Value.ToTimeSpan();
                    var finishTime = detail.McSetupFinish.Value.ToTimeSpan();

                    if (finishTime < startTime)
                    {
                        finishTime = finishTime.Add(new TimeSpan(24, 0, 0));
                    }

                    detail.McSetupDuration = (decimal)(finishTime - startTime).TotalMinutes;
                }

                if (!detail.McSetupDuration.HasValue || detail.McSetupDuration <= 0)
                {
                    detail.McSetupDuration = 0;
                }
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data!" });
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Get the currently logged-in user's ID
                    string userId = User.Identity?.Name ?? "UnknownUser";

                    // Save Master Record
                    var masterRecord = new TblLayoutMonitoringSheet
                    {
                        Company = model.Company,
                        MonitoringDate = model.MonitoringDate,
                        BookingNo = model.BookingNo,
                        BuyerName = model.BuyerName,
                        LineNo = model.LineNo,
                        Total_SMV = model.Total_SMV,
                        FeedStart = model.FeedStart,
                        FeedFinish = model.FeedFinish,
                        PreStyleFinish = model.PreStyleFinish,
                        NewStyleFinish = model.NewStyleFinish,
                        Status = model.Status,
                        Usrid = userId // Save the user ID
                    };

                    _context.TblLayoutMonitoringSheets.Add(masterRecord);
                    _context.SaveChanges();

                    // Save Detail Records
                    foreach (var detail in model.DetailRecords)
                    {
                        var detailRecord = new TblLayoutMonitoringSheetD
                        {
                            Slno = masterRecord.Slno,
                            SequenceNo = detail.SequenceNo,
                            ProcessName = detail.ProcessName,
                            ResourceName = detail.ResourceName,
                            McSetupStart = detail.McSetupStart,
                            McSetupFinish = detail.McSetupFinish,
                            McSetupDuration = detail.McSetupDuration ?? 0,
                            Remarks = detail.Remarks,
                        };

                        _context.TblLayoutMonitoringSheetDs.Add(detailRecord);
                    }

                    _context.SaveChanges();
                    transaction.Commit();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(ex, "Error occurred while saving layout monitoring sheet.");
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

        [HttpGet]
        public IActionResult GetBookingNos(string term)
        {
            var searchTerm = term?.Trim().ToUpper();

            if (string.IsNullOrEmpty(searchTerm))
            {
                // Return an empty list for an empty search term
                return Json(new { results = new List<object>() });
            }

            // Fetch distinct booking data
            var bookingData = _oracleContext.NewView3
                .Where(x => !string.IsNullOrEmpty(x.BOOKING_NO)
                            && EF.Functions.Like(x.BOOKING_NO.ToUpper(), $"%{searchTerm}%"))
                .Select(x => new
                {
                    x.BOOKING_NO,
                    x.BUYER_NAME,
                    x.TOTAL_SMV
                })
                .Distinct()
                .ToList(); // Fetch booking data first

            // Fetch all processes for the filtered booking numbers
            //    var bookingNos = bookingData.Select(x => x.BOOKING_NO).ToList();
            //    var processesData = _oracleContext.NewView3
            //        .Where(x => bookingNos.Contains(x.BOOKING_NO) && !string.IsNullOrEmpty(x.OPERATION_NAME))
            //        .ToList() // Fetch all necessary process data upfront
            //        .GroupBy(x => x.BOOKING_NO)
            //        .ToDictionary(g => g.Key, g => g.Select(p => new
            //        {
            //            processName = p.OPERATION_NAME.Trim(),
            //            resourceName = p.RESOURCE_NAME?.Trim() ?? "",
            //            sequenceNo = p.ROW_SEQUENCE_NO
            //}).ToList());
            var bookingNos = bookingData.Select(x => x.BOOKING_NO).ToList();

            var processesData = _oracleContext.NewView3
                .Where(x => bookingNos.Contains(x.BOOKING_NO) && !string.IsNullOrEmpty(x.OPERATION_NAME))
                .ToList()
                .GroupBy(x => x.BOOKING_NO)
                .ToDictionary(
                    g => g.Key,
                    g => new[] {
            new {
                processName = "Startup Delay",   // forced process
                resourceName = "",
                sequenceNo = 0
            }
                    }
                    .Concat(
                        g.OrderBy(x => x.ROW_SEQUENCE_NO) // preserve sequence from DB
                         .Select(p => new {
                             processName = p.OPERATION_NAME.Trim(),
                             resourceName = p.RESOURCE_NAME?.Trim() ?? "",
                             sequenceNo = p.ROW_SEQUENCE_NO
                         })
                    )
                    .ToList()
                );

            // Combine booking data with their processes
            var combinedData = bookingData.Select(x => new
            {
                id = x.BOOKING_NO.Trim(),
                text = x.BOOKING_NO.Trim(),
                buyerName = x.BUYER_NAME?.Trim() ?? "",
                totalSmv = x.TOTAL_SMV ?? 0,
                processes = processesData.ContainsKey(x.BOOKING_NO)
         ? processesData[x.BOOKING_NO] // List of the anonymous type
         : new List<object>() // Fallback: must use the same type
             .Select(_ => new { processName = "", resourceName = "", sequenceNo = 0 })
             .ToList() // Ensures consistent anonymous type
            }).ToList();

            // Return the results in the format expected by Select2
            return Json(new { results = combinedData });

        }
        public async Task<IActionResult> RawSearch(string term)
        {
            var query = _context.TblLayoutMonitoringSheets.AsQueryable();

            if (!string.IsNullOrEmpty(term))
            {
                query = query.Where(x => x.Company.Contains(term) ||
                                         x.BookingNo.Contains(term) ||
                                         x.BuyerName.Contains(term) ||
                                         x.LineNo.Contains(term));
            }

            var results = await query.OrderByDescending(x => x.MonitoringDate).Take(50).ToListAsync();

            return PartialView("_MonitoringTableBody", results);
        }



    }

}
