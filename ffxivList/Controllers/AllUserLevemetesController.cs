using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ffxivList.Data;
using ffxivList.Models;

namespace ffxivList.Controllers
{
    public class AllUserLevemetesController : Controller
    {
        private readonly FFListContext _context;

        public AllUserLevemetesController(FFListContext context)
        {
            _context = context;    
        }

        // GET: AllUserLevemetes
        public async Task<IActionResult> Index()
        {
            var userId = User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value;

            return View(await _context.AllUserLevemete.Where(u => u.UserID == userId).ToListAsync());
        }

        // POST: AllUserLevemetes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(List<AllUserLevemete> allUserLevemetes)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in allUserLevemetes)
                {
                    try
                    {
                        _context.UserLevemete.Update(new UserLevemete() { LevemeteID = item.LevemeteID, IsComplete = item.IsComplete, UserLevemeteID = item.UserLevemeteID, UserID = item.UserID });
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AllUserLevemeteExists(item.UserLevemeteID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            return View(allUserLevemetes);
        }

        // GET: AllUserLevemetes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allUserLevemete = await _context.AllUserLevemete
                .SingleOrDefaultAsync(m => m.UserLevemeteID == id);
            if (allUserLevemete == null)
            {
                return NotFound();
            }

            return View(allUserLevemete);
        }

        // GET: AllUserLevemetes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AllUserLevemetes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,UserLevemeteID,LevemeteID,UserID,IsComplete,LevemeteName,LevemeteLevel")] AllUserLevemete allUserLevemete)
        {
            if (ModelState.IsValid)
            {
                _context.Add(allUserLevemete);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(allUserLevemete);
        }

        // GET: AllUserLevemetes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allUserLevemete = await _context.AllUserLevemete.SingleOrDefaultAsync(m => m.UserLevemeteID == id);
            if (allUserLevemete == null)
            {
                return NotFound();
            }
            return View(allUserLevemete);
        }

        // POST: AllUserLevemetes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,UserLevemeteID,LevemeteID,UserID,IsComplete,LevemeteName,LevemeteLevel")] AllUserLevemete allUserLevemete)
        {
            if (id != allUserLevemete.UserLevemeteID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(allUserLevemete);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AllUserLevemeteExists(allUserLevemete.UserLevemeteID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(allUserLevemete);
        }

        // GET: AllUserLevemetes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allUserLevemete = await _context.AllUserLevemete
                .SingleOrDefaultAsync(m => m.UserLevemeteID == id);
            if (allUserLevemete == null)
            {
                return NotFound();
            }

            return View(allUserLevemete);
        }

        // POST: AllUserLevemetes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var allUserLevemete = await _context.AllUserLevemete.SingleOrDefaultAsync(m => m.UserLevemeteID == id);
            _context.AllUserLevemete.Remove(allUserLevemete);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool AllUserLevemeteExists(int id)
        {
            return _context.AllUserLevemete.Any(e => e.UserLevemeteID == id);
        }
    }
}
