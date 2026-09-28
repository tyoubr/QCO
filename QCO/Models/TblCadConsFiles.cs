using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QCO.Models;
public class TblCadConsFiles
{
    public int Fileid { get; set; }

    public int Caddid { get; set; }

    public string? Filename { get; set; }

    public string? Filepath { get; set; }

    public long? Filesize { get; set; }

    public string? Contenttype { get; set; }

    public DateTime? Createddate { get; set; }

    public string? Createdby { get; set; }

    // Navigation
    public virtual TblCadConsD? CadDetail { get; set; }
}
