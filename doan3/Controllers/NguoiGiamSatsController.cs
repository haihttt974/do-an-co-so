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
    public class NguoiGiamSatsController : Controller
    {
        private readonly DacsGplxContext _context;

        public NguoiGiamSatsController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: NguoiGiamSats
        public async Task<IActionResult> Index()
        {
            return View(await _context.NguoiGiamSats.ToListAsync());
        }

        // GET: NguoiGiamSats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiGiamSat = await _context.NguoiGiamSats
                .FirstOrDefaultAsync(m => m.NguoigsId == id);
            if (nguoiGiamSat == null)
            {
                return NotFound();
            }

            return View(nguoiGiamSat);
        }

        // GET: NguoiGiamSats/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NguoiGiamSats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NguoigsId,Hoten,Donvi,Sdt,Email,NhomNguoigs")] NguoiGiamSat nguoiGiamSat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nguoiGiamSat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nguoiGiamSat);
        }

        // GET: NguoiGiamSats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiGiamSat = await _context.NguoiGiamSats.FindAsync(id);
            if (nguoiGiamSat == null)
            {
                return NotFound();
            }
            return View(nguoiGiamSat);
        }

        // POST: NguoiGiamSats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NguoigsId,Hoten,Donvi,Sdt,Email,NhomNguoigs")] NguoiGiamSat nguoiGiamSat)
        {
            if (id != nguoiGiamSat.NguoigsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nguoiGiamSat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NguoiGiamSatExists(nguoiGiamSat.NguoigsId))
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
            return View(nguoiGiamSat);
        }

        // GET: NguoiGiamSats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiGiamSat = await _context.NguoiGiamSats
                .FirstOrDefaultAsync(m => m.NguoigsId == id);
            if (nguoiGiamSat == null)
            {
                return NotFound();
            }

            return View(nguoiGiamSat);
        }

        // POST: NguoiGiamSats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nguoiGiamSat = await _context.NguoiGiamSats.FindAsync(id);
            if (nguoiGiamSat != null)
            {
                _context.NguoiGiamSats.Remove(nguoiGiamSat);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NguoiGiamSatExists(int id)
        {
            return _context.NguoiGiamSats.Any(e => e.NguoigsId == id);
        }
    }
}
