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
            
            return View(await _context.AllUserCraft.Where(u => u.UserID == userId).ToListAsync());
        }

        // POST: AllUserCrafts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(List<AllUserCraft> allUserCraft)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in allUserCraft)
                {
                    try
                    {
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
                return RedirectToAction("Index");
            }
            return View(allUserCraft);
        }

        // GET: AllUserCrafts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allUserCraft = await _context.AllUserCraft
                .SingleOrDefaultAsync(m => m.UserCraftID == id);
            if (allUserCraft == null)
            {
                return NotFound();
            }

            return View(allUserCraft);
        }

        // GET: AllUserCrafts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AllUserCrafts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserCraftID,UserCraftID,CraftID,UserID,IsComplete,CraftName,CraftLevel")] AllUserCraft allUserCraft)
        {
            if (ModelState.IsValid)
            {
                _context.Add(allUserCraft);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(allUserCraft);
        }

        // GET: AllUserCrafts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allUserCraft = await _context.AllUserCraft.SingleOrDefaultAsync(m => m.UserCraftID == id);
            if (allUserCraft == null)
            {
                return NotFound();
            }
            return View(allUserCraft);
        }

        // POST: AllUserCrafts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserCraftID,UserCraftID,CraftID,UserID,IsComplete,CraftName,CraftLevel")] AllUserCraft allUserCraft)
        {
            if (id != allUserCraft.UserCraftID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(allUserCraft);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AllUserCraftExists(allUserCraft.UserCraftID))
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
            return View(allUserCraft);
        }

        // GET: AllUserCrafts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allUserCraft = await _context.AllUserCraft
                .SingleOrDefaultAsync(m => m.UserCraftID == id);
            if (allUserCraft == null)
            {
                return NotFound();
            }

            return View(allUserCraft);
        }

        // POST: AllUserCrafts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var allUserCraft = await _context.AllUserCraft.SingleOrDefaultAsync(m => m.UserCraftID == id);
            _context.AllUserCraft.Remove(allUserCraft);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool AllUserCraftExists(int id)
        {
            return _context.AllUserCraft.Any(e => e.UserCraftID == id);
        }
    }
}
