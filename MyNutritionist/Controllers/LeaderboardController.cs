using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyNutritionist.Data;
using MyNutritionist.Models;

namespace MyNutritionist.Controllers
{
    public class LeaderboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaderboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Leaderboard
        public async Task<IActionResult> Index()
        {
              return _context.Leaderboard != null ? 
                          View(await _context.Leaderboard.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Leaderboard'  is null.");
        }

        // GET: Leaderboard/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Leaderboard == null)
            {
                return NotFound();
            }

            var leaderboard = await _context.Leaderboard
                .FirstOrDefaultAsync(m => m.LID == id);
            if (leaderboard == null)
            {
                return NotFound();
            }

            return View(leaderboard);
        }

        // GET: Leaderboard/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Leaderboard/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LID")] Leaderboard leaderboard)
        {
            if (ModelState.IsValid)
            {
                _context.Add(leaderboard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(leaderboard);
        }

        // GET: Leaderboard/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Leaderboard == null)
            {
                return NotFound();
            }

            var leaderboard = await _context.Leaderboard.FindAsync(id);
            if (leaderboard == null)
            {
                return NotFound();
            }
            return View(leaderboard);
        }

        // POST: Leaderboard/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LID")] Leaderboard leaderboard)
        {
            if (id != leaderboard.LID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leaderboard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaderboardExists(leaderboard.LID))
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
            return View(leaderboard);
        }

        // GET: Leaderboard/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Leaderboard == null)
            {
                return NotFound();
            }

            var leaderboard = await _context.Leaderboard
                .FirstOrDefaultAsync(m => m.LID == id);
            if (leaderboard == null)
            {
                return NotFound();
            }

            return View(leaderboard);
        }

        // POST: Leaderboard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Leaderboard == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Leaderboard'  is null.");
            }
            var leaderboard = await _context.Leaderboard.FindAsync(id);
            if (leaderboard != null)
            {
                _context.Leaderboard.Remove(leaderboard);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeaderboardExists(int id)
        {
          return (_context.Leaderboard?.Any(e => e.LID == id)).GetValueOrDefault();
        }
    }
}
