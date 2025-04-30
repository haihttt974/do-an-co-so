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
    public class HocViensController : Controller
    {
        private readonly DacsGplxContext _context;

        public HocViensController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: HocViens
        public async Task<IActionResult> Index()
        {
            return View(await _context.HocViens.ToListAsync());
        }

        // GET: HocViens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocVien = await _context.HocViens
                .FirstOrDefaultAsync(m => m.HocvienId == id);
            if (hocVien == null)
            {
                return NotFound();
            }

            return View(hocVien);
        }

        // GET: HocViens/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HocViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HocvienId,Tenhocvien,Socccd,Gioitinh,Ngaysinh")] HocVien hocVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hocVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hocVien);
        }

        // GET: HocViens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocVien = await _context.HocViens.FindAsync(id);
            if (hocVien == null)
            {
                return NotFound();
            }
            return View(hocVien);
        }

        // POST: HocViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HocvienId,Tenhocvien,Socccd,Gioitinh,Ngaysinh")] HocVien hocVien)
        {
            if (id != hocVien.HocvienId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hocVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HocVienExists(hocVien.HocvienId))
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
            return View(hocVien);
        }

        // GET: HocViens/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocVien = await _context.HocViens
                .FirstOrDefaultAsync(m => m.HocvienId == id);
            if (hocVien == null)
            {
                return NotFound();
            }

            return View(hocVien);
        }

        // POST: HocViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hocVien = await _context.HocViens.FindAsync(id);
            if (hocVien != null)
            {
                _context.HocViens.Remove(hocVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HocVienExists(int id)
        {
            return _context.HocViens.Any(e => e.HocvienId == id);
        }
    }
}
