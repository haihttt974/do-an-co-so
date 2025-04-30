using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using doan3.Models;

namespace doan3.Controllers
{
    public class KyThisController : Controller
    {
        private readonly DacsGplxContext _context;

        public KyThisController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: KyThis
        public async Task<IActionResult> Index()
        {
            var dacsGplxContext = _context.KyThis.Include(k => k.Hang).Include(k => k.Loaithi);
            return View(await dacsGplxContext.ToListAsync());
        }

        // GET: KyThis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kyThi = await _context.KyThis
                .Include(k => k.Hang)
                .Include(k => k.Loaithi)
                .FirstOrDefaultAsync(m => m.KythiId == id);
            if (kyThi == null)
            {
                return NotFound();
            }

            return View(kyThi);
        }

        // GET: KyThis/Create
        public IActionResult Create()
        {
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId");
            ViewData["LoaithiId"] = new SelectList(_context.LoaiThis, "LoaithiId", "LoaithiId");
            return View();
        }

        // POST: KyThis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KythiId,Tenkythi,HangId,LoaithiId")] KyThi kyThi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kyThi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId", kyThi.HangId);
            ViewData["LoaithiId"] = new SelectList(_context.LoaiThis, "LoaithiId", "LoaithiId", kyThi.LoaithiId);
            return View(kyThi);
        }

        // GET: KyThis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kyThi = await _context.KyThis.FindAsync(id);
            if (kyThi == null)
            {
                return NotFound();
            }
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId", kyThi.HangId);
            ViewData["LoaithiId"] = new SelectList(_context.LoaiThis, "LoaithiId", "LoaithiId", kyThi.LoaithiId);
            return View(kyThi);
        }

        // POST: KyThis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("KythiId,Tenkythi,HangId,LoaithiId")] KyThi kyThi)
        {
            if (id != kyThi.KythiId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kyThi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KyThiExists(kyThi.KythiId))
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
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId", kyThi.HangId);
            ViewData["LoaithiId"] = new SelectList(_context.LoaiThis, "LoaithiId", "LoaithiId", kyThi.LoaithiId);
            return View(kyThi);
        }

        // GET: KyThis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kyThi = await _context.KyThis
                .Include(k => k.Hang)
                .Include(k => k.Loaithi)
                .FirstOrDefaultAsync(m => m.KythiId == id);
            if (kyThi == null)
            {
                return NotFound();
            }

            return View(kyThi);
        }

        // POST: KyThis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kyThi = await _context.KyThis.FindAsync(id);
            if (kyThi != null)
            {
                _context.KyThis.Remove(kyThi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KyThiExists(int id)
        {
            return _context.KyThis.Any(e => e.KythiId == id);
        }
    }
}
