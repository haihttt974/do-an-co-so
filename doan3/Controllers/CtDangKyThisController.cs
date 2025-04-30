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
    public class CtDangKyThisController : Controller
    {
        private readonly DacsGplxContext _context;

        public CtDangKyThisController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: CtDangKyThis
        public async Task<IActionResult> Index()
        {
            var dacsGplxContext = _context.CtDangKyThis.Include(c => c.Hoso).Include(c => c.Kythi).Include(c => c.Lichthi);
            return View(await dacsGplxContext.ToListAsync());
        }

        // GET: CtDangKyThis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ctDangKyThi = await _context.CtDangKyThis
                .Include(c => c.Hoso)
                .Include(c => c.Kythi)
                .Include(c => c.Lichthi)
                .FirstOrDefaultAsync(m => m.CtDktId == id);
            if (ctDangKyThi == null)
            {
                return NotFound();
            }

            return View(ctDangKyThi);
        }

        // GET: CtDangKyThis/Create
        public IActionResult Create()
        {
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId");
            ViewData["KythiId"] = new SelectList(_context.KyThis, "KythiId", "KythiId");
            ViewData["LichthiId"] = new SelectList(_context.LichThis, "LichthiId", "LichthiId");
            return View();
        }

        // POST: CtDangKyThis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CtDktId,KythiId,HosoId,Thoigiandk,Thoigianthi,LichthiId")] CtDangKyThi ctDangKyThi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ctDangKyThi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", ctDangKyThi.HosoId);
            ViewData["KythiId"] = new SelectList(_context.KyThis, "KythiId", "KythiId", ctDangKyThi.KythiId);
            ViewData["LichthiId"] = new SelectList(_context.LichThis, "LichthiId", "LichthiId", ctDangKyThi.LichthiId);
            return View(ctDangKyThi);
        }

        // GET: CtDangKyThis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ctDangKyThi = await _context.CtDangKyThis.FindAsync(id);
            if (ctDangKyThi == null)
            {
                return NotFound();
            }
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", ctDangKyThi.HosoId);
            ViewData["KythiId"] = new SelectList(_context.KyThis, "KythiId", "KythiId", ctDangKyThi.KythiId);
            ViewData["LichthiId"] = new SelectList(_context.LichThis, "LichthiId", "LichthiId", ctDangKyThi.LichthiId);
            return View(ctDangKyThi);
        }

        // POST: CtDangKyThis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CtDktId,KythiId,HosoId,Thoigiandk,Thoigianthi,LichthiId")] CtDangKyThi ctDangKyThi)
        {
            if (id != ctDangKyThi.CtDktId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ctDangKyThi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CtDangKyThiExists(ctDangKyThi.CtDktId))
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
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", ctDangKyThi.HosoId);
            ViewData["KythiId"] = new SelectList(_context.KyThis, "KythiId", "KythiId", ctDangKyThi.KythiId);
            ViewData["LichthiId"] = new SelectList(_context.LichThis, "LichthiId", "LichthiId", ctDangKyThi.LichthiId);
            return View(ctDangKyThi);
        }

        // GET: CtDangKyThis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ctDangKyThi = await _context.CtDangKyThis
                .Include(c => c.Hoso)
                .Include(c => c.Kythi)
                .Include(c => c.Lichthi)
                .FirstOrDefaultAsync(m => m.CtDktId == id);
            if (ctDangKyThi == null)
            {
                return NotFound();
            }

            return View(ctDangKyThi);
        }

        // POST: CtDangKyThis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ctDangKyThi = await _context.CtDangKyThis.FindAsync(id);
            if (ctDangKyThi != null)
            {
                _context.CtDangKyThis.Remove(ctDangKyThi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CtDangKyThiExists(int id)
        {
            return _context.CtDangKyThis.Any(e => e.CtDktId == id);
        }
    }
}
