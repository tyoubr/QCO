using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QCO.Data;
using QCO.Models;
using QCO.ViewModel;

public class RecapeInfoController : Controller
{
    private readonly QCOContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public RecapeInfoController(QCOContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> RecapeInfoList(int page = 1, string searchString = "")
    {
        int pageSize = 15;

        var query = _context.TblRecapeInfo.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();

            query = query.Where(x =>
                x.StyleRef.Contains(searchString) ||
                x.SeasonName.Contains(searchString) ||
                x.BuyerName.Contains(searchString) ||
                x.TeamLeaderName.Contains(searchString)
            );
        }

        var totalRecords = await query.CountAsync();

        var data = await query
            .OrderBy(x => x.Trid)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        ViewBag.SearchString = searchString;
        ViewBag.totalRecapeInfo = totalRecords;

        return View(data);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.StyleRefList = await GetStyleRefList();

        return View(new TblRecapeInfo());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblRecapeInfo recapeInfo)
    {
        try
        {
            var now = DateTime.Now;
            var currentUser = User.Identity?.Name ?? "System";

            recapeInfo.CreatedAt = now;
            recapeInfo.CreatedBy = currentUser;

            _context.TblRecapeInfo.Add(recapeInfo);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Created successfully.";

            return RedirectToAction(nameof(RecapeInfoList));
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to create.";

            // reload dropdown data
            ViewBag.StyleRefList = await GetStyleRefList();

            return View(recapeInfo);
        }
    }
    [HttpGet]
    public async Task<IActionResult> GetRecapeInfo(string styleRef)
    {
        var result = new TblRecapeInfo();

        try
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SP_RECAPE";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                var parameter = command.CreateParameter();
                parameter.ParameterName = "@STYLE_REF";
                parameter.Value = styleRef;
                command.Parameters.Add(parameter);

                await _context.Database.OpenConnectionAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        result.StyleRef = reader["STYLE_REF"]?.ToString();

                        result.SeasonName = reader["SEASON_NAME"] == DBNull.Value
                            ? null
                            : reader["SEASON_NAME"].ToString();

                        result.QuotedPrice = reader["QUOTED_PRICE"] == DBNull.Value
                            ? null
                            : Convert.ToDouble(reader["QUOTED_PRICE"]);

                        result.TgtPrice = reader["TGT_PRICE"] == DBNull.Value
                            ? null
                            : Convert.ToDouble(reader["TGT_PRICE"]);

                        result.SeasonYear = reader["SEASON_YEAR"] == DBNull.Value
                            ? null
                            : Convert.ToInt32(reader["SEASON_YEAR"]);

                        result.BuyerName = reader["BUYER_NAME"] == DBNull.Value
                            ? null
                            : reader["BUYER_NAME"].ToString();

                        result.TeamLeaderName = reader["TEAM_LEADER_NAME"] == DBNull.Value
                            ? null
                            : reader["TEAM_LEADER_NAME"].ToString();
                    }
                }
            }
        }
        finally
        {
            _context.Database.CloseConnection();
        }

        return Json(result);
    }

    public class StyleRefDropdown
    {
        public string StyleRef { get; set; }
        public string SeasonName { get; set; }
    }

    private async Task<List<StyleRefDropdown>> GetStyleRefList()
    {
        var styleRefList = new List<StyleRefDropdown>();

        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "SP_RECAPE_STYLE_LIST";
            command.CommandType = System.Data.CommandType.StoredProcedure;

            await _context.Database.OpenConnectionAsync();

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    styleRefList.Add(new StyleRefDropdown
                    {
                        StyleRef = reader["STYLE_REF"].ToString(),
                        SeasonName = reader["SEASON_NAME"] == DBNull.Value
                            ? ""
                            : reader["SEASON_NAME"].ToString()
                    });
                }
            }
        }

        _context.Database.CloseConnection();

        return styleRefList;
    }
}


