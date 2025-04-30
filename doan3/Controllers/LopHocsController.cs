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
    public class LopHocsController : Controller
    {
        private readonly DacsGplxContext _context;

        public LopHocsController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: LopHocs
        public async Task<IActionResult> Index()
        {
            var dacsGplxContext = _context.LopHocs.Include(l => l.Giaovien).Include(l => l.Khoahoc);
            return View(await dacsGplxContext.ToListAsync());
        }

        // GET: LopHocs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lopHoc = await _context.LopHocs
                .Include(l => l.Giaovien)
                .Include(l => l.Khoahoc)
                .FirstOrDefaultAsync(m => m.LopId == id);
            if (lopHoc == null)
            {
                return NotFound();
            }

            return View(lopHoc);
        }

        // GET: LopHocs/Create
        public IActionResult Create()
        {
            ViewData["GiaovienId"] = new SelectList(_context.GiaoViens, "GiaovienId", "GiaovienId");
            ViewData["KhoahocId"] = new SelectList(_context.KhoaHocs, "KhoahocId", "KhoahocId");
            return View();
        }

        // POST: LopHocs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LopId,Tenlop,LoaiLop,KhoahocId,GiaovienId")] LopHoc lopHoc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lopHoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GiaovienId"] = new SelectList(_context.GiaoViens, "GiaovienId", "GiaovienId", lopHoc.GiaovienId);
            ViewData["KhoahocId"] = new SelectList(_context.KhoaHocs, "KhoahocId", "KhoahocId", lopHoc.KhoahocId);
            return View(lopHoc);
        }

        // GET: LopHocs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lopHoc = await _context.LopHocs.FindAsync(id);
            if (lopHoc == null)
            {
                return NotFound();
            }
            ViewData["GiaovienId"] = new SelectList(_context.GiaoViens, "GiaovienId", "GiaovienId", lopHoc.GiaovienId);
            ViewData["KhoahocId"] = new SelectList(_context.KhoaHocs, "KhoahocId", "KhoahocId", lopHoc.KhoahocId);
            return View(lopHoc);
        }

        // POST: LopHocs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LopId,Tenlop,LoaiLop,KhoahocId,GiaovienId")] LopHoc lopHoc)
        {
            if (id != lopHoc.LopId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lopHoc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LopHocExists(lopHoc.LopId))
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
            ViewData["GiaovienId"] = new SelectList(_context.GiaoViens, "GiaovienId", "GiaovienId", lopHoc.GiaovienId);
            ViewData["KhoahocId"] = new SelectList(_context.KhoaHocs, "KhoahocId", "KhoahocId", lopHoc.KhoahocId);
            return View(lopHoc);
        }

        // GET: LopHocs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lopHoc = await _context.LopHocs
                .Include(l => l.Giaovien)
                .Include(l => l.Khoahoc)
                .FirstOrDefaultAsync(m => m.LopId == id);
            if (lopHoc == null)
            {
                return NotFound();
            }

            return View(lopHoc);
        }

        // POST: LopHocs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lopHoc = await _context.LopHocs.FindAsync(id);
            if (lopHoc != null)
            {
                _context.LopHocs.Remove(lopHoc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LopHocExists(int id)
        {
            return _context.LopHocs.Any(e => e.LopId == id);
        }
    }
}
