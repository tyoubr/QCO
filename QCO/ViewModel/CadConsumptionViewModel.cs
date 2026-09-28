using QCO.Models;

namespace QCO.ViewModel
{
    public class CadConsumptionViewModel
    {
        public TblCadConsM Master { get; set; } = new TblCadConsM();

        public List<CadConsumptionDetailViewModel> Details { get; set; }
            = new List<CadConsumptionDetailViewModel>();
    }
}
