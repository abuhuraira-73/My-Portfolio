using Microsoft.AspNetCore.Mvc;
using My_CV_3.Services;
using System.Threading.Tasks;

namespace My_CV_3.Controllers
{
    public class AdminController : Controller
    {
        private readonly VisitorService _visitorService;

        public AdminController(VisitorService visitorService)
        {
            _visitorService = visitorService;
        }

        public async Task<IActionResult> VisitorStats()
        {
            ViewBag.VisitorCount = await _visitorService.GetVisitCountAsync();
            return View();
        }
    }
}