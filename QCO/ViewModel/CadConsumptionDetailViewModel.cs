using Microsoft.AspNetCore.Http;
using QCO.Models;

namespace QCO.ViewModel
{
    public class CadConsumptionDetailViewModel
    {
        // Existing detail ID
        public int Caddid { get; set; }

        public DateTime? Transdate { get; set; }

        public int? Cadmid { get; set; }

        public string? Ptnnmbr { get; set; }
        public string? Gmntitem { get; set; }
        public string? Gmntcolor { get; set; }
        public string? Fabricdes { get; set; }
        public string? Fabricusage { get; set; }

        public double? Gsm { get; set; }
        public double? Fullwidth { get; set; }
        public double? Cutwidth { get; set; }
        public double? Efficiency { get; set; }

        public string? Sizeratio { get; set; }

        public double? Markerqty { get; set; }
        public double? Conspcs { get; set; }
        public double? Consdzn { get; set; }
        public double? Wastage { get; set; }

        public string? Comments { get; set; }

        public double? Opt01 { get; set; }
        public List<int> DeletedFileIds { get; set; } = new List<int>();


        // =========================
        // NEW FILE UPLOAD
        // =========================
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();

        // =========================
        // EXISTING FILES - EDIT
        // =========================
        public List<TblCadConsFiles> ExistingFiles { get; set; }
            = new List<TblCadConsFiles>();
    }
}
