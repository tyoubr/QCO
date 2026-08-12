using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QCO.Data;
using QCO.Models;
using QCO.ViewModel;
using QCO.ViewModels;
using System.Data;

public class RecapeInfoController : Controller
{
    private readonly QCOContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public RecapeInfoController(
        QCOContext context,
        UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // =========================================================
    // INDEX
    // =========================================================

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    // =========================================================
    // MASTER LIST
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> RecapeInfoList(
        int page = 1,
        string searchString = "")
    {
        int pageSize = 15;

        if (page < 1)
        {
            page = 1;
        }

        var query = _context.TblRecapMasters
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();

            query = query.Where(x =>
                (x.StyleName != null &&
                 x.StyleName.Contains(searchString)) ||

                (x.BuyerName != null &&
                 x.BuyerName.Contains(searchString)) ||

                (x.BookingNo != null &&
                 x.BookingNo.Contains(searchString)) ||

                (x.PoNo != null &&
                 x.PoNo.Contains(searchString))
            );
        }

        var totalRecords = await query.CountAsync();

        var data = await query
            .OrderByDescending(x => x.Rcmid)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;

        ViewBag.TotalPages =
            (int)Math.Ceiling(
                totalRecords / (double)pageSize
            );

        ViewBag.SearchString = searchString;
        ViewBag.TotalRecapInfo = totalRecords;

        return View(data);
    }

