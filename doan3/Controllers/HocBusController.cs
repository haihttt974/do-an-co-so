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
    public class HocBusController : Controller
    {
        private readonly DacsGplxContext _context;

        public HocBusController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: HocBus
        public async Task<IActionResult> Index()
        {
            var dacsGplxContext = _context.HocBus.Include(h => h.Hoso).Include(h => h.Lichhoc);
            return View(await dacsGplxContext.ToListAsync());
        }

        // GET: HocBus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocBu = await _context.HocBus
                .Include(h => h.Hoso)
                .Include(h => h.Lichhoc)
                .FirstOrDefaultAsync(m => m.HocbuId == id);
            if (hocBu == null)
            {
                return NotFound();
            }

            return View(hocBu);
        }

        // GET: HocBus/Create
        public IActionResult Create()
        {
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId");
            ViewData["LichhocId"] = new SelectList(_context.LichHocs, "LichhocId", "LichhocId");
            return View();
        }

        // POST: HocBus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HocbuId,Ngayhoc,Lephi,HosoId,LichhocId")] HocBu hocBu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hocBu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", hocBu.HosoId);
            ViewData["LichhocId"] = new SelectList(_context.LichHocs, "LichhocId", "LichhocId", hocBu.LichhocId);
            return View(hocBu);
        }

        // GET: HocBus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocBu = await _context.HocBus.FindAsync(id);
            if (hocBu == null)
            {
                return NotFound();
            }
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", hocBu.HosoId);
            ViewData["LichhocId"] = new SelectList(_context.LichHocs, "LichhocId", "LichhocId", hocBu.LichhocId);
            return View(hocBu);
        }

        // POST: HocBus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HocbuId,Ngayhoc,Lephi,HosoId,LichhocId")] HocBu hocBu)
        {
            if (id != hocBu.HocbuId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hocBu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HocBuExists(hocBu.HocbuId))
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
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", hocBu.HosoId);
            ViewData["LichhocId"] = new SelectList(_context.LichHocs, "LichhocId", "LichhocId", hocBu.LichhocId);
            return View(hocBu);
        }

        // GET: HocBus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocBu = await _context.HocBus
                .Include(h => h.Hoso)
                .Include(h => h.Lichhoc)
                .FirstOrDefaultAsync(m => m.HocbuId == id);
            if (hocBu == null)
            {
                return NotFound();
            }

            return View(hocBu);
        }

        // POST: HocBus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hocBu = await _context.HocBus.FindAsync(id);
            if (hocBu != null)
            {
                _context.HocBus.Remove(hocBu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HocBuExists(int id)
        {
            return _context.HocBus.Any(e => e.HocbuId == id);
        }
    }
}
