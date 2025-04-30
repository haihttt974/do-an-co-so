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
    public class KetQuaThisController : Controller
    {
        private readonly DacsGplxContext _context;

        public KetQuaThisController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: KetQuaThis
        public async Task<IActionResult> Index()
        {
            var dacsGplxContext = _context.KetQuaThis.Include(k => k.Baithi);
            return View(await dacsGplxContext.ToListAsync());
        }

        // GET: KetQuaThis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ketQuaThi = await _context.KetQuaThis
                .Include(k => k.Baithi)
                .FirstOrDefaultAsync(m => m.KetquaId == id);
            if (ketQuaThi == null)
            {
                return NotFound();
            }

            return View(ketQuaThi);
        }

        // GET: KetQuaThis/Create
        public IActionResult Create()
        {
            ViewData["BaithiId"] = new SelectList(_context.BaiThis, "BaithiId", "BaithiId");
            return View();
        }

        // POST: KetQuaThis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KetquaId,BaithiId,Ketqua,Ghichu,Diem")] KetQuaThi ketQuaThi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ketQuaThi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BaithiId"] = new SelectList(_context.BaiThis, "BaithiId", "BaithiId", ketQuaThi.BaithiId);
            return View(ketQuaThi);
        }

        // GET: KetQuaThis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ketQuaThi = await _context.KetQuaThis.FindAsync(id);
            if (ketQuaThi == null)
            {
                return NotFound();
            }
            ViewData["BaithiId"] = new SelectList(_context.BaiThis, "BaithiId", "BaithiId", ketQuaThi.BaithiId);
            return View(ketQuaThi);
        }

        // POST: KetQuaThis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("KetquaId,BaithiId,Ketqua,Ghichu,Diem")] KetQuaThi ketQuaThi)
        {
            if (id != ketQuaThi.KetquaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ketQuaThi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KetQuaThiExists(ketQuaThi.KetquaId))
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
            ViewData["BaithiId"] = new SelectList(_context.BaiThis, "BaithiId", "BaithiId", ketQuaThi.BaithiId);
            return View(ketQuaThi);
        }

        // GET: KetQuaThis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ketQuaThi = await _context.KetQuaThis
                .Include(k => k.Baithi)
                .FirstOrDefaultAsync(m => m.KetquaId == id);
            if (ketQuaThi == null)
            {
                return NotFound();
            }

            return View(ketQuaThi);
        }

        // POST: KetQuaThis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ketQuaThi = await _context.KetQuaThis.FindAsync(id);
            if (ketQuaThi != null)
            {
                _context.KetQuaThis.Remove(ketQuaThi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KetQuaThiExists(int id)
        {
            return _context.KetQuaThis.Any(e => e.KetquaId == id);
        }
    }
}
