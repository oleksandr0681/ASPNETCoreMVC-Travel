using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Travel.Models;

namespace Travel.Controllers
{
    [Authorize]
    public class MarksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MarksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Marks
        public async Task<IActionResult> Index()
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            var applicationDbContext = _context.Marks
                .Where(m => m.ApplicationUserId == currentUser.Id 
                && m.ApplicationUser != null && m.Place != null)
                .Include(m => m.ApplicationUser).Include(m => m.Place);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Marks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mark = await _context.Marks
                .Include(m => m.ApplicationUser)
                .Include(m => m.Place)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mark == null)
            {
                return NotFound();
            }

            return View(mark);
        }

        // GET: Marks/Create
        //public IActionResult Create()
        //{
        //    ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
        //    ViewData["PlaceId"] = new SelectList(_context.Places, "Id", "ApplicationUserId");
        //    return View();
        //}

        // POST: Marks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,Point,Commentary,PlaceId,Created")] Mark mark)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(mark);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", mark.ApplicationUserId);
        //    ViewData["PlaceId"] = new SelectList(_context.Places, "Id", "ApplicationUserId", mark.PlaceId);
        //    return View(mark);
        //}

        // GET: Marks/Mark/5
        [HttpGet]
        //[Route("Marks/Mark/{placeId?}")]
        public async Task<IActionResult> Mark()
        //public IActionResult Mark()
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            int? placeId = HttpContext.Session.GetInt32("PlaceId");
            if (placeId == null)
            {
                ViewData["ErrorMessage"] = "Помилка передачі ідентифікатора.";
                return View("~/Views/Shared/HandleError.cshtml");
            }
            Place? place = await _context.Places.Where(p => p.Id == placeId).FirstOrDefaultAsync();
            if (place == null)
            {
                ViewData["ErrorMessage"] = "Місце не знайдене.";
                return View("~/Views/Shared/HandleError.cshtml");
            }
            ViewData["Place"] = place;
            ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == currentUser.Id), "Id", "UserName");
            ViewData["PlaceId"] = new SelectList(_context.Places.Where(p => p.Id == placeId), "Id", "Name");
            return View();
        }

        // POST: Marks/Mark
        [HttpPost]
        public async Task<IActionResult> Mark(Mark mark)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
                if (currentUser == null)
                {
                    return Unauthorized();
                }
                Place? place = await _context.Places.Where(p => p.Id == mark.PlaceId).FirstOrDefaultAsync();
                if (place == null)
                {
                    ViewData["ErrorMessage"] = "Місце не знайдене.";
                    return View("~/Views/Shared/HandleError.cshtml");
                }
                if (place.ApplicationUserId == currentUser.Id)
                {
                    ModelState.AddModelError("", "Оцінювати можна тільки місця інших користувачів.");
                    ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == mark.ApplicationUserId), "Id", "UserName", mark.ApplicationUserId);
                    ViewData["PlaceId"] = new SelectList(_context.Places.Where(p => p.Id == mark.PlaceId), "Id", "Name", mark.PlaceId);
                    return View(mark);
                }
                int markQuantity = _context.Marks
                    .Where(m => m.ApplicationUserId == currentUser.Id 
                    && m.PlaceId == place.Id).Count();
                if (markQuantity > 0)
                {
                    ModelState.AddModelError("", "Ви вже оцінювали це місце.");
                    ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == mark.ApplicationUserId), "Id", "UserName", mark.ApplicationUserId);
                    ViewData["PlaceId"] = new SelectList(_context.Places.Where(p => p.Id == mark.PlaceId), "Id", "Name", mark.PlaceId);
                    return View(mark);
                }
                mark.Created = DateTime.Now;
                _context.Add(mark);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "Home");
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == mark.ApplicationUserId), "Id", "UserName", mark.ApplicationUserId);
            ViewData["PlaceId"] = new SelectList(_context.Places.Where(p => p.Id == mark.PlaceId), "Id", "Name", mark.PlaceId);
            return View(mark);
        }

        // GET: Marks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mark = await _context.Marks.FindAsync(id);
            if (mark == null)
            {
                return NotFound();
            }
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            else if (currentUser.Id != mark.ApplicationUserId)
            {
                return View("AccessDenied");
            }
            Place? place = await _context.Places.Where(p => p.Id == mark.PlaceId).FirstOrDefaultAsync();
            if (place == null)
            {
                ViewData["ErrorMessage"] = "Місце не знайдене.";
                return View("~/Views/Shared/HandleError.cshtml");
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == currentUser.Id), "Id", "UserName", mark.ApplicationUserId);
            ViewData["PlaceId"] = new SelectList(_context.Places.Where(p => p.Id == place.Id), "Id", "Name", mark.PlaceId);
            return View(mark);
        }

        // POST: Marks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,Point,Commentary,PlaceId,Created")] Mark mark)
        {
            if (id != mark.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mark);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarkExists(mark.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == mark.ApplicationUserId), "Id", "Id", mark.ApplicationUserId);
            ViewData["PlaceId"] = new SelectList(_context.Places.Where(p => p.Id == mark.PlaceId), "Id", "ApplicationUserId", mark.PlaceId);
            return View(mark);
        }

        // GET: Marks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mark = await _context.Marks
                .Include(m => m.ApplicationUser)
                .Include(m => m.Place)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mark == null)
            {
                return NotFound();
            }
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            if (currentUser.Id != mark.ApplicationUserId)
            {
                return View("AccessDenied");
            }

            return View(mark);
        }

        // POST: Marks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mark = await _context.Marks.FindAsync(id);
            if (mark != null)
            {
                ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
                if (currentUser == null)
                {
                    return Unauthorized();
                }
                if (currentUser.Id != mark.ApplicationUserId)
                {
                    return View("AccessDenied");
                }
                _context.Marks.Remove(mark);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MarkExists(int id)
        {
            return _context.Marks.Any(e => e.Id == id);
        }
    }
}
