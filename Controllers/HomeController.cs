using IdentityServerProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace IdentityServerProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _logger = logger;
            _userManager = userManager;
            _configuration = configuration;

        }

        public IActionResult Index()
        {
            var companyName = _configuration.GetSection("CompanyInfo:CompanyName").Value;
            ViewBag.CompanyName = companyName;
            return View();
        }

        [Authorize(Roles = "admin")]
        public IActionResult AdminDashboard()
        {
            return View(); // Admin dashboard view
        }

        public async Task<IActionResult> UserDashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new UserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            return View(model); // User dashboard view with model
        }

        public IActionResult ManagerDashboard()
        {
            return View(); // Manager dashboard view
        }

        public IActionResult Privacy()
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

