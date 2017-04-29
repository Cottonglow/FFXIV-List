using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ffxivList.Data;
using ffxivList.Models;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> IndexAdmin()
        {
            List<Levemete> levemete = await _context.Levemetes.ToListAsync();
            return View(levemete);
        }

        // GET: Levemetes/Details/5
        [Authorize(Policy = "RequireAdministratorRole")]
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
        [Authorize(Policy = "RequireAdministratorRole")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Levemetes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "RequireAdministratorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LevemeteID,LevemeteName,LevemeteLevel")] Levemete levemete)
        {
            if (ModelState.IsValid)
            {
                _context.Add(levemete);
                
                await _context.SaveChangesAsync();
                
                List<User> users = await _context.Users.AsNoTracking().ToListAsync();

                foreach (var user in users)
                {
                    _context.UserLevemete.Add(new UserLevemete()
                    {
                        IsComplete = false,
                        LevemeteId = levemete.LevemeteId,
                        UserId = user.UserId
                    });

                    await _context.SaveChangesAsync();
#if DEBUG
                    UserLevemete userlevemete = await _context.UserLevemete.LastAsync();

                    _context.AllUserLevemete.Add(new AllUserLevemete()
                    {
                        IsComplete = false,
                        LevemeteId = levemete.LevemeteId,
                        LevemeteLevel = levemete.LevemeteLevel,
                        LevemeteName = levemete.LevemeteName,
                        UserId = user.UserId,
                        UserLevemeteId = userlevemete.UserLevemeteId
                    });

                    await _context.SaveChangesAsync();
#endif
                }
                return RedirectToAction("IndexAdmin");
            }
            return View(levemete);
        }

        // GET: Levemetes/Edit/5
        [Authorize(Policy = "RequireAdministratorRole")]
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
        [Authorize(Policy = "RequireAdministratorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LevemeteID,LevemeteName,LevemeteIsComplete,LevemeteLevel")] Levemete levemete)
        {
#if DEBUG
            levemete.LevemeteId = id;
#endif
            if (id != levemete.LevemeteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(levemete);

#if DEBUG
                    var allUserLevemetes = await _context.AllUserLevemete.AsNoTracking().Where(leve => leve.LevemeteId == levemete.LevemeteId).ToListAsync();

                    foreach (var allUserLevemete in allUserLevemetes)
                    {
                        _context.AllUserLevemete.Update(new AllUserLevemete()
                        {
                            LevemeteLevel = levemete.LevemeteLevel,
                            LevemeteId = levemete.LevemeteId,
                            IsComplete = allUserLevemete.IsComplete,
                            LevemeteName = levemete.LevemeteName,
                            UserId = allUserLevemete.UserId,
                            UserLevemeteId = allUserLevemete.UserLevemeteId
                        });

                        await _context.SaveChangesAsync();
                    }
#endif
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
        [Authorize(Policy = "RequireAdministratorRole")]
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
        [Authorize(Policy = "RequireAdministratorRole")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var levemete = await _context.Levemetes.SingleOrDefaultAsync(m => m.LevemeteId == id);
            _context.Levemetes.Remove(levemete);

#if DEBUG
            var allUserLevemetes = await _context.AllUserLevemete.AsNoTracking().Where(leve => leve.LevemeteId == levemete.LevemeteId).ToListAsync();
            
            foreach (var allUserLevemete in allUserLevemetes)
            {
                _context.AllUserLevemete.Remove(new AllUserLevemete()
                {
                    LevemeteLevel = levemete.LevemeteLevel,
                    LevemeteId = levemete.LevemeteId,
                    IsComplete = allUserLevemete.IsComplete,
                    LevemeteName = levemete.LevemeteName,
                    UserId = allUserLevemete.UserId,
                    UserLevemeteId = allUserLevemete.UserLevemeteId
                });

                if (allUserLevemete.IsComplete)
                {
                    var users = _context.Users.Find(allUserLevemete.UserId);
                    users.UserLevemetesCompleted -= 1;
                    _context.Users.Update(users);
                }

                await _context.SaveChangesAsync();
            }
#endif

            await _context.SaveChangesAsync();
            return RedirectToAction("IndexAdmin");
        }

        private bool LevemeteExists(int id)
        {
            return _context.Levemetes.Any(e => e.LevemeteId == id);
        }
    }
}
