using Microsoft.AspNetCore.Mvc.Rendering;

namespace QCO.ViewModel
{
    public class RecapeCreateVM
    {
        public string StyleRef { get; set; }
        public string SeasonName { get; set; }
        public double? QuotedPrice { get; set; }
        public double? TgtPrice { get; set; }
        public int? SeasonYear { get; set; }
        public string BuyerName { get; set; }
        public string TeamLeaderName { get; set; }

        public string E1 { get; set; }
        public string E2 { get; set; }
        public string E3 { get; set; }
        public string E4 { get; set; }
        public string E5 { get; set; }

        public List<SelectListItem> StyleRefList { get; set; }
    }
}
