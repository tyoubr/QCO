using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCO.Models
{
    public partial class TblLayoutMonitoringSheetD
    {
        [Key]
        public int Trnsid { get; set; }

        // Foreign Key pointing to the master table
        public int? Slno { get; set; }

        public int? SequenceNo { get; set; }
        public string? ProcessName { get; set; }
        public string? ResourceName { get; set; }
        public TimeOnly? McSetupStart { get; set; }
        public TimeOnly? McSetupFinish { get; set; }
        public decimal? McSetupDuration { get; set; }
        public string? Remarks { get; set; }
        public TblLayoutMonitoringSheet LayoutMonitoringSheet { get; set; }

        // Collection of causes related to this detail
        public virtual ICollection<tbl_Cause_Detail> SavedCauses { get; set; } = new List<tbl_Cause_Detail>();
    }
   

}
