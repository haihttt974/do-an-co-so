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
    public class LichThisController : Controller
    {
        private readonly DacsGplxContext _context;

        public LichThisController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: LichThis
        public async Task<IActionResult> Index()
        {
            return View(await _context.LichThis.ToListAsync());
        }

        // GET: LichThis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichThi = await _context.LichThis
                .FirstOrDefaultAsync(m => m.LichthiId == id);
            if (lichThi == null)
            {
                return NotFound();
            }

            return View(lichThi);
        }

        // GET: LichThis/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LichThis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LichthiId,Thoigianthi,Diadiemthi,Ghichu")] LichThi lichThi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lichThi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lichThi);
        }

        // GET: LichThis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichThi = await _context.LichThis.FindAsync(id);
            if (lichThi == null)
            {
                return NotFound();
            }
            return View(lichThi);
        }

        // POST: LichThis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LichthiId,Thoigianthi,Diadiemthi,Ghichu")] LichThi lichThi)
        {
            if (id != lichThi.LichthiId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lichThi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LichThiExists(lichThi.LichthiId))
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
            return View(lichThi);
        }

        // GET: LichThis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichThi = await _context.LichThis
                .FirstOrDefaultAsync(m => m.LichthiId == id);
            if (lichThi == null)
            {
                return NotFound();
            }

            return View(lichThi);
        }

        // POST: LichThis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lichThi = await _context.LichThis.FindAsync(id);
            if (lichThi != null)
            {
                _context.LichThis.Remove(lichThi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LichThiExists(int id)
        {
            return _context.LichThis.Any(e => e.LichthiId == id);
        }
    }
}
