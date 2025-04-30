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
    public class LoaiThisController : Controller
    {
        private readonly DacsGplxContext _context;

        public LoaiThisController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: LoaiThis
        public async Task<IActionResult> Index()
        {
            return View(await _context.LoaiThis.ToListAsync());
        }

        // GET: LoaiThis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiThi = await _context.LoaiThis
                .FirstOrDefaultAsync(m => m.LoaithiId == id);
            if (loaiThi == null)
            {
                return NotFound();
            }

            return View(loaiThi);
        }

        // GET: LoaiThis/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LoaiThis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LoaithiId,Tenloaithi")] LoaiThi loaiThi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loaiThi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loaiThi);
        }

        // GET: LoaiThis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiThi = await _context.LoaiThis.FindAsync(id);
            if (loaiThi == null)
            {
                return NotFound();
            }
            return View(loaiThi);
        }

        // POST: LoaiThis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LoaithiId,Tenloaithi")] LoaiThi loaiThi)
        {
            if (id != loaiThi.LoaithiId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaiThi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiThiExists(loaiThi.LoaithiId))
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
            return View(loaiThi);
        }

        // GET: LoaiThis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiThi = await _context.LoaiThis
                .FirstOrDefaultAsync(m => m.LoaithiId == id);
            if (loaiThi == null)
            {
                return NotFound();
            }

            return View(loaiThi);
        }

        // POST: LoaiThis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loaiThi = await _context.LoaiThis.FindAsync(id);
            if (loaiThi != null)
            {
                _context.LoaiThis.Remove(loaiThi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoaiThiExists(int id)
        {
            return _context.LoaiThis.Any(e => e.LoaithiId == id);
        }
    }
}
