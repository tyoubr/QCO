using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCO.Models;

public partial class TblCadConsD
{
    public int Caddid { get; set; }
    public DateTime? Transdate { get; set; }
    public int? Cadmid { get; set; }
    public string? Ptnnmbr { get; set; }
    public string? Gmntitem { get; set; }
    public string? Gmntcolor { get; set; }
    public string? Fabricdes { get; set; }
    [Display(Name ="Body Parts")]
    public string? Fabricusage { get; set; }//Body Part
    public double? Gsm { get; set; }
    public double? Fullwidth { get; set; }
    public double? Cutwidth { get; set; }
    public double? Efficiency { get; set; }
    public string? Sizeratio { get; set; }
    public double? Markerqty { get; set; }
    public double? Conspcs { get; set; }
    public double? Consdzn { get; set; }
    public double? Wastage { get; set; }
    public double? Shrinkagel { get; set; }
    public double? Shrinkagew { get; set; }
    public string? Comments { get; set; }
    [DisplayName("Marker Length")]
    public double? Opt01 { get; set; }//Marker Length
    //Only For showing in UI
    [NotMapped]
    public string? OriginalFilename { get; set; }
    public virtual TblCadConsM? Cadm { get; set; }
    // Navigation to multiple files
    public virtual ICollection<TblCadConsFiles> TblCadConsFiles { get; set; }
        = new List<TblCadConsFiles>();
}
