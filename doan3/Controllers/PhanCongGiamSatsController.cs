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
    public class PhanCongGiamSatsController : Controller
    {
        private readonly DacsGplxContext _context;

        public PhanCongGiamSatsController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: PhanCongGiamSats
        public async Task<IActionResult> Index()
        {
            var dacsGplxContext = _context.PhanCongGiamSats.Include(p => p.Baithi).Include(p => p.Nguoigs);
            return View(await dacsGplxContext.ToListAsync());
        }

        // GET: PhanCongGiamSats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phanCongGiamSat = await _context.PhanCongGiamSats
                .Include(p => p.Baithi)
                .Include(p => p.Nguoigs)
                .FirstOrDefaultAsync(m => m.PhancongGsId == id);
            if (phanCongGiamSat == null)
            {
                return NotFound();
            }

            return View(phanCongGiamSat);
        }

        // GET: PhanCongGiamSats/Create
        public IActionResult Create()
        {
            ViewData["BaithiId"] = new SelectList(_context.BaiThis, "BaithiId", "BaithiId");
            ViewData["NguoigsId"] = new SelectList(_context.NguoiGiamSats, "NguoigsId", "NguoigsId");
            return View();
        }

        // POST: PhanCongGiamSats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PhancongGsId,NguoigsId,BaithiId,Vaitro,Ghichu")] PhanCongGiamSat phanCongGiamSat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(phanCongGiamSat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BaithiId"] = new SelectList(_context.BaiThis, "BaithiId", "BaithiId", phanCongGiamSat.BaithiId);
            ViewData["NguoigsId"] = new SelectList(_context.NguoiGiamSats, "NguoigsId", "NguoigsId", phanCongGiamSat.NguoigsId);
            return View(phanCongGiamSat);
        }

        // GET: PhanCongGiamSats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phanCongGiamSat = await _context.PhanCongGiamSats.FindAsync(id);
            if (phanCongGiamSat == null)
            {
                return NotFound();
            }
            ViewData["BaithiId"] = new SelectList(_context.BaiThis, "BaithiId", "BaithiId", phanCongGiamSat.BaithiId);
            ViewData["NguoigsId"] = new SelectList(_context.NguoiGiamSats, "NguoigsId", "NguoigsId", phanCongGiamSat.NguoigsId);
            return View(phanCongGiamSat);
        }

        // POST: PhanCongGiamSats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PhancongGsId,NguoigsId,BaithiId,Vaitro,Ghichu")] PhanCongGiamSat phanCongGiamSat)
        {
            if (id != phanCongGiamSat.PhancongGsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phanCongGiamSat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhanCongGiamSatExists(phanCongGiamSat.PhancongGsId))
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
            ViewData["BaithiId"] = new SelectList(_context.BaiThis, "BaithiId", "BaithiId", phanCongGiamSat.BaithiId);
            ViewData["NguoigsId"] = new SelectList(_context.NguoiGiamSats, "NguoigsId", "NguoigsId", phanCongGiamSat.NguoigsId);
            return View(phanCongGiamSat);
        }

        // GET: PhanCongGiamSats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phanCongGiamSat = await _context.PhanCongGiamSats
                .Include(p => p.Baithi)
                .Include(p => p.Nguoigs)
                .FirstOrDefaultAsync(m => m.PhancongGsId == id);
            if (phanCongGiamSat == null)
            {
                return NotFound();
            }

            return View(phanCongGiamSat);
        }

        // POST: PhanCongGiamSats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phanCongGiamSat = await _context.PhanCongGiamSats.FindAsync(id);
            if (phanCongGiamSat != null)
            {
                _context.PhanCongGiamSats.Remove(phanCongGiamSat);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhanCongGiamSatExists(int id)
        {
            return _context.PhanCongGiamSats.Any(e => e.PhancongGsId == id);
        }
    }
}
