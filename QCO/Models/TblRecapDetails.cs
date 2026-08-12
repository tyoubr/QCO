using QCO.Models;
using System;
using System.Collections.Generic;

namespace QCO.Models;

public partial class TblRecapDetails
{
    public int Rcdid { get; set; }

    public int? Rcmid { get; set; }

    public string? ItemName { get; set; }
    public string? BodyPart { get; set; }
    public string? Fabrication { get; set; }
    public double? Gsm { get; set; }
    public string? ColorName { get; set; }
    public double? ConsPerUnit { get; set; }
    public double? TotalQty { get; set; }

    public virtual TblRecapMaster? Rcm { get; set; }
}
