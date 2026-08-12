using Microsoft.AspNetCore.Http;
using QCO.Models;
using System.Collections.Generic;

namespace QCO.ViewModels
{
    public class RecapViewModel
    {
        public TblRecapMaster Master { get; set; } = new();

        public List<TblRecapDetails> Details { get; set; } = new();

        public List<TblRecapItemDetails> ItemDetails { get; set; } = new();

        public IFormFile? PhotoFile { get; set; }
    }
}

