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

            return View(await _context.AllUserQuest.Where(u => u.UserID == userId).ToListAsync());
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
                foreach (var item in allUserQuests)
                {
                    try
                    {
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
                return RedirectToAction("Index");
            }
            return View(allUserQuests);
        }

        // GET: AllUserQuests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allUserQuest = await _context.AllUserQuest
                .SingleOrDefaultAsync(m => m.UserQuestID == id);
            if (allUserQuest == null)
            {
                return NotFound();
            }

            return View(allUserQuest);
        }

        // GET: AllUserQuests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AllUserQuests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,UserQuestID,QuestID,UserID,IsComplete,QuestName,QuestLevel")] AllUserQuest allUserQuest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(allUserQuest);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(allUserQuest);
        }

        // GET: AllUserQuests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allUserQuest = await _context.AllUserQuest.SingleOrDefaultAsync(m => m.UserQuestID == id);
            if (allUserQuest == null)
            {
                return NotFound();
            }
            return View(allUserQuest);
        }

        // POST: AllUserQuests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,UserQuestID,QuestID,UserID,IsComplete,QuestName,QuestLevel")] AllUserQuest allUserQuest)
        {
            if (id != allUserQuest.UserQuestID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(allUserQuest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AllUserQuestExists(allUserQuest.UserQuestID))
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
            return View(allUserQuest);
        }

        // GET: AllUserQuests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allUserQuest = await _context.AllUserQuest
                .SingleOrDefaultAsync(m => m.UserQuestID == id);
            if (allUserQuest == null)
            {
                return NotFound();
            }

            return View(allUserQuest);
        }

        // POST: AllUserQuests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var allUserQuest = await _context.AllUserQuest.SingleOrDefaultAsync(m => m.UserQuestID == id);
            _context.AllUserQuest.Remove(allUserQuest);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool AllUserQuestExists(int id)
        {
            return _context.AllUserQuest.Any(e => e.UserQuestID == id);
        }
    }
}
