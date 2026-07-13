using QCO.Models;

public class SewingEfficiencyViewModel
{
    // Header Information
    public TblSewingEfficiency Header { get; set; } = new();

    // SP Result + User Input
    public List<SewingEfficiencyDetailModel> Details { get; set; } = new();
    public List<SewingEfficiencyEditDetailModel> EditDetails { get; set; } = new();
}