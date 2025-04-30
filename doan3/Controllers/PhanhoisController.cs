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
    public class PhanhoisController : Controller
    {
        private readonly DacsGplxContext _context;

        public PhanhoisController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: Phanhois
        public async Task<IActionResult> Index()
        {
            var dacsGplxContext = _context.Phanhois.Include(p => p.Hocvien);
            return View(await dacsGplxContext.ToListAsync());
        }

        // GET: Phanhois/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phanhoi = await _context.Phanhois
                .Include(p => p.Hocvien)
                .FirstOrDefaultAsync(m => m.PhanhoiId == id);
            if (phanhoi == null)
            {
                return NotFound();
            }

            return View(phanhoi);
        }

        // GET: Phanhois/Create
        public IActionResult Create()
        {
            ViewData["HocvienId"] = new SelectList(_context.HocViens, "HocvienId", "HocvienId");
            return View();
        }

        // POST: Phanhois/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PhanhoiId,Noidung,Thoigianph,HocvienId")] Phanhoi phanhoi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(phanhoi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HocvienId"] = new SelectList(_context.HocViens, "HocvienId", "HocvienId", phanhoi.HocvienId);
            return View(phanhoi);
        }

        // GET: Phanhois/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phanhoi = await _context.Phanhois.FindAsync(id);
            if (phanhoi == null)
            {
                return NotFound();
            }
            ViewData["HocvienId"] = new SelectList(_context.HocViens, "HocvienId", "HocvienId", phanhoi.HocvienId);
            return View(phanhoi);
        }

        // POST: Phanhois/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PhanhoiId,Noidung,Thoigianph,HocvienId")] Phanhoi phanhoi)
        {
            if (id != phanhoi.PhanhoiId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phanhoi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhanhoiExists(phanhoi.PhanhoiId))
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
            ViewData["HocvienId"] = new SelectList(_context.HocViens, "HocvienId", "HocvienId", phanhoi.HocvienId);
            return View(phanhoi);
        }

        // GET: Phanhois/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phanhoi = await _context.Phanhois
                .Include(p => p.Hocvien)
                .FirstOrDefaultAsync(m => m.PhanhoiId == id);
            if (phanhoi == null)
            {
                return NotFound();
            }

            return View(phanhoi);
        }

        // POST: Phanhois/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phanhoi = await _context.Phanhois.FindAsync(id);
            if (phanhoi != null)
            {
                _context.Phanhois.Remove(phanhoi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhanhoiExists(int id)
        {
            return _context.Phanhois.Any(e => e.PhanhoiId == id);
        }
    }
}
