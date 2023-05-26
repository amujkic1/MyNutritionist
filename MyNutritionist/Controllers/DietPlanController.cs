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
    public class DietPlanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DietPlanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DietPlan
        public async Task<IActionResult> Index()
        {
              return _context.DietPlan != null ? 
                          View(await _context.DietPlan.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.DietPlan'  is null.");
        }

        // GET: DietPlan/Details/5
        public async Task<IActionResult> Details(int? id)
        {
           /* if (id == null || _context.DietPlan == null)
            {
                return NotFound();
            }*/

            var dietPlan = await _context.DietPlan
                .FirstOrDefaultAsync(m => m.DPID == id);
           /* if (dietPlan == null)
            {
                return NotFound();
            }*/

            return View(dietPlan);
        }

        // GET: DietPlan/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DietPlan/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DPID,TotalCalories")] DietPlan dietPlan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dietPlan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dietPlan);
        }

        // GET: DietPlan/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
           /* if (id == null || _context.DietPlan == null)
            {
                return NotFound();
            }*/

            var dietPlan = await _context.DietPlan.FindAsync(id);
            /*if (dietPlan == null)
            {
                return NotFound();
            }*/
            return View(dietPlan);
        }

        // POST: DietPlan/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       public async Task<IActionResult> Edit(int id, [Bind("DPID,TotalCalories")] DietPlan dietPlan)
        {
           /* if (id != dietPlan.DPID)
            {
                return NotFound();
            }*/

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dietPlan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                   /* if (!DietPlanExists(dietPlan.DPID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }*/
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dietPlan);
        }

        // GET: DietPlan/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
          /* if (id == null || _context.DietPlan == null)
            {
                return NotFound();
            }*/

            var dietPlan = await _context.DietPlan
                .FirstOrDefaultAsync(m => m.DPID == id);
           /* if (dietPlan == null)
            {
                return NotFound();
            }*/

            return View(dietPlan);
        }

        // POST: DietPlan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DietPlan == null)
            {
                return Problem("Entity set 'ApplicationDbContext.DietPlan'  is null.");
            }
            var dietPlan = await _context.DietPlan.FindAsync(id);
            if (dietPlan != null)
            {
                _context.DietPlan.Remove(dietPlan);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DietPlanExists(int id)
        {
          return (_context.DietPlan?.Any(e => e.DPID == id)).GetValueOrDefault();
        }
    }
}
