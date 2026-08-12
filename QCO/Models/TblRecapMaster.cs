using System;
using System.Collections.Generic;

namespace QCO.Models;

public partial class TblRecapMaster
{
    public int Rcmid { get; set; }

    public string? StyleName { get; set; }
    public string? Remarks { get; set; }

    public string? BuyerName { get; set; }

    public string? BookingNo { get; set; }

    public string? PoNo { get; set; }
    public string? TeamLeaderName { get; set; }

    public string? RecapMonth { get; set; }

    public int? RecapeYear { get; set; }

    public DateOnly? FacShipmentDate { get; set; }

    public DateOnly? ActShipmentDate { get; set; }

    public DateOnly? SubmissionDate { get; set; }

    public byte[]? Photo { get; set; }
    public string? SewingFactory { get; set; }
    public string? DyeingFactory { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<TblRecapDetails> TblRecapDetails { get; set; } = new List<TblRecapDetails>();
    public virtual ICollection<TblRecapItemDetails> TblRecapItemDetails { get; set; } = new List<TblRecapItemDetails>();
}
