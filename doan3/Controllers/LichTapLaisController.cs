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
    public class LichTapLaisController : Controller
    {
        private readonly DacsGplxContext _context;

        public LichTapLaisController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: LichTapLais
        public async Task<IActionResult> Index()
        {
            var dacsGplxContext = _context.LichTapLais.Include(l => l.Giaovien).Include(l => l.Hoso).Include(l => l.Xe);
            return View(await dacsGplxContext.ToListAsync());
        }

        // GET: LichTapLais/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichTapLai = await _context.LichTapLais
                .Include(l => l.Giaovien)
                .Include(l => l.Hoso)
                .Include(l => l.Xe)
                .FirstOrDefaultAsync(m => m.LichId == id);
            if (lichTapLai == null)
            {
                return NotFound();
            }

            return View(lichTapLai);
        }

        // GET: LichTapLais/Create
        public IActionResult Create()
        {
            ViewData["GiaovienId"] = new SelectList(_context.GiaoViens, "GiaovienId", "GiaovienId");
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId");
            ViewData["XeId"] = new SelectList(_context.XeTapLais, "XeId", "XeId");
            return View();
        }

        // POST: LichTapLais/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LichId,GiaovienId,HosoId,XeId,Tgbatdau,Tgketthuc,Diadiem,Noidung,Ghichu")] LichTapLai lichTapLai)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lichTapLai);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GiaovienId"] = new SelectList(_context.GiaoViens, "GiaovienId", "GiaovienId", lichTapLai.GiaovienId);
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", lichTapLai.HosoId);
            ViewData["XeId"] = new SelectList(_context.XeTapLais, "XeId", "XeId", lichTapLai.XeId);
            return View(lichTapLai);
        }

        // GET: LichTapLais/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichTapLai = await _context.LichTapLais.FindAsync(id);
            if (lichTapLai == null)
            {
                return NotFound();
            }
            ViewData["GiaovienId"] = new SelectList(_context.GiaoViens, "GiaovienId", "GiaovienId", lichTapLai.GiaovienId);
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", lichTapLai.HosoId);
            ViewData["XeId"] = new SelectList(_context.XeTapLais, "XeId", "XeId", lichTapLai.XeId);
            return View(lichTapLai);
        }

        // POST: LichTapLais/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LichId,GiaovienId,HosoId,XeId,Tgbatdau,Tgketthuc,Diadiem,Noidung,Ghichu")] LichTapLai lichTapLai)
        {
            if (id != lichTapLai.LichId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lichTapLai);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LichTapLaiExists(lichTapLai.LichId))
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
            ViewData["GiaovienId"] = new SelectList(_context.GiaoViens, "GiaovienId", "GiaovienId", lichTapLai.GiaovienId);
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", lichTapLai.HosoId);
            ViewData["XeId"] = new SelectList(_context.XeTapLais, "XeId", "XeId", lichTapLai.XeId);
            return View(lichTapLai);
        }

        // GET: LichTapLais/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichTapLai = await _context.LichTapLais
                .Include(l => l.Giaovien)
                .Include(l => l.Hoso)
                .Include(l => l.Xe)
                .FirstOrDefaultAsync(m => m.LichId == id);
            if (lichTapLai == null)
            {
                return NotFound();
            }

            return View(lichTapLai);
        }

        // POST: LichTapLais/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lichTapLai = await _context.LichTapLais.FindAsync(id);
            if (lichTapLai != null)
            {
                _context.LichTapLais.Remove(lichTapLai);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LichTapLaiExists(int id)
        {
            return _context.LichTapLais.Any(e => e.LichId == id);
        }
    }
}
