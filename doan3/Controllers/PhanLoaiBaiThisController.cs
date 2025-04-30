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
    public class PhanLoaiBaiThisController : Controller
    {
        private readonly DacsGplxContext _context;

        public PhanLoaiBaiThisController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: PhanLoaiBaiThis
        public async Task<IActionResult> Index()
        {
            return View(await _context.PhanLoaiBaiThis.ToListAsync());
        }

        // GET: PhanLoaiBaiThis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phanLoaiBaiThi = await _context.PhanLoaiBaiThis
                .FirstOrDefaultAsync(m => m.LoaibaiId == id);
            if (phanLoaiBaiThi == null)
            {
                return NotFound();
            }

            return View(phanLoaiBaiThi);
        }

        // GET: PhanLoaiBaiThis/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PhanLoaiBaiThis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LoaibaiId,Tenloaibai,Mota")] PhanLoaiBaiThi phanLoaiBaiThi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(phanLoaiBaiThi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(phanLoaiBaiThi);
        }

        // GET: PhanLoaiBaiThis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phanLoaiBaiThi = await _context.PhanLoaiBaiThis.FindAsync(id);
            if (phanLoaiBaiThi == null)
            {
                return NotFound();
            }
            return View(phanLoaiBaiThi);
        }

        // POST: PhanLoaiBaiThis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LoaibaiId,Tenloaibai,Mota")] PhanLoaiBaiThi phanLoaiBaiThi)
        {
            if (id != phanLoaiBaiThi.LoaibaiId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phanLoaiBaiThi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhanLoaiBaiThiExists(phanLoaiBaiThi.LoaibaiId))
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
            return View(phanLoaiBaiThi);
        }

        // GET: PhanLoaiBaiThis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phanLoaiBaiThi = await _context.PhanLoaiBaiThis
                .FirstOrDefaultAsync(m => m.LoaibaiId == id);
            if (phanLoaiBaiThi == null)
            {
                return NotFound();
            }

            return View(phanLoaiBaiThi);
        }

        // POST: PhanLoaiBaiThis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phanLoaiBaiThi = await _context.PhanLoaiBaiThis.FindAsync(id);
            if (phanLoaiBaiThi != null)
            {
                _context.PhanLoaiBaiThis.Remove(phanLoaiBaiThi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhanLoaiBaiThiExists(int id)
        {
            return _context.PhanLoaiBaiThis.Any(e => e.LoaibaiId == id);
        }
    }
}
