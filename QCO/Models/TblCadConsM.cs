using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QCO.Models;

public partial class TblCadConsM
{
    public int Cadmid { get; set; }

    [Required]
    public DateTime? Caddate { get; set; }
    [Required]

    public string? Styleref { get; set; }

    public string? Ir { get; set; }

    public string? Job { get; set; }

    public string? Style { get; set; }

    public string? Buyer { get; set; }

    public string? Brand { get; set; }

    public string? Season { get; set; }

    public string? Seasonyear { get; set; }
    [Required]
    public string? Styledes { get; set; }

    public string? Patternmaster { get; set; }

    public string? Consfor { get; set; }

    public bool? Isapproved { get; set; }

    public string? Comments { get; set; }
    [Required]
    public string? Opt01 { get; set; } //For Company

    public string? Opt02 { get; set; }

    public string? Opt03 { get; set; }

    public virtual ICollection<TblCadConsD> TblCadConsDs { get; set; } = new List<TblCadConsD>();
}
