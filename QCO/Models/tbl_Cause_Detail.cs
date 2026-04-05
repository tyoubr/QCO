using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCO.Models;

public partial class tbl_Cause_Detail
{
    public int DID { get; set; }

    //[ForeignKey("TblLayoutMonitoringSheetD")]
    public int? TRNSID { get; set; }

    public int? CAUSEID { get; set; }

    public int? TIME { get; set; }

    public string? REMARKS { get; set; }

}
