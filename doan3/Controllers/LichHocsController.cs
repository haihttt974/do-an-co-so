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
    public class LichHocsController : Controller
    {
        private readonly DacsGplxContext _context;

        public LichHocsController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: LichHocs
        public async Task<IActionResult> Index()
        {
            var dacsGplxContext = _context.LichHocs.Include(l => l.Lop);
            return View(await dacsGplxContext.ToListAsync());
        }

        // GET: LichHocs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichHoc = await _context.LichHocs
                .Include(l => l.Lop)
                .FirstOrDefaultAsync(m => m.LichhocId == id);
            if (lichHoc == null)
            {
                return NotFound();
            }

            return View(lichHoc);
        }

        // GET: LichHocs/Create
        public IActionResult Create()
        {
            ViewData["LopId"] = new SelectList(_context.LopHocs, "LopId", "LopId");
            return View();
        }

        // POST: LichHocs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LichhocId,LopId,TgBatdau,TgKetthuc,Hinhthuchoc,Noidung")] LichHoc lichHoc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lichHoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LopId"] = new SelectList(_context.LopHocs, "LopId", "LopId", lichHoc.LopId);
            return View(lichHoc);
        }

        // GET: LichHocs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichHoc = await _context.LichHocs.FindAsync(id);
            if (lichHoc == null)
            {
                return NotFound();
            }
            ViewData["LopId"] = new SelectList(_context.LopHocs, "LopId", "LopId", lichHoc.LopId);
            return View(lichHoc);
        }

        // POST: LichHocs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LichhocId,LopId,TgBatdau,TgKetthuc,Hinhthuchoc,Noidung")] LichHoc lichHoc)
        {
            if (id != lichHoc.LichhocId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lichHoc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LichHocExists(lichHoc.LichhocId))
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
            ViewData["LopId"] = new SelectList(_context.LopHocs, "LopId", "LopId", lichHoc.LopId);
            return View(lichHoc);
        }

        // GET: LichHocs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichHoc = await _context.LichHocs
                .Include(l => l.Lop)
                .FirstOrDefaultAsync(m => m.LichhocId == id);
            if (lichHoc == null)
            {
                return NotFound();
            }

            return View(lichHoc);
        }

        // POST: LichHocs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lichHoc = await _context.LichHocs.FindAsync(id);
            if (lichHoc != null)
            {
                _context.LichHocs.Remove(lichHoc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LichHocExists(int id)
        {
            return _context.LichHocs.Any(e => e.LichhocId == id);
        }
    }
}
