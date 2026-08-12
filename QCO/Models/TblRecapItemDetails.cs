using System;
using System.Collections.Generic;

namespace QCO.Models;

public partial class TblRecapItemDetails
{
    public int Itemid { get; set; }

    public int? Rcmid { get; set; }

    public string? ItemName { get; set; }

    public double? OfferedQty { get; set; }

    public double? QuoatedPrice { get; set; }

    public int? IsPrint { get; set; }

    public int? IsWash { get; set; }

    public int? IsEmb { get; set; }

    public virtual TblRecapMaster? Rcm { get; set; }
}
