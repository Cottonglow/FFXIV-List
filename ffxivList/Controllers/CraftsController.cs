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
    public class CraftsController : Controller
    {
        private readonly FfListContext _context;

        public CraftsController(FfListContext context)
        {
            
            _context = context;    
        }

        // GET: Crafts
        public async Task<IActionResult> Index()
        {
            List<Craft> crafts = await _context.Craft.ToListAsync();
            return View(crafts);
        }

        // GET: Crafts
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> IndexAdmin(string alertMessage)
        {
            if (alertMessage != null)
            {
                ViewData["Alert"] = alertMessage;
            }
            List<Craft> crafts = await _context.Craft.ToListAsync();
            return View(crafts);
        }

        // GET: Crafts/Details/5
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("IndexAdmin", new {alertMessage = Constants.IdNotFound });
            }

            var craft = await _context.Craft
                .SingleOrDefaultAsync(m => m.CraftId == id);
            if (craft == null)
            {
                return RedirectToAction("IndexAdmin", new { alertMessage = Constants.CraftNotFound });
            }

            return View(craft);
        }

        // GET: Crafts/Create
        [Authorize(Policy = "RequireAdministratorRole")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Crafts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "RequireAdministratorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CraftID,CraftName,CraftLevel")] Craft craft)
        {
            if (ModelState.IsValid)
            {
                if (craft.CraftName.Length > 50 || craft.CraftLevel > 60 || craft.CraftLevel < 1)
                {
                    return RedirectToAction("IndexAdmin", new { alertMessage = Constants.ParametersNotAllowed});
                }

                _context.Add(craft);

                await _context.SaveChangesAsync();

                Craft c = await _context.Craft.AsNoTracking().LastAsync();

                List<User> users = await _context.Users.AsNoTracking().ToListAsync();

                foreach (var user in users)
                {
                    _context.UserCraft.Add(new UserCraft()
                    {
                        IsComplete = false,
                        CraftId = c.CraftId,
                        UserId = user.UserId
                    });

                    await _context.SaveChangesAsync();
#if DEBUG
                    UserCraft userCraft = await _context.UserCraft.LastAsync();

                    _context.AllUserCraft.Add(new AllUserCraft()
                    {
                        IsComplete = false,
                        CraftId = craft.CraftId,
                        CraftLevel = craft.CraftLevel,
                        CraftName = craft.CraftName,
                        UserId = user.UserId,
                        UserCraftId = userCraft.UserCraftId
                    });

                    await _context.SaveChangesAsync();
#endif
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("IndexAdmin");
            }
            return View(craft);
        }

        // GET: Crafts/Edit/5
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("IndexAdmin", new { alertMessage = Constants.IdNotFound });
            }

            var craft = await _context.Craft
                .SingleOrDefaultAsync(m => m.CraftId == id);
            if (craft == null)
            {
                return RedirectToAction("IndexAdmin", new { alertMessage = Constants.CraftNotFound });
            }
            return View(craft);
        }

        // POST: Crafts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "RequireAdministratorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CraftID,CraftName,CraftLevel")] Craft craft)
        {
#if DEBUG
            craft.CraftId = id;
#endif
            if (id != craft.CraftId)
            {
                return RedirectToAction("IndexAdmin", new { alertMessage = Constants.IdNotFound });
            }

            if (craft.CraftName.Length > 50 || craft.CraftLevel > 60 || craft.CraftLevel < 1)
            {
                return RedirectToAction("IndexAdmin", new { alertMessage = Constants.ParametersNotAllowed });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(craft);
#if DEBUG
                    var allUserCrafts = await _context.AllUserCraft.AsNoTracking().Where(c => c.CraftId == craft.CraftId).ToListAsync();

                    foreach (var allUserCraft in allUserCrafts)
                    {
                        _context.AllUserCraft.Update(new AllUserCraft()
                        {
                            CraftLevel = craft.CraftLevel,
                            CraftId = craft.CraftId,
                            IsComplete = allUserCraft.IsComplete,
                            CraftName = craft.CraftName,
                            UserId = allUserCraft.UserId,
                            UserCraftId = allUserCraft.UserCraftId
                        });

                        await _context.SaveChangesAsync();
                    }
#endif
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (!CraftExists(craft.CraftId))
                    {
                        return RedirectToAction("IndexAdmin", new { alertMessage = Constants.CraftNotFound });
                    }
                    else
                    {
                        return RedirectToAction("IndexAdmin", new { alertMessage = Constants.DbUpdateError + e.Message });
                    }
                }
                return RedirectToAction("IndexAdmin");
            }
            return View(craft);
        }

        // GET: Crafts/Delete/5
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("IndexAdmin", new { alertMessage = Constants.IdNotFound });
            }

            var craft = await _context.Craft
                .SingleOrDefaultAsync(m => m.CraftId == id);
            if (craft == null)
            {
                return RedirectToAction("IndexAdmin", new { alertMessage = Constants.CraftNotFound });
            }

            return View(craft);
        }

        // POST: Crafts/Delete/5
        [Authorize(Policy = "RequireAdministratorRole")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var craft = await _context.Craft
                .SingleOrDefaultAsync(m => m.CraftId == id);
            if (craft == null)
            {
                return RedirectToAction("IndexAdmin", new { alertMessage = Constants.CraftNotFound });
            }
            
            _context.Craft.Remove(craft);
#if DEBUG
            var allUserCrafts = await _context.AllUserCraft.AsNoTracking().Where(c => c.CraftId == craft.CraftId).ToListAsync();

            foreach (var allUserCraft in allUserCrafts)
            {
                _context.AllUserCraft.Remove(new AllUserCraft()
                {
                    CraftLevel = craft.CraftLevel,
                    CraftId = craft.CraftId,
                    IsComplete = allUserCraft.IsComplete,
                    CraftName = craft.CraftName,
                    UserId = allUserCraft.UserId,
                    UserCraftId = allUserCraft.UserCraftId
                });

                if (allUserCraft.IsComplete)
                {
                    var users = _context.Users.Find(allUserCraft.UserId);
                    users.UserCraftsCompleted -= 1;
                    _context.Users.Update(users);
                }

                await _context.SaveChangesAsync();
            }
#endif
            await _context.SaveChangesAsync();
            return RedirectToAction("IndexAdmin");
        }

        private bool CraftExists(int id)
        {
            return _context.Craft.Any(e => e.CraftId == id);
        }
    }
}
