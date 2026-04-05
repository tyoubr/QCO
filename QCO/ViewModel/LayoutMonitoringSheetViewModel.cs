using QCO.Models;

public class LayoutMonitoringSheetViewModel
{
    public int Slno { get; set; } // ✅ Ensure this matches 'SLNO' in DB
    public string? Company { get; set; }
    public DateTime? MonitoringDate { get; set; }
    public string? BookingNo { get; set; }
    public string? BuyerName { get; set; }
    public string? LineNo { get; set; }
    public float? Total_SMV { get; set; }
    public DateTime? FeedStart { get; set; }
    public DateTime? FeedFinish { get; set; }
    public DateTime? PreStyleFinish { get; set; }
    public DateTime? NewStyleFinish { get; set; }
    public string? Status { get; set; }
    public string? Usrid { get; set; }

    public List<DetailViewModel> DetailRecords { get; set; } = new List<DetailViewModel>();
}


public class DetailViewModel
{

        public int Trnsid { get; set; }
        public int? SequenceNo { get; set; }
        public string? ProcessName { get; set; }
        public string? ResourceName { get; set; }
        public TimeOnly? McSetupStart { get; set; }
        public TimeOnly? McSetupFinish { get; set; }
        public decimal? McSetupDuration { get; set; }
        public string? Remarks { get; set; }
        public List<int> SelectedCauses { get; set; } = new List<int>(); // Ensure this is initialized
        public List<int> CauseTimes { get; set; } = new List<int>(); // Ensure this is initialized
        public List<tbl_Cause_Of_Delay> Causes { get; set; } = new List<tbl_Cause_Of_Delay>();
       
}
public class Cause
{
    public int CAUSEID { get; set; }
    public string CAUSE_OF_DELAY { get; set; }
}