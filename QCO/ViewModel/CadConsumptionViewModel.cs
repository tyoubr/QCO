using QCO.Models;

namespace QCO.ViewModel
{
    public class CadConsumptionViewModel
    {
        public TblCadConsM Master { get; set; } = new TblCadConsM();
        public List<TblCadConsD> Details { get; set; } = new List<TblCadConsD>();
    }
}
