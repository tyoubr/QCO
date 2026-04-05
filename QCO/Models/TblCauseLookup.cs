// Models/TblCauseLookup.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCO.Models
{
    public class TblCauseLookup
    {
        [Key]
        public int CAUSEID { get; set; }
        public string CAUSE_OF_DELAY { get; set; }
    }
}