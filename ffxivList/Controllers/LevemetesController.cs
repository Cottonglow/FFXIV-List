using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ffxivList.Data;
using ffxivList.Models;

namespace ffxivList.Controllers
{
    public class LevemetesController : Controller
    {
        private readonly FfListContext _context;

        public LevemetesController(FfListContext context)
        {
            _context = context;     
        }

        // GET: Levemetes
        public async Task<IActionResult> Index()
        {
            List<Levemete> levemete = await _context.Levemetes.ToListAsync();
            return View(levemete);
        }

        // GET: Levemetes
        public async Task<IActionResult> IndexAdmin()
        {
            List<Levemete> levemete = await _context.Levemetes.ToListAsync();
            return View(levemete);
        }

        // GET: Levemetes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var levemete = await _context.Levemetes
                .SingleOrDefaultAsync(m => m.LevemeteId == id);
            if (levemete == null)
            {
                return NotFound();
            }

            return View(levemete);
        }

        // GET: Levemetes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Levemetes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LevemeteID,LevemeteName,LevemeteLevel")] Levemete levemete)
        {
            if (ModelState.IsValid)
            {
                _context.Add(levemete);
                
                await _context.SaveChangesAsync();

                Levemete leve = await _context.Levemetes.AsNoTracking().LastAsync();

                List<User> users = await _context.Users.AsNoTracking().ToListAsync();

                foreach (var user in users)
                {
                    _context.UserLevemete.Add(new UserLevemete()
                    {
                        IsComplete = false,
                        LevemeteId = leve.LevemeteId,
                        UserId = user.UserId
                    });
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("IndexAdmin");
            }
            return View(levemete);
        }

        // GET: Levemetes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var levemete = await _context.Levemetes.SingleOrDefaultAsync(m => m.LevemeteId == id);
            if (levemete == null)
            {
                return NotFound();
            }
            return View(levemete);
        }

        // POST: Levemetes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LevemeteID,LevemeteName,LevemeteIsComplete,LevemeteLevel")] Levemete levemete)
        {
            if (id != levemete.LevemeteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(levemete);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LevemeteExists(levemete.LevemeteId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("IndexAdmin");
            }
            return View(levemete);
        }

        // GET: Levemetes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var levemete = await _context.Levemetes
                .SingleOrDefaultAsync(m => m.LevemeteId == id);
            if (levemete == null)
            {
                return NotFound();
            }

            return View(levemete);
        }

        // POST: Levemetes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var levemete = await _context.Levemetes.SingleOrDefaultAsync(m => m.LevemeteId == id);
            _context.Levemetes.Remove(levemete);
            await _context.SaveChangesAsync();
            return RedirectToAction("IndexAdmin");
        }

        private bool LevemeteExists(int id)
        {
            return _context.Levemetes.Any(e => e.LevemeteId == id);
        }
    }
}
