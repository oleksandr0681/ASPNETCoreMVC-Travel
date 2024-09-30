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
    public class SelectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SelectsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Selects
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Selects.Include(s => s.ApplicationUser).Include(s => s.Place);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Selects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @select = await _context.Selects
                .Include(s => s.ApplicationUser)
                .Include(s => s.Place)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@select == null)
            {
                return NotFound();
            }

            return View(@select);
        }

        // GET: Selects/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["PlaceId"] = new SelectList(_context.Places, "Id", "Id");
            return View();
        }

        // POST: Selects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,PlaceId")] Select @select)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@select);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", @select.ApplicationUserId);
            ViewData["PlaceId"] = new SelectList(_context.Places, "Id", "Id", @select.PlaceId);
            return View(@select);
        }

        // GET: Selects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @select = await _context.Selects.FindAsync(id);
            if (@select == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", @select.ApplicationUserId);
            ViewData["PlaceId"] = new SelectList(_context.Places, "Id", "Id", @select.PlaceId);
            return View(@select);
        }

        // POST: Selects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,PlaceId")] Select @select)
        {
            if (id != @select.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@select);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SelectExists(@select.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", @select.ApplicationUserId);
            ViewData["PlaceId"] = new SelectList(_context.Places, "Id", "Id", @select.PlaceId);
            return View(@select);
        }

        // GET: Selects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @select = await _context.Selects
                .Include(s => s.ApplicationUser)
                .Include(s => s.Place)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@select == null)
            {
                return NotFound();
            }

            return View(@select);
        }

        // POST: Selects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @select = await _context.Selects.FindAsync(id);
            if (@select != null)
            {
                _context.Selects.Remove(@select);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SelectExists(int id)
        {
            return _context.Selects.Any(e => e.Id == id);
        }
    }
}
