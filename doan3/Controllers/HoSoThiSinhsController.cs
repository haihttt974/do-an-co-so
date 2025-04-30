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
    public class HoSoThiSinhsController : Controller
    {
        private readonly DacsGplxContext _context;

        public HoSoThiSinhsController(DacsGplxContext context)
        {
            _context = context;
        }

        // GET: HoSoThiSinhs
        public async Task<IActionResult> Index()
        {
            var dacsGplxContext = _context.HoSoThiSinhs.Include(h => h.Hang).Include(h => h.Hocvien);
            return View(await dacsGplxContext.ToListAsync());
        }

        // GET: HoSoThiSinhs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoSoThiSinh = await _context.HoSoThiSinhs
                .Include(h => h.Hang)
                .Include(h => h.Hocvien)
                .FirstOrDefaultAsync(m => m.HosoId == id);
            if (hoSoThiSinh == null)
            {
                return NotFound();
            }

            return View(hoSoThiSinh);
        }

        // GET: HoSoThiSinhs/Create
        public IActionResult Create()
        {
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId");
            ViewData["HocvienId"] = new SelectList(_context.HocViens, "HocvienId", "HocvienId");
            return View();
        }

        // POST: HoSoThiSinhs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HosoId,HocvienId,ImgThisinh,LoaiHoso,HangId,Ngaydk,Khamsuckhoe,Ghichu")] HoSoThiSinh hoSoThiSinh)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hoSoThiSinh);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId", hoSoThiSinh.HangId);
            ViewData["HocvienId"] = new SelectList(_context.HocViens, "HocvienId", "HocvienId", hoSoThiSinh.HocvienId);
            return View(hoSoThiSinh);
        }

        // GET: HoSoThiSinhs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoSoThiSinh = await _context.HoSoThiSinhs.FindAsync(id);
            if (hoSoThiSinh == null)
            {
                return NotFound();
            }
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId", hoSoThiSinh.HangId);
            ViewData["HocvienId"] = new SelectList(_context.HocViens, "HocvienId", "HocvienId", hoSoThiSinh.HocvienId);
            return View(hoSoThiSinh);
        }

        // POST: HoSoThiSinhs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HosoId,HocvienId,ImgThisinh,LoaiHoso,HangId,Ngaydk,Khamsuckhoe,Ghichu")] HoSoThiSinh hoSoThiSinh)
        {
            if (id != hoSoThiSinh.HosoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoSoThiSinh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoSoThiSinhExists(hoSoThiSinh.HosoId))
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
            ViewData["HangId"] = new SelectList(_context.HangGplxes, "HangId", "HangId", hoSoThiSinh.HangId);
            ViewData["HocvienId"] = new SelectList(_context.HocViens, "HocvienId", "HocvienId", hoSoThiSinh.HocvienId);
            return View(hoSoThiSinh);
        }

        // GET: HoSoThiSinhs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoSoThiSinh = await _context.HoSoThiSinhs
                .Include(h => h.Hang)
                .Include(h => h.Hocvien)
                .FirstOrDefaultAsync(m => m.HosoId == id);
            if (hoSoThiSinh == null)
            {
                return NotFound();
            }

            return View(hoSoThiSinh);
        }

        // POST: HoSoThiSinhs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hoSoThiSinh = await _context.HoSoThiSinhs.FindAsync(id);
            if (hoSoThiSinh != null)
            {
                _context.HoSoThiSinhs.Remove(hoSoThiSinh);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HoSoThiSinhExists(int id)
        {
            return _context.HoSoThiSinhs.Any(e => e.HosoId == id);
        }
    }
}
