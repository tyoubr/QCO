using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Mvc;

namespace QCO.Controllers
{
    public class ReportCallingController : Controller
    {
        public IActionResult Index()
        {
            try
            {
                var rptPath = "QCO.Reports.rptHitRate11";
                XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
                ViewBag.ReportName = report;
                return View("~/Views/Shared/_LayoutReport.cshtml");
            }
            catch (Exception ex)
            {

                throw ex.InnerException;
            }

        }


        //public IActionResult Viewer()
        //{
        //    return View();
        //}

        public IActionResult Viewer(string reportName = "rptHitRate11")
        {
            try
            {
                var rptPath = $"QCO.Reports.{reportName}";
                XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
                return View(report);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

    }
}
