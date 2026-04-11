using QCO.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCO.ViewModel
{
    public class CadConsumptionViewModel
    {
        public TblCadConsM Master { get; set; } = new TblCadConsM();
        public List<TblCadConsD> Details { get; set; } = new List<TblCadConsD>();

        //public int Cadmid { get; set; }

        //public DateTime? Caddate { get; set; }
        //[Required]
        //public string? Styleref { get; set; }

        //public string? Ir { get; set; }

        //public string? Job { get; set; }

        //public string? Style { get; set; }

        //public string? Buyer { get; set; }

        //public string? Brand { get; set; }

        //public string? Season { get; set; }

        //public DateTime? Seasonyear { get; set; }

        //public string? Styledes { get; set; }

        //public string? Patternmaster { get; set; }

        //public string? Consfor { get; set; }

        //public bool? Isapproved { get; set; }

        //public string? Comments { get; set; }

        //public string? Opt01 { get; set; }

        //public string? Opt02 { get; set; }

        //public string? Opt03 { get; set; }

        //public int Caddid { get; set; }

        //public DateTime? Transdate { get; set; }

        //public string? Ptnnmbr { get; set; }

        //public string? Gmntitem { get; set; }

        //public string? Gmntcolor { get; set; }

        //public string? Fabricdes { get; set; }

        //public string? Fabricusage { get; set; }

        //public double? Gsm { get; set; }

        //public double? Fullwidth { get; set; }

        //public double? Cutwidth { get; set; }

        //public double? Efficiency { get; set; }

        //public string? Sizeratio { get; set; }

        //public double? Markerqty { get; set; }

        //public double? Conspcs { get; set; }

        //public double? Consdzn { get; set; }

        //public double? Wastage { get; set; }

        //public double? Shrinkagel { get; set; }

        //public double? Shrinkagew { get; set; }

        //public string? Filename { get; set; }
        //[NotMapped] // Do not map this to DB
        //public IFormFile? UploadFile { get; set; } // For uploaded file

        //public string? Filepath { get; set; }

        //public long? Filesize { get; set; }

        //public string? Contenttype { get; set; }
    }
}
