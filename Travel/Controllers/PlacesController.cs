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
//using Microsoft.AspNetCore.Http;

namespace Travel.Controllers
{
    [Authorize]
    public class PlacesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly int _maxPlacesQuantity;

        public PlacesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _maxPlacesQuantity = 5000;
        }

        // GET: Places
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Places.Include(p => p.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Places/IndexModerator
        [Authorize(Roles = "Moderator")]
        public IActionResult IndexModerator()
        {
            IEnumerable<Place> places = _context.Places
                .Where(p => p.ApplicationUser != null && p.IsConfirmed == false)
                .Include(p => p.ApplicationUser);
            return View(places.ToList());
        }

        // GET: Places/Details/5
        [AllowAnonymous]
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

        // GET: Places/Create
        public async Task<IActionResult> CreateAsync()
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            //ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == currentUser.Id), "Id", "Id");
            return View();
        }

        // POST: Places/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,Data,PictureMimeType,FileName,Name,Country,City,Address,Description,IsConfirmed,Created")] Place place, 
            IFormFile? formFile = null)
        {
            if (ModelState.IsValid)
            {
                //ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
                //if (currentUser == null)
                //{
                //    return Unauthorized();
                //}
                int placesQuantity = _context.Places.Count();
                if (placesQuantity >= _maxPlacesQuantity)
                {
                    ModelState.AddModelError("", "Кількість місць перевищує " + _maxPlacesQuantity + ".");
                    ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == place.ApplicationUserId), "Id", "Id", place.ApplicationUserId);
                    return View(place);
                }
                if (formFile != null)
                {
                    // Якщо файл не є фотографією.
                    if (formFile.ContentType.ToLower().StartsWith("image/") == false)
                    {
                        ModelState.AddModelError("", "Файл не є фотографією.");
                        ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == place.ApplicationUserId), "Id", "Id", place.ApplicationUserId);
                        return View(place);
                    }
                    byte[]? photographData = null;
                    // Зчитування переданого файла в масив байтів.
                    using (BinaryReader binaryReader = new BinaryReader(formFile.OpenReadStream()))
                    {
                        photographData = binaryReader.ReadBytes((int)formFile.Length);
                    }
                    // Встановлення масива байтів.
                    place.Data = photographData;
                    // Встановлення MimeType.
                    place.PictureMimeType = formFile.ContentType;
                    // Встановлення імені файла.
                    place.FileName = formFile.FileName;
                }
                place.Created = DateTime.Now;
                _context.Add(place);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", place.ApplicationUserId);
            ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == place.ApplicationUserId), "Id", "Id", place.ApplicationUserId);
            return View(place);
        }

        // GET: Places/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var place = await _context.Places.FindAsync(id);
            if (place == null)
            {
                return NotFound();
            }
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            else if (currentUser.Id != place.ApplicationUserId)
            {
                return View("AccessDenied");
            }
            //ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", place.ApplicationUserId);
            ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == place.ApplicationUserId), "Id", "Id", place.ApplicationUserId);
            return View(place);
        }

        // POST: Places/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,Data,PictureMimeType,FileName,Name,Country,City,Address,Description,IsConfirmed,Created")] Place place, 
            IFormFile? formFile = null)
        {
            if (id != place.Id)
            {
                return NotFound();
            }
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            else if (currentUser.Id != place.ApplicationUserId)
            {
                return View("AccessDenied");
            }

            if (ModelState.IsValid)
            {
                if (formFile != null)
                {
                    // Якщо файл не є фотографією.
                    if (formFile.ContentType.ToLower().StartsWith("image/") == false)
                    {
                        ModelState.AddModelError("", "Файл не є фотографією.");
                        ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == place.ApplicationUserId), "Id", "Id", place.ApplicationUserId);
                        return View(place);
                    }
                    byte[]? photographData = null;
                    // Зчитування переданого файла в масив байтів.
                    using (BinaryReader binaryReader = new BinaryReader(formFile.OpenReadStream()))
                    {
                        photographData = binaryReader.ReadBytes((int)formFile.Length);
                    }
                    // Встановлення масива байтів.
                    place.Data = photographData;
                    // Встановлення MimeType.
                    place.PictureMimeType = formFile.ContentType;
                    // Встановлення імені файла.
                    place.FileName = formFile.FileName;
                }
                try
                {
                    _context.Update(place);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaceExists(place.Id))
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
            //ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", place.ApplicationUserId);
            ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == place.ApplicationUserId), "Id", "Id", place.ApplicationUserId);
            return View(place);
        }

        // GET: Places/EditModerator/5
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> EditModerator(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Place? place = await _context.Places.FindAsync(id);
            if (place == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == place.ApplicationUserId), "Id", "Id", place.ApplicationUserId);
            return View(place);
        }

        // POST: Places/EditModerator/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> EditModerator(int id, [Bind("Id,ApplicationUserId,Data,PictureMimeType,FileName,Name,Country,City,Address,Description,IsConfirmed,Created")] Place place,
            IFormFile? formFile = null)
        {
            if (id != place.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (formFile != null)
                {
                    // Якщо файл не є фотографією.
                    if (formFile.ContentType.ToLower().StartsWith("image/") == false)
                    {
                        ModelState.AddModelError("", "Файл не є фотографією.");
                        ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == place.ApplicationUserId), "Id", "Id", place.ApplicationUserId);
                        return View(place);
                    }
                    byte[]? photographData = null;
                    // Зчитування переданого файла в масив байтів.
                    using (BinaryReader binaryReader = new BinaryReader(formFile.OpenReadStream()))
                    {
                        photographData = binaryReader.ReadBytes((int)formFile.Length);
                    }
                    // Встановлення масива байтів.
                    place.Data = photographData;
                    // Встановлення MimeType.
                    place.PictureMimeType = formFile.ContentType;
                    // Встановлення імені файла.
                    place.FileName = formFile.FileName;
                }
                try
                {
                    _context.Update(place);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaceExists(place.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(IndexModerator));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == place.ApplicationUserId), "Id", "Id", place.ApplicationUserId);
            return View(place);
        }

        // GET: Places/Delete/5
        public async Task<IActionResult> Delete(int? id)
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
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            IList<string> userRoles = new List<string>();
            userRoles = await _userManager.GetRolesAsync(currentUser);
            if (currentUser.Id != place.ApplicationUserId && 
                userRoles.Contains("Moderator") == false)
            {
                return View("AccessDenied");
            }

            return View(place);
        }

        // POST: Places/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            IList<string> userRoles = new List<string>();
            var place = await _context.Places.FindAsync(id);
            if (place != null)
            {
                ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
                if (currentUser == null)
                {
                    return Unauthorized();
                }
                userRoles = await _userManager.GetRolesAsync(currentUser);
                if (currentUser.Id != place.ApplicationUserId && 
                    userRoles.Contains("Moderator") == false)
                {
                    return View("AccessDenied");
                }
                _context.Places.Remove(place);
            }

            await _context.SaveChangesAsync();
            if (userRoles.Contains("Moderator"))
            {
                return RedirectToAction(nameof(IndexModerator));
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PlaceExists(int id)
        {
            return _context.Places.Any(e => e.Id == id);
        }
    }
}
