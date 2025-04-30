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
    public class TaiLieuxController : Controller
    {
        private readonly DacsGplxContext _context;

        public TaiLieuxController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: TaiLieux
        public async Task<IActionResult> Index()
        {
            var dacsGplxContext = _context.TaiLieus.Include(t => t.Hang);
            return View(await dacsGplxContext.ToListAsync());
        }

        // GET: TaiLieux/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiLieu = await _context.TaiLieus
                .Include(t => t.Hang)
                .FirstOrDefaultAsync(m => m.TailieuId == id);
            if (taiLieu == null)
            {
                return NotFound();
            }

            return View(taiLieu);
        }

        // GET: TaiLieux/Create
        public IActionResult Create()
        {
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId");
            return View();
        }

        // POST: TaiLieux/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TailieuId,HangId,Tentl,Sl,Thoigiancapnhat")] TaiLieu taiLieu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(taiLieu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId", taiLieu.HangId);
            return View(taiLieu);
        }

        // GET: TaiLieux/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiLieu = await _context.TaiLieus.FindAsync(id);
            if (taiLieu == null)
            {
                return NotFound();
            }
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId", taiLieu.HangId);
            return View(taiLieu);
        }

        // POST: TaiLieux/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TailieuId,HangId,Tentl,Sl,Thoigiancapnhat")] TaiLieu taiLieu)
        {
            if (id != taiLieu.TailieuId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taiLieu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaiLieuExists(taiLieu.TailieuId))
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
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId", taiLieu.HangId);
            return View(taiLieu);
        }

        // GET: TaiLieux/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiLieu = await _context.TaiLieus
                .Include(t => t.Hang)
                .FirstOrDefaultAsync(m => m.TailieuId == id);
            if (taiLieu == null)
            {
                return NotFound();
            }

            return View(taiLieu);
        }

        // POST: TaiLieux/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taiLieu = await _context.TaiLieus.FindAsync(id);
            if (taiLieu != null)
            {
                _context.TaiLieus.Remove(taiLieu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaiLieuExists(int id)
        {
            return _context.TaiLieus.Any(e => e.TailieuId == id);
        }
    }
}
