using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using My_CV_3.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace My_CV_3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMongoCollection<Contact> _contactCollection;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            var mongoSettings = configuration.GetSection("MongoDbSettings");
            var client = new MongoClient(mongoSettings["ConnectionString"]);
            var database = client.GetDatabase(mongoSettings["DatabaseName"]);
            _contactCollection = database.GetCollection<Contact>(mongoSettings["ContactCollectionName"]);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult portfolio()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitContact([FromForm] Contact contact)
        {
if (!ModelState.IsValid)
{
    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
    _logger.LogError("Invalid contact form data: {Errors}", string.Join(", ", errors));
    return Json(new { success = false, message = "Invalid form data.", errors });
}
            try
            {
                contact.SubmittedAt = DateTime.UtcNow;
                await _contactCollection.InsertOneAsync(contact);
                return Json(new { success = true, message = "Thank you for your message!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Database error: " + ex.Message });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
