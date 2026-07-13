using Microsoft.EntityFrameworkCore;


namespace QCO.Models
{
    [Keyless]

    // ===== SP থেকে আসবে =====

    public class SewingEfficiencyDetailModel
    {
        // SP থেকে আসবে, পরে edit করা যাবে
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
        public decimal? LineTarget { get; set; }

        // User edit করবে
        //public double? StyleTarget { get; set; }
        //public double? InderectMan { get; set; }
        //public double? AfterWashProduction { get; set; }
    }
}