    // =========================================================
    // CREATE - GET
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        try
        {
            ViewBag.StyleNameList = await GetStyleNameList();
            ViewBag.FabricationList = await GetFabricationList();
            ViewBag.ItemNameList = await GetItemNameList();
            ViewBag.BookingNoList = await GetBookingNoList();

            ViewBag.TeamLeaderList = await GetTeamLeaderList();


            // If you want Team Leader list later:
            // ViewBag.TeamLeaderList =
            //     await GetTeamLeaderList();

            var model = new RecapViewModel();

            // Keep one empty item row in model
            if (model.ItemDetails == null)
            {
                model.ItemDetails =
                    new List<TblRecapItemDetails>();
            }

            model.ItemDetails.Add(
                new TblRecapItemDetails()
            );

            return View(model);
        }
        catch (Exception ex)
        {
            TempData["Error"] =
                "Failed to load Create Recap page. " +
                ex.Message;

            return View(new RecapViewModel());
        }
    }

    // =========================================================
    // CREATE - POST
    // MASTER + RECAP DETAILS + ITEM DETAILS
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RecapViewModel recapViewModel)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            // =================================================
            // BASIC VALIDATION
            // =================================================

            if (recapViewModel == null)
            {
                TempData["Error"] = "Invalid recap information.";

                return RedirectToAction(nameof(Create));
            }

            if (recapViewModel.Master == null)
            {
                TempData["Error"] = "Master information is required.";

                return RedirectToAction(nameof(Create));
            }


            // =================================================
            // CURRENT USER
            // =================================================

            var now = DateTime.Now;

            var currentUser =
                User.Identity?.Name ?? "System";


            // =================================================
            // PHOTO
            // =================================================

            if (recapViewModel.PhotoFile != null &&
                recapViewModel.PhotoFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();

                await recapViewModel.PhotoFile.CopyToAsync(
                    memoryStream
                );

                recapViewModel.Master.Photo =
                    memoryStream.ToArray();
            }


            // =================================================
            // MASTER AUDIT
            // =================================================

            recapViewModel.Master.CreatedAt = now;
            recapViewModel.Master.CreatedBy = currentUser;
            recapViewModel.Master.Remarks = recapViewModel.Master.BookingNo != null ? "Tagged" : "Pending";


            // =================================================
            // SAVE MASTER FIRST
            // =================================================

            _context.TblRecapMasters.Add(
                    recapViewModel.Master
                );

            await _context.SaveChangesAsync();


            // =================================================
            // MASTER ID
            // =================================================

            int masterId =
                recapViewModel.Master.Rcmid;


            // =================================================
            // SAVE RECAP DETAILS
            // =================================================

            var validDetails =
                recapViewModel.Details?
                    .Where(x => x != null)
                    .ToList()
                ?? new List<TblRecapDetails>();


            if (validDetails.Count > 0)
            {
                foreach (var detail in validDetails)
                {
                    detail.Rcmid = masterId;
                }

                await _context.TblRecapDetails.AddRangeAsync(
                    validDetails
                );

                await _context.SaveChangesAsync();
            }


            // =================================================
            // SAVE ITEM DETAILS
            // =================================================

            var validItemDetails =
                recapViewModel.ItemDetails?
                    .Where(x => x != null)
                    .ToList()
                ?? new List<TblRecapItemDetails>();


            if (validItemDetails.Count > 0)
            {
                foreach (var itemDetail in validItemDetails)
                {
                    itemDetail.Rcmid = masterId;
                }

                await _context.TblRecapItemDetails.AddRangeAsync(
                    validItemDetails
                );

                await _context.SaveChangesAsync();
            }


            // =================================================
            // IMPORTANT VALIDATION
            // =================================================

            if (validDetails.Count == 0)
            {
                throw new Exception(
                    "At least one Recap Detail is required."
                );
            }

            if (validItemDetails.Count == 0)
            {
                throw new Exception(
                    "At least one Item Detail is required."
                );
            }


            // =================================================
            // COMMIT
            // =================================================

            await transaction.CommitAsync();


            // =================================================
            // SUCCESS
            // =================================================

            TempData["Success"] =
                "Recap created successfully.";

            return RedirectToAction(
                nameof(RecapeInfoList)
            );
        }
        catch (Exception ex)
        {
            // =================================================
            // ROLLBACK
            // =================================================

            await transaction.RollbackAsync();


            // =================================================
            // ERROR
            // =================================================

            TempData["Error"] =
                "Failed to create recap. " +
                ex.Message;


            // =================================================
            // RELOAD DROPDOWNS
            // =================================================

            try
            {
                ViewBag.StyleNameList =
                    await GetStyleNameList();

                ViewBag.FabricationList =
                    await GetFabricationList();

                ViewBag.ItemNameList =
                    await GetItemNameList();

                ViewBag.BookingNoList =
                    await GetBookingNoList();

                ViewBag.TeamLeaderList =
                    await GetTeamLeaderList();
            }
            catch
            {
                // Ignore dropdown reload errors
            }


            // =================================================
            // RETURN VIEW
            // =================================================

            return View(recapViewModel);
        }
    }

    // =========================================================
    // GET DETAILS BY MASTER ID
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> GetDetails(int id)
    {
        var details = await _context.TblRecapDetails
            .AsNoTracking()
            .Where(x => x.Rcmid == id)
            .OrderBy(x => x.Rcdid)
            .ToListAsync();

        return Json(details);
    }

    // =========================================================
    // GET MASTER + DETAILS
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> GetRecap(int id)
    {
        var recap =
            await _context.TblRecapMasters
                .AsNoTracking()
                .Include(x => x.TblRecapDetails)
                .FirstOrDefaultAsync(
                    x => x.Rcmid == id
                );

        if (recap == null)
        {
            return NotFound();
        }

        return Json(recap);
    }

    // =========================================================
    // DELETE
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var master =
                await _context.TblRecapMasters
                    .Include(x => x.TblRecapDetails)
                    .FirstOrDefaultAsync(
                        x => x.Rcmid == id
                    );

            if (master == null)
            {
                TempData["Error"] =
                    "Recap not found.";

                return RedirectToAction(
                    nameof(RecapeInfoList)
                );
            }

            // -------------------------------------------------
            // DELETE DETAILS
            // -------------------------------------------------

            if (master.TblRecapDetails != null &&
                master.TblRecapDetails.Any())
            {
                _context.TblRecapDetails
                    .RemoveRange(
                        master.TblRecapDetails
                    );
            }

            // -------------------------------------------------
            // DELETE MASTER
            // -------------------------------------------------

            _context.TblRecapMasters
                .Remove(master);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Recap deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] =
                "Failed to delete recap. " +
                ex.Message;
        }

        return RedirectToAction(
            nameof(RecapeInfoList)
        );
    }

    // =========================================================
    // PHOTO
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> GetPhoto(int id)
    {
        var photo =
            await _context.TblRecapMasters
                .AsNoTracking()
                .Where(x => x.Rcmid == id)
                .Select(x => x.Photo)
                .FirstOrDefaultAsync();

        if (photo == null ||
            photo.Length == 0)
        {
            return NotFound();
        }

        return File(
            photo,
            "image/jpeg"
        );
    }

    // =========================================================
    // DROPDOWN MODEL - STYLE
    // =========================================================

    public class StyleNameDropdown
    {
        public string StyleName { get; set; }
            = string.Empty;

        public string BuyerName { get; set; }
            = string.Empty;
    }

    // =========================================================
    // DROPDOWN MODEL - ITEM
    // =========================================================

    public class ItemNameDropdown
    {
        public string ItemName { get; set; }
            = string.Empty;
    }
    public class TeamLeaderDropdown
    {
        public string TeamLeaderName { get; set; }
            = string.Empty;
    }

    // =========================================================
    // DROPDOWN MODEL - FABRICATION
    // =========================================================

    public class FabricationDropdown
    {
        public string? FabricationName { get; set; }
    }

    // =========================================================
    // BOOKING NO DROPDOWN MODEL
    // =========================================================

    public class BookingNoDropdown
    {
        public string BookingNo { get; set; } = string.Empty;

        public string PoNo { get; set; } = string.Empty;

        public string ItemName { get; set; } = string.Empty;

        public string BodyPart { get; set; } = string.Empty;

        public string Fabrication { get; set; } = string.Empty;

        public string Gsm { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public string ConsPcs { get; set; } = string.Empty;

        public string TotalQty { get; set; } = string.Empty;
    }


    // =========================================================
    // GET BOOKING NO LIST
    // =========================================================

    [HttpGet]
    private async Task<List<BookingNoDropdown>> GetBookingNoList()
    {
        var bookingNoList =
            new List<BookingNoDropdown>();


        var connection =
            _context.Database.GetDbConnection();


        try
        {
            // =================================================
            // CHECK CONNECTION STRING
            // =================================================

            if (string.IsNullOrWhiteSpace(
                connection.ConnectionString))
            {
                throw new InvalidOperationException(
                    "Database connection string is not configured."
                );
            }


            // =================================================
            // OPEN DATABASE CONNECTION
            // =================================================

            if (connection.State !=
                System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }


            // =================================================
            // CREATE COMMAND
            // =================================================

            using var command =
                connection.CreateCommand();


            command.CommandText =
                "SP_RECAPE_BOOKINGNO_LIST";


            command.CommandType =
                CommandType.StoredProcedure;


            // =================================================
            // EXECUTE STORED PROCEDURE
            // =================================================

            using var reader =
                await command.ExecuteReaderAsync();


            // =================================================
            // READ ALL RECORDS
            // =================================================

            while (await reader.ReadAsync())
            {
                var bookingNo =
                    reader["BOOKING_NO"] == DBNull.Value
                        ? string.Empty
                        : reader["BOOKING_NO"]?.ToString()
                          ?? string.Empty;


                var poNo =
                    reader["PO_NO"] == DBNull.Value
                        ? string.Empty
                        : reader["PO_NO"]?.ToString()
                          ?? string.Empty;


                var itemName =
                    reader["ITEM_NAME"] == DBNull.Value
                        ? string.Empty
                        : reader["ITEM_NAME"]?.ToString()
                          ?? string.Empty;


                var bodyPart =
                    reader["BODY_PART"] == DBNull.Value
                        ? string.Empty
                        : reader["BODY_PART"]?.ToString()
                          ?? string.Empty;


                var fabrication =
                    reader["FABRICATION"] == DBNull.Value
                        ? string.Empty
                        : reader["FABRICATION"]?.ToString()
                          ?? string.Empty;


                var gsm =
                    reader["GSM"] == DBNull.Value
                        ? string.Empty
                        : reader["GSM"]?.ToString()
                          ?? string.Empty;


                var color =
                    reader["COLOR"] == DBNull.Value
                        ? string.Empty
                        : reader["COLOR"]?.ToString()
                          ?? string.Empty;


                var consPcs =
                    reader["CONS_PCS"] == DBNull.Value
                        ? string.Empty
                        : reader["CONS_PCS"]?.ToString()
                          ?? string.Empty;


                var totalQty =
                    reader["TOTAL_QTY"] == DBNull.Value
                        ? string.Empty
                        : reader["TOTAL_QTY"]?.ToString()
                          ?? string.Empty;


                // =================================================
                // ADD RECORD
                // =================================================

                bookingNoList.Add(
                    new BookingNoDropdown
                    {
                        BookingNo = bookingNo,

                        PoNo = poNo,

                        ItemName = itemName,

                        BodyPart = bodyPart,

                        Fabrication = fabrication,

                        Gsm = gsm,

                        Color = color,

                        ConsPcs = consPcs,

                        TotalQty = totalQty
                    }
                );
            }
        }
        finally
        {
            // =================================================
            // CLOSE DATABASE CONNECTION
            // =================================================

            if (connection.State ==
                System.Data.ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }


        // =====================================================
        // RETURN RESULT
        // =====================================================

        return bookingNoList;
    }

    // =========================================================
    // GET STYLE NAME LIST
    // =========================================================

    private async Task<List<StyleNameDropdown>>
        GetStyleNameList()
    {
        var styleNameList =
            new List<StyleNameDropdown>();

        var connection =
            _context.Database.GetDbConnection();

        try
        {
            // ---------------------------------------------
            // IMPORTANT:
            // Connection must have a valid connection string
            // ---------------------------------------------

            if (string.IsNullOrWhiteSpace(
                connection.ConnectionString))
            {
                throw new InvalidOperationException(
                    "Database connection string is not configured."
                );
            }

            if (connection.State !=
                System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            using var command =
                connection.CreateCommand();

            command.CommandText =
                "SP_RECAPE_STYLE_LIST";

            command.CommandType =
                CommandType.StoredProcedure;

            using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                styleNameList.Add(
                    new StyleNameDropdown
                    {
                        StyleName =
                            reader["STYLE_REF"]
                                ?.ToString()
                            ?? string.Empty,

                        BuyerName =
                            reader["BUYER_NAME"]
                                == DBNull.Value
                                ? string.Empty
                                : reader["BUYER_NAME"]
                                    ?.ToString()
                                    ?? string.Empty
                    }
                );
            }
        }
        finally
        {
            if (connection.State ==
                System.Data.ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }

        return styleNameList;
    }

    // =========================================================
    // GET ITEM NAME LIST
    // =========================================================
    [HttpGet]
    private async Task<List<ItemNameDropdown>>
        GetItemNameList()
    {
        var itemNameList =
            new List<ItemNameDropdown>();

        var connection =
            _context.Database.GetDbConnection();

        try
        {
            // ---------------------------------------------
            // IMPORTANT:
            // Check connection string before opening
            // ---------------------------------------------

            if (string.IsNullOrWhiteSpace(
                connection.ConnectionString))
            {
                throw new InvalidOperationException(
                    "Database connection string is not configured."
                );
            }

            if (connection.State !=
                System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            using var command =
                connection.CreateCommand();

            command.CommandText =
                "SP_RECAPE_ITEM_LIST";

            command.CommandType =
                CommandType.StoredProcedure;

            using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                itemNameList.Add(
                    new ItemNameDropdown
                    {
                        ItemName =
                            reader["ITEM_NAME"]
                                ?.ToString()
                            ?? string.Empty
                    }
                );
            }
        }
        finally
        {
            if (connection.State ==
                System.Data.ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }

        return itemNameList;
    }

    // =========================================================
    // GET Team Leader NAME LIST
    // =========================================================
    [HttpGet]
    private async Task<List<TeamLeaderDropdown>>
        GetTeamLeaderList()
    {
        var teamLeaderList =
            new List<TeamLeaderDropdown>();

        var connection =
            _context.Database.GetDbConnection();

        try
        {
            // ---------------------------------------------
            // IMPORTANT:
            // Check connection string before opening
            // ---------------------------------------------

            if (string.IsNullOrWhiteSpace(
                connection.ConnectionString))
            {
                throw new InvalidOperationException(
                    "Database connection string is not configured."
                );
            }

            if (connection.State !=
                System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            using var command =
                connection.CreateCommand();

            command.CommandText =
                "SP_RECAPE_TEAM_LEADER_LIST";

            command.CommandType =
                CommandType.StoredProcedure;

            using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                teamLeaderList.Add(
                    new TeamLeaderDropdown
                    {
                        TeamLeaderName =
                            reader["TEAM_LEADER_NAME"]
                                ?.ToString()
                            ?? string.Empty
                    }
                );
            }
        }
        finally
        {
            if (connection.State ==
                System.Data.ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }

        return teamLeaderList;
    }

    // =========================================================
    // GET FABRICATION LIST
    // =========================================================

    private async Task<List<FabricationDropdown>>
        GetFabricationList()
    {
        var list =
            new List<FabricationDropdown>();

        var connection =
            _context.Database.GetDbConnection();

        try
        {
            // ---------------------------------------------
            // IMPORTANT:
            // Check connection string before opening
            // ---------------------------------------------

            if (string.IsNullOrWhiteSpace(
                connection.ConnectionString))
            {
                throw new InvalidOperationException(
                    "Database connection string is not configured."
                );
            }

            if (connection.State !=
                System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            using var command =
                connection.CreateCommand();

            command.CommandText =
                "SP_RECAPE_FABRICATION_LIST";

            command.CommandType =
                CommandType.StoredProcedure;

            using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(
                    new FabricationDropdown
                    {
                        FabricationName =
                            reader[
                                "FABRIC_COMPOSITION_NAME"
                            ]?.ToString()
                    }
                );
            }
        }
        finally
        {
            if (connection.State ==
                System.Data.ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }

        return list;
    }

    // =========================================================
    // GET BUYER BY STYLE
    // =========================================================

    [HttpGet]
    public async Task<IActionResult>
        GetBuyerByStyle(string styleName)
    {
        string buyerName = string.Empty;

        if (string.IsNullOrWhiteSpace(styleName))
        {
            return Json(new
            {
                buyerName
            });
        }

        var connection =
            _context.Database.GetDbConnection();

        try
        {
            // ---------------------------------------------
            // IMPORTANT:
            // Check connection string before opening
            // ---------------------------------------------

            if (string.IsNullOrWhiteSpace(
                connection.ConnectionString))
            {
                throw new InvalidOperationException(
                    "Database connection string is not configured."
                );
            }

            if (connection.State !=
                System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            using var command =
                connection.CreateCommand();

            command.CommandText =
                "SP_RECAP_BUYER_BY_STYLE";

            command.CommandType =
                CommandType.StoredProcedure;

            var parameter =
                command.CreateParameter();

            parameter.ParameterName =
                "@STYLE_NAME";

            parameter.Value =
                styleName.Trim();

            command.Parameters.Add(
                parameter
            );

            using var reader =
                await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                buyerName =
                    reader["BUYER_NAME"]
                        == DBNull.Value
                        ? string.Empty
                        : reader["BUYER_NAME"]
                            ?.ToString()
                            ?? string.Empty;
            }
        }
        finally
        {
            if (connection.State ==
                System.Data.ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }

        return Json(new
        {
            buyerName
        });
    }

    // =========================================================
    // GET RECAP DETAILS BY INTERNAL REF / BOOKING NO
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> GetDetailsByBookingNo(
        string bookingNo)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(bookingNo))
            {
                return Json(new
                {
                    success = false,
                    message = "Internal Ref. is required.",
                    data = new List<object>()
                });
            }

            bookingNo = bookingNo.Trim();

            // -------------------------------------------------
            // Get all booking data from Stored Procedure
            // -------------------------------------------------

            var bookingList =
                await GetBookingNoList();

            // -------------------------------------------------
            // Filter selected Internal Ref.
            // -------------------------------------------------

            var details =
                bookingList
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x.BookingNo) &&
                        x.BookingNo.Trim()
                            .Equals(
                                bookingNo,
                                StringComparison.OrdinalIgnoreCase
                            )
                    )
                    .Select(x => new
                    {
                        poNo = x.PoNo,
                        itemName = x.ItemName,
                        bodyPart = x.BodyPart,
                        fabrication = x.Fabrication,
                        gsm = x.Gsm,
                        color = x.Color,
                        consPcs = x.ConsPcs,
                        totalQty = x.TotalQty
                    })
                    .ToList();

            // -------------------------------------------------
            // No data
            // -------------------------------------------------

            if (!details.Any())
            {
                return Json(new
                {
                    success = true,
                    message = "No recap details found.",
                    poNo = "",
                    data = new List<object>()
                });
            }

            // -------------------------------------------------
            // PO Number
            // -------------------------------------------------

            var poNo =
                details
                    .Select(x => x.poNo)
                    .FirstOrDefault(x =>
                        !string.IsNullOrWhiteSpace(x)
                    )
                ?? string.Empty;

            // -------------------------------------------------
            // SUCCESS
            // -------------------------------------------------

            return Json(new
            {
                success = true,
                message = "Recap details loaded successfully.",
                poNo = poNo,
                data = details
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message,
                data = new List<object>()
            });
        }
    }
}
