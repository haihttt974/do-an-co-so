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
    public class HangGplxesController : Controller
    {
        private readonly DacsGplxContext _context;

        public HangGplxesController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: HangGplxes
        public async Task<IActionResult> Index()
        {
            return View(await _context.HangGplxes.ToListAsync());
        }

        // GET: HangGplxes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hangGplx = await _context.HangGplxes
                .FirstOrDefaultAsync(m => m.HangId == id);
            if (hangGplx == null)
            {
                return NotFound();
            }

            return View(hangGplx);
        }

        // GET: HangGplxes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HangGplxes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HangId,Tenhang,Mota,Loaiphuongtien,Tuoitoithieu,Tuoitoida,Suckhoe,Hocvan,TgDtLythuyet,TgDtThuchanh,PhiDaotao,PhiThi,PhiCapphep,OnlySatHach")] HangGplx hangGplx)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hangGplx);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hangGplx);
        }

        // GET: HangGplxes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hangGplx = await _context.HangGplxes.FindAsync(id);
            if (hangGplx == null)
            {
                return NotFound();
            }
            return View(hangGplx);
        }

        // POST: HangGplxes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HangId,Tenhang,Mota,Loaiphuongtien,Tuoitoithieu,Tuoitoida,Suckhoe,Hocvan,TgDtLythuyet,TgDtThuchanh,PhiDaotao,PhiThi,PhiCapphep,OnlySatHach")] HangGplx hangGplx)
        {
            if (id != hangGplx.HangId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hangGplx);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HangGplxExists(hangGplx.HangId))
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
            return View(hangGplx);
        }

        // GET: HangGplxes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hangGplx = await _context.HangGplxes
                .FirstOrDefaultAsync(m => m.HangId == id);
            if (hangGplx == null)
            {
                return NotFound();
            }

            return View(hangGplx);
        }

        // POST: HangGplxes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hangGplx = await _context.HangGplxes.FindAsync(id);
            if (hangGplx != null)
            {
                _context.HangGplxes.Remove(hangGplx);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HangGplxExists(int id)
        {
            return _context.HangGplxes.Any(e => e.HangId == id);
        }
    }
}
