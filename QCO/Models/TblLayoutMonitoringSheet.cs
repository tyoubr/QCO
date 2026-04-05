using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCO.Models;

public partial class TblLayoutMonitoringSheet
{
    [Key]
    [Column("SLNO")]  // ✅ Explicit column mapping
    public int Slno { get; set; }
    public string? Company {  get; set; }
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

    public string? Usrid {  get; set; }

    // ✅ Ensure this navigation property is properly mapped
    public virtual ICollection<TblLayoutMonitoringSheetD> DetailRecords { get; set; } = new List<TblLayoutMonitoringSheetD>();
}
