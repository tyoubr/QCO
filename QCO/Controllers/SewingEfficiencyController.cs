using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QCO.Controllers;
using QCO.Models;
using X.PagedList;
using X.PagedList.Extensions;
using X.PagedList.Mvc.Core;

public class SewingEfficiencyController : Controller
{

    private readonly QCOContext _context;
    private readonly OracleContext _oracleContext;
    private readonly ILogger<LayoutMonitoringSheetsController> _logger;

    public SewingEfficiencyController(QCOContext context, OracleContext oracleContext, ILogger<LayoutMonitoringSheetsController> logger)
    {
        _context = context;
        _oracleContext = oracleContext;
        _logger = logger;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> SewingEfficiencyList(string search, int? page)
    {
        int pageSize = 16;
        int pageNumber = page ?? 1;

        var query = _context.TblSewingEfficiency.AsQueryable();


        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(x =>
                x.Company.Contains(search) ||
                x.BookingNo.Contains(search) ||
                x.LineNo.Contains(search)
            );
        }


        IPagedList<TblSewingEfficiency> data = query
            .OrderByDescending(x => x.Trid)
            .ToPagedList(pageNumber, pageSize);


        ViewBag.Search = search;


        return View(data);
    }
    [HttpGet]
    public IActionResult Create()
    {
        LoadFloorList();

        var vm = new SewingEfficiencyViewModel
        {
            Details = new List<SewingEfficiencyDetailModel>()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SewingEfficiencyViewModel model)
    {
        if (model.Details == null || model.Details.Count == 0)
        {
            ModelState.AddModelError("", "No data found.");
            LoadFloorList();
            return View(model);
        }


        foreach (var item in model.Details)
        {
            var detail = new TblSewingEfficiency
            {
                Trdate = DateTime.Now,
                Company = model.Header.Company,
                Floor = model.Header.Floor,
                LineNo = model.Header.LineNo,
                SwDate = model.Header.SwDate,

                BookingNo = item.BookingNo,
                Style = item.Style,
                Item = item.Item,

                Operator = item.Operator,
                Helper = item.Helper,
                ManPower = item.ManPower,
                GenWorkingHr = item.GenWorkingHr,

                SixPm = item.SixPm,
                SevenPm = item.SevenPm,
                EightPm = item.EightPm,
                NinePm = item.NinePm,
                TenPm = item.TenPm,
                ElevenPm = item.ElevenPm,
                TwelveAm = item.TwelveAm,

                Smv = item.Smv,
                EfficiencyPercent = item.EfficiencyPercent,
                LineTarget = item.LineTarget
            };


            _context.TblSewingEfficiency.Add(detail);
        }


        await _context.SaveChangesAsync();


        TempData["Success"] = "Saved successfully.";

        return RedirectToAction("SewingEfficiencyList");
    }
    [HttpPost]
    public IActionResult Show(SewingEfficiencyViewModel model)
    {
        LoadFloorList();

        if (string.IsNullOrWhiteSpace(model.Header.Company) ||
            model.Header.Floor == null ||
            model.Header.SwDate == null)
        {
            ModelState.AddModelError("", "Company, Floor and Date are required.");
            return View("Create", model);
        }

        int companyId = model.Header.Company switch
        {
            "Tropical Knitex Ltd." => 3,
            "Cotton Club BD Ltd." => 2,
            "Cotton Club BD ltd [Extended Part]" => 1,
            "Cotton Clothing BD Ltd." => 4,
            _ => 0
        };

        if (companyId == 0)
        {
            ModelState.AddModelError("", "Invalid Company.");
            return View("Create", model);
        }
        //string LineNo = model.Header.LineNo;

        model.Details = _context.SewingEfficiencyDetail
            .FromSqlInterpolated($@"
            EXEC SEWING_EFFICIENCY
                @CompanyId = {companyId},
                @PrDate    = {model.Header.SwDate.Value.Date},
                @FloorId   = {model.Header.Floor.Value},
                @LineNo    = {model.Header.LineNo}
        ")
            .AsNoTracking()
            .ToList();

        return View("Create", model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var data = await _context.TblSewingEfficiency
            .Where(x => x.Trid == id)
            .ToListAsync();


        if (data == null || data.Count == 0)
            return NotFound();


        var vm = new SewingEfficiencyViewModel();


        vm.Header = new TblSewingEfficiency
        {
            Trid = id,
            Company = data.First().Company,
            Floor = data.First().Floor,
            LineNo = data.First().LineNo,
            SwDate = data.First().SwDate
        };


        vm.EditDetails = data.Select(x => new SewingEfficiencyEditDetailModel
        {
            Trid = x.Trid,

            LineNo = x.LineNo,
            BookingNo = x.BookingNo,
            Style = x.Style,
            Item = x.Item,

            Operator = x.Operator,
            Helper = x.Helper,
            ManPower = x.ManPower,
            GenWorkingHr = x.GenWorkingHr,

            SixPm = x.SixPm,
            SevenPm = x.SevenPm,
            EightPm = x.EightPm,
            NinePm = x.NinePm,
            TenPm = x.TenPm,
            ElevenPm = x.ElevenPm,
            TwelveAm = x.TwelveAm,

            Smv = x.Smv,
            EfficiencyPercent =x.EfficiencyPercent,
            LineTarget = x.LineTarget

        }).ToList();


        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SewingEfficiencyViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }


        foreach (var item in model.EditDetails)
        {
            var data = await _context.TblSewingEfficiency
                .FirstOrDefaultAsync(x => x.Trid == item.Trid);


            if (data != null)
            {
                // Update editable fields

                data.Operator = item.Operator;
                data.Helper = item.Helper;
                data.ManPower = item.ManPower;
                data.GenWorkingHr = item.GenWorkingHr;


                data.SixPm = item.SixPm;
                data.SevenPm = item.SevenPm;
                data.EightPm = item.EightPm;
                data.NinePm = item.NinePm;
                data.TenPm = item.TenPm;
                data.ElevenPm = item.ElevenPm;
                data.TwelveAm = item.TwelveAm;


                // যদি user edit করতে পারে
                // data.StyleTarget = item.StyleTarget;
                // data.InderectMan = item.InderectMan;
                // data.AfterWashProduction = item.AfterWashProduction;


                // Calculation fields যদি DB তে update করতে চান

                data.EfficiencyPercent =
                    (decimal?)item.EfficiencyPercent;

                data.LineTarget =
                    (decimal?)item.LineTarget;


                _context.TblSewingEfficiency.Update(data);
            }
        }


        await _context.SaveChangesAsync();


        TempData["Success"] = "Updated Successfully";


        return RedirectToAction(nameof(SewingEfficiencyList));
    }
    [HttpGet]
    private void LoadFloorList()
    
    {
        var floors = _context.FloorDropdown
                             .FromSqlRaw("EXEC FLOOR")
                             .AsNoTracking()
                             .ToList();

        ViewBag.FloorList = new SelectList(floors, "ID", "FLOOR_NAME");
    }
    [HttpGet]
    public async Task<IActionResult> GetIndirectManpower(int trid)
    {
        var data = await _context.TblInderectManpower
            .FirstOrDefaultAsync(x => x.Trid == trid);


        if (data == null)
        {
            return Json(null);
        }


        return Json(data);
    }

    [HttpPost]
    public async Task<IActionResult> SaveIndirectManpower(TblInderectManpower inderectManpower)
    {
        var existing = await _context.TblInderectManpower
            .FirstOrDefaultAsync(x => x.Trid == inderectManpower.Trid);


        if (existing == null)
        {
            // INSERT

            inderectManpower.Total =
                (inderectManpower.Pregnent ?? 0)
                + (inderectManpower.SizeSetSample ?? 0)
                + (inderectManpower.AutoElasticMake ?? 0)
                + (inderectManpower.MedicalLeave ?? 0)
                + (inderectManpower.TraineeSupervisor ?? 0)
                + (inderectManpower.Repoter ?? 0)
                + (inderectManpower.Others ?? 0);


            _context.TblInderectManpower.Add(inderectManpower);

        }
        else
        {
            // UPDATE

            existing.Pregnent = inderectManpower.Pregnent;
            existing.SizeSetSample = inderectManpower.SizeSetSample;
            existing.AutoElasticMake = inderectManpower.AutoElasticMake;
            existing.MedicalLeave = inderectManpower.MedicalLeave;
            existing.TraineeSupervisor = inderectManpower.TraineeSupervisor;
            existing.Repoter = inderectManpower.Repoter;
            existing.Others = inderectManpower.Others;


            existing.Total =
                (inderectManpower.Pregnent ?? 0)
                + (inderectManpower.SizeSetSample ?? 0)
                + (inderectManpower.AutoElasticMake ?? 0)
                + (inderectManpower.MedicalLeave ?? 0)
                + (inderectManpower.TraineeSupervisor ?? 0)
                + (inderectManpower.Repoter ?? 0)
                + (inderectManpower.Others ?? 0);
        }


        await _context.SaveChangesAsync();


        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblSewingEfficiency
            .FirstOrDefaultAsync(x => x.Trid == id);

        if (data == null)
        {
            return NotFound();
        }

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Delete Indirect Manpower
            var indirect = await _context.TblInderectManpower
                .Where(x => x.Trid == id)
                .ToListAsync();

            if (indirect.Any())
            {
                _context.TblInderectManpower.RemoveRange(indirect);
            }

            // Delete Sewing Efficiency
            var sewing = await _context.TblSewingEfficiency
                .FirstOrDefaultAsync(x => x.Trid == id);

            if (sewing != null)
            {
                _context.TblSewingEfficiency.Remove(sewing);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["Success"] = "Deleted Successfully.";
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            TempData["Error"] = "Delete Failed.";
        }

        return RedirectToAction(nameof(SewingEfficiencyList));
    }
}
