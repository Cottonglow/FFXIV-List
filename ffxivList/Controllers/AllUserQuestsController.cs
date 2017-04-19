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
    public class AllUserQuestsController : Controller
    {
        private readonly FFListContext _context;

        public AllUserQuestsController(FFListContext context)
        {
            _context = context;    
        }

        // GET: AllUserQuests
        public async Task<IActionResult> Index()
        {
            var userId = User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value;

            AllUserModel model = new AllUserModel()
            {
                AllUserQuests = await _context.AllUserQuest.Where(u => u.UserID == userId).ToListAsync(),
                Quests = await _context.Quest.ToListAsync(),
                User = _context.Users.Find(userId)
            };

            return View(model);
        }

        // POST: AllUserQuests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(List<AllUserQuest> allUserQuests)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.AsNoTracking().Where(u => u.UserId == allUserQuests[0].UserID).ToListAsync();

                foreach (var item in allUserQuests)
                {
                    try
                    {
                        var userQuest = await _context.UserQuest.AsNoTracking().Where(uq => uq.UserQuestID == item.UserQuestID).ToListAsync();

                        if (item.IsComplete != userQuest[0].IsComplete)
                        {
                            if (item.IsComplete)
                            {
                                user[0].UserQuestsCompleted += 1;
                            }
                            else
                            {
                                user[0].UserQuestsCompleted -= 1;
                            }
                        }

                        _context.UserQuest.Update(new UserQuest() { QuestID = item.QuestID, IsComplete = item.IsComplete, UserQuestID = item.UserQuestID, UserID = item.UserID });
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AllUserQuestExists(item.UserQuestID))
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
            return View(allUserQuests);
        }

        private bool AllUserQuestExists(int id)
        {
            return _context.AllUserQuest.Any(e => e.UserQuestID == id);
        }
    }
}
