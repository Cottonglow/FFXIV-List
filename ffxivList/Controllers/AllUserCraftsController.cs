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
    public class AllUserCraftsController : Controller
    {
        private readonly FFListContext _context;

        public AllUserCraftsController(FFListContext context)
        {
            _context = context;    
        }

        // GET: AllUserCrafts
        public async Task<IActionResult> Index()
        {
            var userId = User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value;

            AllUserModel model = new AllUserModel()
            {
                AllUserCrafts = await _context.AllUserCraft.Where(u => u.UserID == userId).ToListAsync(),
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
                var user = await _context.Users.AsNoTracking().Where(u => u.UserId == allUserCrafts[0].UserID).ToListAsync();

                foreach (var item in allUserCrafts)
                {
                    try
                    {
                        var userCraft = await _context.UserCraft.AsNoTracking().Where(uq => uq.UserCraftID == item.UserCraftID).ToListAsync();

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

                        _context.UserCraft.Update(new UserCraft() { CraftID = item.CraftID, IsComplete = item.IsComplete, UserCraftID = item.UserCraftID, UserID = item.UserID });
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AllUserCraftExists(item.UserCraftID))
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
            return _context.AllUserCraft.Any(e => e.UserCraftID == id);
        }
    }
}
