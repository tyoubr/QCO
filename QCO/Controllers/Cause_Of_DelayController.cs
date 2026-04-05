using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QCO.Models;

namespace QCO.Controllers
{
    public class Cause_Of_DelayController : Controller
    {
        private readonly QCOContext _context;

        public Cause_Of_DelayController(QCOContext context)
        {
            _context = context;
        }

        // GET: Cause_Of_Delay
        public async Task<IActionResult> Index()
        {
            return View(await _context.tbl_Cause_Of_Delays.ToListAsync());
        }

        // GET: Cause_Of_Delay/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbl_Cause_Of_Delay = await _context.tbl_Cause_Of_Delays
                .FirstOrDefaultAsync(m => m.CAUSEID == id);
            if (tbl_Cause_Of_Delay == null)
            {
                return NotFound();
            }

            return View(tbl_Cause_Of_Delay);
        }

        // GET: Cause_Of_Delay/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cause_Of_Delay/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CAUSEID,CAUSE_OF_DELAY,REMARKS")] tbl_Cause_Of_Delay tbl_Cause_Of_Delay)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbl_Cause_Of_Delay);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tbl_Cause_Of_Delay);
        }

        // GET: Cause_Of_Delay/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbl_Cause_Of_Delay = await _context.tbl_Cause_Of_Delays.FindAsync(id);
            if (tbl_Cause_Of_Delay == null)
            {
                return NotFound();
            }
            return View(tbl_Cause_Of_Delay);
        }

        // POST: Cause_Of_Delay/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CAUSEID,CAUSE_OF_DELAY,REMARKS")] tbl_Cause_Of_Delay tbl_Cause_Of_Delay)
        {
            if (id != tbl_Cause_Of_Delay.CAUSEID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbl_Cause_Of_Delay);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!tbl_Cause_Of_DelayExists(tbl_Cause_Of_Delay.CAUSEID))
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
            return View(tbl_Cause_Of_Delay);
        }

        // GET: Cause_Of_Delay/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbl_Cause_Of_Delay = await _context.tbl_Cause_Of_Delays
                .FirstOrDefaultAsync(m => m.CAUSEID == id);
            if (tbl_Cause_Of_Delay == null)
            {
                return NotFound();
            }

            return View(tbl_Cause_Of_Delay);
        }

        // POST: Cause_Of_Delay/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbl_Cause_Of_Delay = await _context.tbl_Cause_Of_Delays.FindAsync(id);
            if (tbl_Cause_Of_Delay != null)
            {
                _context.tbl_Cause_Of_Delays.Remove(tbl_Cause_Of_Delay);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool tbl_Cause_Of_DelayExists(int id)
        {
            return _context.tbl_Cause_Of_Delays.Any(e => e.CAUSEID == id);
        }
    }
}
