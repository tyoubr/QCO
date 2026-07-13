using System;
using System.Collections.Generic;

namespace QCO.Models;

public partial class TblSewingEfficiency
{
    public int Trid { get; set; }

    public DateTime? Trdate { get; set; }

    public string? Company { get; set; }

    public int? Floor { get; set; }

    public DateTime? SwDate { get; set; }

    public string? LineNo { get; set; }

    public string? BookingNo { get; set; }

    public string? Style { get; set; }

    public string? Item { get; set; }

    public int? Operator { get; set; }

    public int? Helper { get; set; }

    public int? ManPower { get; set; }

    public int? GenWorkingHr { get; set; }

    public int? SixPm { get; set; }

    public int? SevenPm { get; set; }

    public int? EightPm { get; set; }

    public int? NinePm { get; set; }

    public int? TenPm { get; set; }

    public int? ElevenPm { get; set; }

    public int? TwelveAm { get; set; }

    public decimal? Smv { get; set; }

    public decimal? EfficiencyPercent { get; set; }

    public int? StyleTarget { get; set; }

    public decimal? LineTarget { get; set; }

    public int? InderectMan { get; set; }

    public decimal? AfterWashProduction { get; set; }
    public virtual ICollection<TblInderectManpower> TblInderectManpowers { get; set; } = new List<TblInderectManpower>();

}
