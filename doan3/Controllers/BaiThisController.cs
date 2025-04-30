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
    public class BaiThisController : Controller
    {
        private readonly DacsGplxContext _context;

        public BaiThisController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: BaiThis
        public async Task<IActionResult> Index()
        {
            var dacsGplxContext = _context.BaiThis.Include(b => b.Hoso).Include(b => b.Kythi).Include(b => b.Loaibai);
            return View(await dacsGplxContext.ToListAsync());
        }

        // GET: BaiThis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var baiThi = await _context.BaiThis
                .Include(b => b.Hoso)
                .Include(b => b.Kythi)
                .Include(b => b.Loaibai)
                .FirstOrDefaultAsync(m => m.BaithiId == id);
            if (baiThi == null)
            {
                return NotFound();
            }

            return View(baiThi);
        }

        // GET: BaiThis/Create
        public IActionResult Create()
        {
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId");
            ViewData["KythiId"] = new SelectList(_context.KyThis, "KythiId", "KythiId");
            ViewData["LoaibaiId"] = new SelectList(_context.PhanLoaiBaiThis, "LoaibaiId", "LoaibaiId");
            return View();
        }

        // POST: BaiThis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BaithiId,KythiId,HosoId,LoaibaiId,Tenbaithi,Thutu,Thoigian")] BaiThi baiThi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(baiThi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", baiThi.HosoId);
            ViewData["KythiId"] = new SelectList(_context.KyThis, "KythiId", "KythiId", baiThi.KythiId);
            ViewData["LoaibaiId"] = new SelectList(_context.PhanLoaiBaiThis, "LoaibaiId", "LoaibaiId", baiThi.LoaibaiId);
            return View(baiThi);
        }

        // GET: BaiThis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var baiThi = await _context.BaiThis.FindAsync(id);
            if (baiThi == null)
            {
                return NotFound();
            }
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", baiThi.HosoId);
            ViewData["KythiId"] = new SelectList(_context.KyThis, "KythiId", "KythiId", baiThi.KythiId);
            ViewData["LoaibaiId"] = new SelectList(_context.PhanLoaiBaiThis, "LoaibaiId", "LoaibaiId", baiThi.LoaibaiId);
            return View(baiThi);
        }

        // POST: BaiThis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BaithiId,KythiId,HosoId,LoaibaiId,Tenbaithi,Thutu,Thoigian")] BaiThi baiThi)
        {
            if (id != baiThi.BaithiId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(baiThi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BaiThiExists(baiThi.BaithiId))
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
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", baiThi.HosoId);
            ViewData["KythiId"] = new SelectList(_context.KyThis, "KythiId", "KythiId", baiThi.KythiId);
            ViewData["LoaibaiId"] = new SelectList(_context.PhanLoaiBaiThis, "LoaibaiId", "LoaibaiId", baiThi.LoaibaiId);
            return View(baiThi);
        }

        // GET: BaiThis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var baiThi = await _context.BaiThis
                .Include(b => b.Hoso)
                .Include(b => b.Kythi)
                .Include(b => b.Loaibai)
                .FirstOrDefaultAsync(m => m.BaithiId == id);
            if (baiThi == null)
            {
                return NotFound();
            }

            return View(baiThi);
        }

        // POST: BaiThis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var baiThi = await _context.BaiThis.FindAsync(id);
            if (baiThi != null)
            {
                _context.BaiThis.Remove(baiThi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BaiThiExists(int id)
        {
            return _context.BaiThis.Any(e => e.BaithiId == id);
        }
    }
}
