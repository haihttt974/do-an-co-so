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
    public class QuyDinhHangGplxesController : Controller
    {
        private readonly DacsGplxContext _context;

        public QuyDinhHangGplxesController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: QuyDinhHangGplxes
        public async Task<IActionResult> Index()
        {
            var dacsGplxContext = _context.QuyDinhHangGplxes.Include(q => q.Hang);
            return View(await dacsGplxContext.ToListAsync());
        }

        // GET: QuyDinhHangGplxes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quyDinhHangGplx = await _context.QuyDinhHangGplxes
                .Include(q => q.Hang)
                .FirstOrDefaultAsync(m => m.QuydinhId == id);
            if (quyDinhHangGplx == null)
            {
                return NotFound();
            }

            return View(quyDinhHangGplx);
        }

        // GET: QuyDinhHangGplxes/Create
        public IActionResult Create()
        {
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId");
            return View();
        }

        // POST: QuyDinhHangGplxes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuydinhId,HangId,KmToithieu,SogioBandem,LyThuyet,MoPhong,SaHinh,DuongTruong,Ghichu")] QuyDinhHangGplx quyDinhHangGplx)
        {
            if (ModelState.IsValid)
            {
                _context.Add(quyDinhHangGplx);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId", quyDinhHangGplx.HangId);
            return View(quyDinhHangGplx);
        }

        // GET: QuyDinhHangGplxes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quyDinhHangGplx = await _context.QuyDinhHangGplxes.FindAsync(id);
            if (quyDinhHangGplx == null)
            {
                return NotFound();
            }
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId", quyDinhHangGplx.HangId);
            return View(quyDinhHangGplx);
        }

        // POST: QuyDinhHangGplxes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("QuydinhId,HangId,KmToithieu,SogioBandem,LyThuyet,MoPhong,SaHinh,DuongTruong,Ghichu")] QuyDinhHangGplx quyDinhHangGplx)
        {
            if (id != quyDinhHangGplx.QuydinhId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quyDinhHangGplx);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuyDinhHangGplxExists(quyDinhHangGplx.QuydinhId))
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
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId", quyDinhHangGplx.HangId);
            return View(quyDinhHangGplx);
        }

        // GET: QuyDinhHangGplxes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quyDinhHangGplx = await _context.QuyDinhHangGplxes
                .Include(q => q.Hang)
                .FirstOrDefaultAsync(m => m.QuydinhId == id);
            if (quyDinhHangGplx == null)
            {
                return NotFound();
            }

            return View(quyDinhHangGplx);
        }

        // POST: QuyDinhHangGplxes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quyDinhHangGplx = await _context.QuyDinhHangGplxes.FindAsync(id);
            if (quyDinhHangGplx != null)
            {
                _context.QuyDinhHangGplxes.Remove(quyDinhHangGplx);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuyDinhHangGplxExists(int id)
        {
            return _context.QuyDinhHangGplxes.Any(e => e.QuydinhId == id);
        }
    }
}
