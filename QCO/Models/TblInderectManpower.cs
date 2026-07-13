using System;
using System.Collections.Generic;

namespace QCO.Models;

public partial class TblInderectManpower
{
    public int Impid { get; set; }

    public int? Trid { get; set; }

    public int? Pregnent { get; set; }

    public int? SizeSetSample { get; set; }

    public int? AutoElasticMake { get; set; }

    public int? MedicalLeave { get; set; }

    public int? TraineeSupervisor { get; set; }

    public int? Repoter { get; set; }

    public int? Others { get; set; }

    public int? Total { get; set; }

    public string? Remarks { get; set; }

    public virtual TblSewingEfficiency? Tr { get; set; }
}
