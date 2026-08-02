using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QCO.Models;

public partial class TblRecapeInfo
{
    [Key]
    public int Trid { get; set; }
    public DateTime? Trdate { get; set; }
    public string? StyleRef { get; set; }
    public string? SeasonName { get; set; }
    public double? OfferdQty { get; set; }
    public double? QuotedPrice { get; set; }
    public double? TgtPrice { get; set; }
    public int? SeasonYear { get; set; }
    public string? BuyerName { get; set; }
    public string? TeamLeaderName { get; set; }
    public string? E1 { get; set; }
    public string? E2 { get; set; }
    public string? E3 { get; set; }
    public string? E4 { get; set; }
    public string? E5 { get; set; }

    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
