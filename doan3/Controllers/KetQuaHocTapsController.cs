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
    public class KetQuaHocTapsController : Controller
    {
        private readonly DacsGplxContext _context;

        public KetQuaHocTapsController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: KetQuaHocTaps
        public async Task<IActionResult> Index()
        {
            var dacsGplxContext = _context.KetQuaHocTaps.Include(k => k.Hoso).Include(k => k.Lop);
            return View(await dacsGplxContext.ToListAsync());
        }

        //GET: KetQuaHocTaps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ketQuaHocTap = await _context.KetQuaHocTaps
                .Include(k => k.Hoso)
                .ThenInclude(h => h.Hocvien)
                .Include(k => k.Hoso.Hang)
                .Include(k => k.Lop)
                .FirstOrDefaultAsync(m => m.HosoId == id);

            if (ketQuaHocTap == null)
            {
                return NotFound();
            }

            return View(ketQuaHocTap);
        }

        // GET: KetQuaHocTaps/Create
        public IActionResult Create()
        {
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId");
            ViewData["LopId"] = new SelectList(_context.LopHocs, "LopId", "LopId");
            return View();
        }

        // POST: KetQuaHocTaps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KetquaId,LopId,HosoId,Nhanxet,Sobuoiodahoc,Sobuoitoithieu,KmHoanthanh,GioBandem,HtLythuyet,HtMophong,HtSahinh,HtDuongtruong,DauTn,DuDkThisat,Thoigiancapnhat")] KetQuaHocTap ketQuaHocTap)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ketQuaHocTap);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", ketQuaHocTap.HosoId);
            ViewData["LopId"] = new SelectList(_context.LopHocs, "LopId", "LopId", ketQuaHocTap.LopId);
            return View(ketQuaHocTap);
        }

        // GET: KetQuaHocTaps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ketQuaHocTap = await _context.KetQuaHocTaps.FindAsync(id);
            if (ketQuaHocTap == null)
            {
                return NotFound();
            }
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", ketQuaHocTap.HosoId);
            ViewData["LopId"] = new SelectList(_context.LopHocs, "LopId", "LopId", ketQuaHocTap.LopId);
            return View(ketQuaHocTap);
        }

        // POST: KetQuaHocTaps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("KetquaId,LopId,HosoId,Nhanxet,Sobuoiodahoc,Sobuoitoithieu,KmHoanthanh,GioBandem,HtLythuyet,HtMophong,HtSahinh,HtDuongtruong,DauTn,DuDkThisat,Thoigiancapnhat")] KetQuaHocTap ketQuaHocTap)
        {
            if (id != ketQuaHocTap.KetquaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ketQuaHocTap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KetQuaHocTapExists(ketQuaHocTap.KetquaId))
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
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", ketQuaHocTap.HosoId);
            ViewData["LopId"] = new SelectList(_context.LopHocs, "LopId", "LopId", ketQuaHocTap.LopId);
            return View(ketQuaHocTap);
        }

        // GET: KetQuaHocTaps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ketQuaHocTap = await _context.KetQuaHocTaps
                .Include(k => k.Hoso)
                .Include(k => k.Lop)
                .FirstOrDefaultAsync(m => m.KetquaId == id);
            if (ketQuaHocTap == null)
            {
                return NotFound();
            }

            return View(ketQuaHocTap);
        }

        // POST: KetQuaHocTaps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ketQuaHocTap = await _context.KetQuaHocTaps.FindAsync(id);
            if (ketQuaHocTap != null)
            {
                _context.KetQuaHocTaps.Remove(ketQuaHocTap);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KetQuaHocTapExists(int id)
        {
            return _context.KetQuaHocTaps.Any(e => e.KetquaId == id);
        }
    }
}
