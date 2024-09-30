using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Travel.Models;

namespace Travel.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger,
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        //[Route("")]
        //[Route("Home")]
        //[Route("Home/Index")]
        //[Route("Home/Index/{country}")]
        //[Route("Home/Index/{country}/{city}")]
        public IActionResult Index(string? country, string? city)
        {
            IEnumerable<Place> places = new List<Place>();
            if (country != null && country != "")
            {
                if (city != null && city != "")
                {
                    places = _context.Places
                        .Where(p => p.ApplicationUserId != null 
                        && p.IsConfirmed == true && p.Country != null 
                        && p.Country.Contains(country) && p.City != null && 
                        p.City.Contains(city))
                        .Include(p => p.ApplicationUser)
                        .OrderBy(p => p.Country).ThenBy(p => p.City);
                }
                else
                {
                    places = _context.Places
                        .Where(p => p.ApplicationUserId != null
                        && p.IsConfirmed == true && p.Country != null
                        && p.Country.Contains(country))
                        .Include(p => p.ApplicationUser)
                        .OrderBy(p => p.Country).ThenBy(p => p.City);
                }
            }
            else
            {
                if (city != null && city != "")
                {
                    places = _context.Places
                        .Where(p => p.ApplicationUserId != null
                        && p.IsConfirmed == true && p.City != null &&
                        p.City.Contains(city))
                        .Include(p => p.ApplicationUser)
                        .OrderBy(p => p.Country).ThenBy(p => p.City);
                }
                else
                {
                    places = _context.Places
                .Where(p => p.ApplicationUser != null && p.IsConfirmed == true)
                .Include(p => p.ApplicationUser)
                .OrderBy(p => p.Country).ThenBy(p => p.City);
                }
            }
            return View(places.ToList());
        }

        // GET: Home/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var place = await _context.Places
                .Include(p => p.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (place == null)
            {
                return NotFound();
            }

            return View(place);
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
