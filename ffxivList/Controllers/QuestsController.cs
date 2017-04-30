using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ffxivList.Data;
using ffxivList.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ffxivList.Controllers
{
    public class QuestsController : Controller
    {
        private readonly FfListContext _context;

        public QuestsController(FfListContext context)
        {
            _context = context;    
        }

        // GET: Quests
        public async Task<IActionResult> Index()
        {
            ModelContainer modelContainer = new ModelContainer {Quest = await _context.Quest.ToListAsync()};
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (User.Identity.IsAuthenticated && userId != null)
            {
                modelContainer.UserQuest = await _context.UserQuest.Where(l => l.UserId == userId).ToListAsync();
            }
            return View(modelContainer);
        }

        // GET: Quests
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> IndexAdmin(string alertMessage)
        {
            if (alertMessage != null)
            {
                ViewData["Alert"] = alertMessage;
            }
            ModelContainer modelContainer = new ModelContainer {Quest = await _context.Quest.ToListAsync()};
            return View(modelContainer);
        }
        
        // GET: Quests/Details/5
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("IndexAdmin", new { alertMessage = Constants.IdNotFound });
            }

            var quest = await _context.Quest
                .SingleOrDefaultAsync(m => m.QuestId == id);
            if (quest == null)
            {
                return RedirectToAction("IndexAdmin", new { alertMessage = Constants.QuestNotFound });
            }

            return View(quest);
        }

        // GET: Quests/Create
        [Authorize(Policy = "RequireAdministratorRole")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Quests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "RequireAdministratorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuestID,QuestName,QuestLevel")] Quest quest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(quest);

                await _context.SaveChangesAsync();

                Quest q = await _context.Quest.AsNoTracking().LastAsync();

                List<User> users = await _context.Users.AsNoTracking().ToListAsync();

                foreach (var user in users)
                {
                    _context.UserQuest.Add(new UserQuest()
                    {
                        IsComplete = false,
                        QuestId = q.QuestId,
                        UserId = user.UserId
                    });

                    await _context.SaveChangesAsync();

#if DEBUG
                    UserQuest userQuest = await _context.UserQuest.LastAsync();

                    _context.AllUserQuest.Add(new AllUserQuest()
                    {
                        IsComplete = false,
                        QuestId = quest.QuestId,
                        QuestLevel = quest.QuestLevel,
                        QuestName = quest.QuestName,
                        UserId = user.UserId,
                        UserQuestId = userQuest.UserQuestId
                    });

                    await _context.SaveChangesAsync();
#endif
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("IndexAdmin");
            }
            return View(quest);
        }

        // GET: Quests/Edit/5
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("IndexAdmin", new { alertMessage = Constants.IdNotFound });
            }

            var quest = await _context.Quest.SingleOrDefaultAsync(m => m.QuestId == id);
            if (quest == null)
            {
                return RedirectToAction("IndexAdmin", new { alertMessage = Constants.QuestNotFound });
            }
            return View(quest);
        }

        // POST: Quests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "RequireAdministratorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("QuestID,QuestName,QuestLevel")] Quest quest)
        {
#if DEBUG
            quest.QuestId = id;
#endif
            if (id != quest.QuestId)
            {
                return RedirectToAction("IndexAdmin", new { alertMessage = Constants.IdNotFound });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quest);
#if DEBUG
                    var allUserQuests = await _context.AllUserQuest.AsNoTracking().Where(q => q.QuestId == quest.QuestId).ToListAsync();

                    foreach (var allUserQuest in allUserQuests)
                    {
                        _context.AllUserQuest.Update(new AllUserQuest()
                        {
                            QuestLevel = quest.QuestLevel,
                            QuestId = quest.QuestId,
                            IsComplete = allUserQuest.IsComplete,
                            QuestName = quest.QuestName,
                            UserId = allUserQuest.UserId,
                            UserQuestId = allUserQuest.UserQuestId
                        });

                        await _context.SaveChangesAsync();
                    }
#endif
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (!QuestExists(quest.QuestId))
                    {
                        return RedirectToAction("IndexAdmin", new { alertMessage = Constants.QuestNotFound });
                    }
                    else
                    {
                        return RedirectToAction("IndexAdmin", new { alertMessage = Constants.DbUpdateError + e.Message});
                    }
                }
                return RedirectToAction("IndexAdmin");
            }
            return View(quest);
        }

        // GET: Quests/Delete/5
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("IndexAdmin", new { alertMessage = Constants.IdNotFound });
            }

            var quest = await _context.Quest
                .SingleOrDefaultAsync(m => m.QuestId == id);
            if (quest == null)
            {
                return RedirectToAction("IndexAdmin", new { alertMessage = Constants.QuestNotFound });
            }

            return View(quest);
        }

        // POST: Quests/Delete/5
        [Authorize(Policy = "RequireAdministratorRole")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quest = await _context.Quest.SingleOrDefaultAsync(m => m.QuestId == id);
            _context.Quest.Remove(quest);
#if DEBUG
            var allUserQuests = await _context.AllUserQuest.AsNoTracking().Where(q => q.QuestId == quest.QuestId).ToListAsync();

            foreach (var allUserQuest in allUserQuests)
            {
                _context.AllUserQuest.Remove(new AllUserQuest()
                {
                    QuestLevel = quest.QuestLevel,
                    QuestId = quest.QuestId,
                    IsComplete = allUserQuest.IsComplete,
                    QuestName = quest.QuestName,
                    UserId = allUserQuest.UserId,
                    UserQuestId = allUserQuest.UserQuestId
                });

                if (allUserQuest.IsComplete)
                {
                    var users = _context.Users.Find(allUserQuest.UserId);
                    users.UserQuestsCompleted -= 1;
                    _context.Users.Update(users);
                }

                await _context.SaveChangesAsync();
            }
#endif
            await _context.SaveChangesAsync();
            return RedirectToAction("IndexAdmin");
        }

        private bool QuestExists(int id)
        {
            return _context.Quest.Any(e => e.QuestId == id);
        }
    }
}
