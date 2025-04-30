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
    public class GiayPhepLaiXesController : Controller
    {
        private readonly DacsGplxContext _context;

        public GiayPhepLaiXesController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: GiayPhepLaiXes
        public async Task<IActionResult> Index()
        {
            var dacsGplxContext = _context.GiayPhepLaiXes.Include(g => g.Hang).Include(g => g.Hoso);
            return View(await dacsGplxContext.ToListAsync());
        }

        // GET: GiayPhepLaiXes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giayPhepLaiXe = await _context.GiayPhepLaiXes
                .Include(g => g.Hang)
                .Include(g => g.Hoso)
                .FirstOrDefaultAsync(m => m.GplxId == id);
            if (giayPhepLaiXe == null)
            {
                return NotFound();
            }

            return View(giayPhepLaiXe);
        }

        // GET: GiayPhepLaiXes/Create
        public IActionResult Create()
        {
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId");
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId");
            return View();
        }

        // POST: GiayPhepLaiXes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GplxId,HosoId,HangId,Ngaycap,Ngayhethan,Ghichu")] GiayPhepLaiXe giayPhepLaiXe)
        {
            if (ModelState.IsValid)
            {
                _context.Add(giayPhepLaiXe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId", giayPhepLaiXe.HangId);
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", giayPhepLaiXe.HosoId);
            return View(giayPhepLaiXe);
        }

        // GET: GiayPhepLaiXes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giayPhepLaiXe = await _context.GiayPhepLaiXes.FindAsync(id);
            if (giayPhepLaiXe == null)
            {
                return NotFound();
            }
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId", giayPhepLaiXe.HangId);
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", giayPhepLaiXe.HosoId);
            return View(giayPhepLaiXe);
        }

        // POST: GiayPhepLaiXes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GplxId,HosoId,HangId,Ngaycap,Ngayhethan,Ghichu")] GiayPhepLaiXe giayPhepLaiXe)
        {
            if (id != giayPhepLaiXe.GplxId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(giayPhepLaiXe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GiayPhepLaiXeExists(giayPhepLaiXe.GplxId))
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
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId", giayPhepLaiXe.HangId);
            ViewData["HosoId"] = new SelectList(_context.HoSoThiSinhs, "HosoId", "HosoId", giayPhepLaiXe.HosoId);
            return View(giayPhepLaiXe);
        }

        // GET: GiayPhepLaiXes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giayPhepLaiXe = await _context.GiayPhepLaiXes
                .Include(g => g.Hang)
                .Include(g => g.Hoso)
                .FirstOrDefaultAsync(m => m.GplxId == id);
            if (giayPhepLaiXe == null)
            {
                return NotFound();
            }

            return View(giayPhepLaiXe);
        }

        // POST: GiayPhepLaiXes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var giayPhepLaiXe = await _context.GiayPhepLaiXes.FindAsync(id);
            if (giayPhepLaiXe != null)
            {
                _context.GiayPhepLaiXes.Remove(giayPhepLaiXe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GiayPhepLaiXeExists(int id)
        {
            return _context.GiayPhepLaiXes.Any(e => e.GplxId == id);
        }
    }
}
