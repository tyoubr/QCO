using System;
using System.Collections.Generic;

namespace QCO.Models;

public partial class TblCadConsM
{
    public int Cadmid { get; set; }

    public DateTime? Caddate { get; set; }

    public string? Styleref { get; set; }

    public string? Ir { get; set; }

    public string? Job { get; set; }

    public string? Style { get; set; }

    public string? Buyer { get; set; }

    public string? Brand { get; set; }

    public string? Season { get; set; }

    public DateTime? Seasonyear { get; set; }

    public string? Styledes { get; set; }

    public byte[]? Patternmaster { get; set; }

    public string? Consfor { get; set; }

    public bool? Isapproved { get; set; }

    public string? Comments { get; set; }

    public string? Opt01 { get; set; }

    public string? Opt02 { get; set; }

    public string? Opt03 { get; set; }

    public virtual ICollection<TblCadConsD> TblCadConsDs { get; set; } = new List<TblCadConsD>();
}
