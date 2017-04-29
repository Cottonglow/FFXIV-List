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
        private readonly FfListContext _context;

        public AllUserLevemetesController(FfListContext context)
        {
            _context = context;    
        }

        // GET: AllUserLevemetes
        public async Task<IActionResult> Index()
        {
            var userId = User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value;

            AllUserModel model = new AllUserModel()
            {
                AllUserLevemetes = await _context.AllUserLevemete.Where(u => u.UserId == userId).ToListAsync(),
                Levemetes = await _context.Levemetes.ToListAsync(),
                User = _context.Users.Find(userId)
            };

            return View(model);
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
                var user = await _context.Users.AsNoTracking().Where(u => u.UserId == allUserLevemetes[0].UserId).ToListAsync();

                foreach (var item in allUserLevemetes)
                {
                    try
                    {
                        var userLevemete = await _context.UserLevemete.AsNoTracking().Where(uq => uq.UserLevemeteId == item.UserLevemeteId).ToListAsync();

                        if (item.IsComplete != userLevemete[0].IsComplete)
                        {
                            if (item.IsComplete)
                            {
                                user[0].UserLevemetesCompleted += 1;
                            }
                            else
                            {
                                user[0].UserLevemetesCompleted -= 1;
                            }
                        }

                        _context.UserLevemete.Update(new UserLevemete()
                        {
                            LevemeteId = item.LevemeteId,
                            IsComplete = item.IsComplete,
                            UserLevemeteId = item.UserLevemeteId,
                            UserId = item.UserId
                        });
#if DEBUG
                        _context.AllUserLevemete.Update(new AllUserLevemete()
                        {
                            LevemeteLevel = item.LevemeteLevel,
                            LevemeteId = item.LevemeteId,
                            IsComplete = item.IsComplete,
                            LevemeteName = item.LevemeteName,
                            UserId = item.UserId,
                            UserLevemeteId = item.UserLevemeteId
                        });
#endif
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AllUserLevemeteExists(item.UserLevemeteId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                _context.Users.Update(user[0]);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(allUserLevemetes);
        }
        
        private bool AllUserLevemeteExists(int id)
        {
            return _context.AllUserLevemete.Any(e => e.UserLevemeteId == id);
        }
    }
}
