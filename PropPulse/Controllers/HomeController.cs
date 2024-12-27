using Microsoft.AspNetCore.Mvc;
using PropPulse.Data;
using PropPulse.Models;
using System.Diagnostics;

namespace PropPulse.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PropPulseContext _context;
        public HomeController(ILogger<HomeController> logger, PropPulseContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            List<Property> properties = _context.Properties.ToList();


            return View(properties);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
