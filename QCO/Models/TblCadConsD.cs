using System;
using System.Collections.Generic;

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

    public double? Shrinkagel { get; set; }

    public double? Shrinkagew { get; set; }

    public string? Filename { get; set; }

    public string? Filepath { get; set; }

    public string? Filesize { get; set; }

    public string? Contenttype { get; set; }

    public string? Comments { get; set; }

    public string? Opt01 { get; set; }

    public string? Opt02 { get; set; }

    public string? Opt03 { get; set; }

    public virtual TblCadConsM? Cadm { get; set; }
}
