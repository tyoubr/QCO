using System;
using System.Collections.Generic;

namespace QCO.Models;

public partial class tbl_Cause_Of_Delay
{
    public int CAUSEID { get; set; }

    public string? CAUSE_OF_DELAY { get; set; }

    public string? REMARKS { get; set; }
}
