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
    public class CtPhieuThanhToansController : Controller
    {
        private readonly DacsGplxContext _context;

        public CtPhieuThanhToansController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: CtPhieuThanhToans
        public async Task<IActionResult> Index()
        {
            var dacsGplxContext = _context.CtPhieuThanhToans.Include(c => c.Hoso).Include(c => c.Thanhtoan);
            return View(await dacsGplxContext.ToListAsync());
        }

        // GET: CtPhieuThanhToans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ctPhieuThanhToan = await _context.CtPhieuThanhToans
                .Include(c => c.Hoso)
                .Include(c => c.Thanhtoan)
                .FirstOrDefaultAsync(m => m.CtPhieuttId == id);
            if (ctPhieuThanhToan == null)
            {
                return NotFound();
            }

            return View(ctPhieuThanhToan);
        }

        // GET: CtPhieuThanhToans/Create
        public IActionResult Create()
        {
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId");
            ViewData["ThanhtoanId"] = new SelectList(_context.ThanhToans, "ThanhtoanId", "ThanhtoanId");
            return View();
        }

        // POST: CtPhieuThanhToans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CtPhieuttId,HosoId,ThanhtoanId,Thoigianthanhtoan,Loaiphi")] CtPhieuThanhToan ctPhieuThanhToan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ctPhieuThanhToan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", ctPhieuThanhToan.HosoId);
            ViewData["ThanhtoanId"] = new SelectList(_context.ThanhToans, "ThanhtoanId", "ThanhtoanId", ctPhieuThanhToan.ThanhtoanId);
            return View(ctPhieuThanhToan);
        }

        // GET: CtPhieuThanhToans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ctPhieuThanhToan = await _context.CtPhieuThanhToans.FindAsync(id);
            if (ctPhieuThanhToan == null)
            {
                return NotFound();
            }
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", ctPhieuThanhToan.HosoId);
            ViewData["ThanhtoanId"] = new SelectList(_context.ThanhToans, "ThanhtoanId", "ThanhtoanId", ctPhieuThanhToan.ThanhtoanId);
            return View(ctPhieuThanhToan);
        }

        // POST: CtPhieuThanhToans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CtPhieuttId,HosoId,ThanhtoanId,Thoigianthanhtoan,Loaiphi")] CtPhieuThanhToan ctPhieuThanhToan)
        {
            if (id != ctPhieuThanhToan.CtPhieuttId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ctPhieuThanhToan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CtPhieuThanhToanExists(ctPhieuThanhToan.CtPhieuttId))
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
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", ctPhieuThanhToan.HosoId);
            ViewData["ThanhtoanId"] = new SelectList(_context.ThanhToans, "ThanhtoanId", "ThanhtoanId", ctPhieuThanhToan.ThanhtoanId);
            return View(ctPhieuThanhToan);
        }

        // GET: CtPhieuThanhToans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ctPhieuThanhToan = await _context.CtPhieuThanhToans
                .Include(c => c.Hoso)
                .Include(c => c.Thanhtoan)
                .FirstOrDefaultAsync(m => m.CtPhieuttId == id);
            if (ctPhieuThanhToan == null)
            {
                return NotFound();
            }

            return View(ctPhieuThanhToan);
        }

        // POST: CtPhieuThanhToans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ctPhieuThanhToan = await _context.CtPhieuThanhToans.FindAsync(id);
            if (ctPhieuThanhToan != null)
            {
                _context.CtPhieuThanhToans.Remove(ctPhieuThanhToan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CtPhieuThanhToanExists(int id)
        {
            return _context.CtPhieuThanhToans.Any(e => e.CtPhieuttId == id);
        }
    }
}
