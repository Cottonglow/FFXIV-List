using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ffxivList.Data;
using ffxivList.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace ffxivList.Controllers
{
    [Authorize]
    public class AllUserCraftsController : Controller
    {
        private readonly FfListContext _context;

        public AllUserCraftsController(FfListContext context)
        {
            _context = context;    
        }

        // GET: AllUserCrafts
        public async Task<IActionResult> Index()
        {
            var userId = User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value;

            AllUserModel model = new AllUserModel()
            {
                AllUserCrafts = await _context.AllUserCraft.Where(u => u.UserId == userId).ToListAsync(),
                Crafts = await _context.Craft.ToListAsync(),
                User = _context.Users.Find(userId)
            };

            return View(model);
        }

        // POST: AllUserCrafts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(List<AllUserCraft> allUserCrafts)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.AsNoTracking().Where(u => u.UserId == allUserCrafts[0].UserId).ToListAsync();

                foreach (var item in allUserCrafts)
                {
                    try
                    {
                        var userCraft = await _context.UserCraft.AsNoTracking().Where(uq => uq.UserCraftId == item.UserCraftId).ToListAsync();

                        if (item.IsComplete != userCraft[0].IsComplete)
                        {
                            if (item.IsComplete)
                            {
                                user[0].UserCraftsCompleted += 1;
                            }
                            else
                            {
                                user[0].UserCraftsCompleted -= 1;
                            }
                        }

                        _context.UserCraft.Update(new UserCraft()
                        {
                            CraftId = item.CraftId,
                            IsComplete = item.IsComplete,
                            UserCraftId = item.UserCraftId,
                            UserId = item.UserId
                        });

#if DEBUG
                        _context.AllUserCraft.Update(new AllUserCraft()
                        {
                            CraftLevel = item.CraftLevel,
                            CraftId = item.CraftId,
                            IsComplete = item.IsComplete,
                            CraftName = item.CraftName,
                            UserId = item.UserId,
                            UserCraftId = item.UserCraftId
                        });
#endif

                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AllUserCraftExists(item.UserCraftId))
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
            return View(allUserCrafts);
        }
        
        private bool AllUserCraftExists(int id)
        {
            return _context.AllUserCraft.Any(e => e.UserCraftId == id);
        }
    }
}
