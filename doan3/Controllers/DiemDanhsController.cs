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
    public class DiemDanhsController : Controller
    {
        private readonly DacsGplxContext _context;

        public DiemDanhsController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: DiemDanhs
        public async Task<IActionResult> Index()
        {
            var dacsGplxContext = _context.DiemDanhs.Include(d => d.Hoso).Include(d => d.Lichhoc);
            return View(await dacsGplxContext.ToListAsync());
        }

        // GET: DiemDanhs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diemDanh = await _context.DiemDanhs
                .Include(d => d.Hoso)
                .Include(d => d.Lichhoc)
                .FirstOrDefaultAsync(m => m.DiemdanhId == id);
            if (diemDanh == null)
            {
                return NotFound();
            }

            return View(diemDanh);
        }

        // GET: DiemDanhs/Create
        public IActionResult Create()
        {
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId");
            ViewData["LichhocId"] = new SelectList(_context.LichHocs, "LichhocId", "LichhocId");
            return View();
        }

        // POST: DiemDanhs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DiemdanhId,Ngayhoc,Trangthai,HosoId,LichhocId")] DiemDanh diemDanh)
        {
            if (ModelState.IsValid)
            {
                _context.Add(diemDanh);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", diemDanh.HosoId);
            ViewData["LichhocId"] = new SelectList(_context.LichHocs, "LichhocId", "LichhocId", diemDanh.LichhocId);
            return View(diemDanh);
        }

        // GET: DiemDanhs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diemDanh = await _context.DiemDanhs.FindAsync(id);
            if (diemDanh == null)
            {
                return NotFound();
            }
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", diemDanh.HosoId);
            ViewData["LichhocId"] = new SelectList(_context.LichHocs, "LichhocId", "LichhocId", diemDanh.LichhocId);
            return View(diemDanh);
        }

        // POST: DiemDanhs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DiemdanhId,Ngayhoc,Trangthai,HosoId,LichhocId")] DiemDanh diemDanh)
        {
            if (id != diemDanh.DiemdanhId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diemDanh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiemDanhExists(diemDanh.DiemdanhId))
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
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", diemDanh.HosoId);
            ViewData["LichhocId"] = new SelectList(_context.LichHocs, "LichhocId", "LichhocId", diemDanh.LichhocId);
            return View(diemDanh);
        }

        // GET: DiemDanhs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diemDanh = await _context.DiemDanhs
                .Include(d => d.Hoso)
                .Include(d => d.Lichhoc)
                .FirstOrDefaultAsync(m => m.DiemdanhId == id);
            if (diemDanh == null)
            {
                return NotFound();
            }

            return View(diemDanh);
        }

        // POST: DiemDanhs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diemDanh = await _context.DiemDanhs.FindAsync(id);
            if (diemDanh != null)
            {
                _context.DiemDanhs.Remove(diemDanh);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiemDanhExists(int id)
        {
            return _context.DiemDanhs.Any(e => e.DiemdanhId == id);
        }
    }
}
