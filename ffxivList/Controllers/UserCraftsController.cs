using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ffxivList.Data;
using ffxivList.Models;
using System.Security.Claims;

namespace ffxivList.Controllers
{
    public class UserCraftsController : Controller
    {
        private readonly FFListContext _context;
        private ModelContainer modelContainer;

        public UserCraftsController(FFListContext context)
        {
            _context = context;    
        }

        // GET: UserCrafts
        public async Task<IActionResult> Index()
        {
            modelContainer = new ModelContainer();
            modelContainer.Craft = await _context.Craft.ToListAsync();
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (User.Identity.IsAuthenticated && userId != null)
            {
                modelContainer.UserCraft = await _context.UserCraft.Where(l => l.UserID == userId).ToListAsync();
            }
            return View(modelContainer);
        }

        // POST: UserCrafts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int id, [Bind("CraftID,IsComplete")] UserCraft userCraft)
        {
            modelContainer = new ModelContainer();
            modelContainer.Craft = await _context.Craft.ToListAsync();
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (User.Identity.IsAuthenticated && userId != null)
            {
                modelContainer.UserCraft = await _context.UserCraft.AsNoTracking().ToListAsync();
            }

            var userCraftInfo = _context.UserCraft.AsNoTracking().Where(u => u.UserID == userId && u.CraftID == userCraft.CraftID).ToList();

            userCraft.UserCraftID = userCraftInfo[0].UserCraftID;
            userCraft.UserID = userId;

            //if (id != userCraft.UserCraftID)
            //{
            //    return NotFound();
            //}

            if (ModelState.IsValid)
            {
                try
                {                    
                    _context.Update(userCraft);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserCraftExists(userCraft.UserCraftID))
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
            return View(modelContainer);
        }

        // GET: UserCrafts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userCraft = await _context.UserCraft
                .SingleOrDefaultAsync(m => m.UserCraftID == id);
            if (userCraft == null)
            {
                return NotFound();
            }

            return View(userCraft);
        }

        // GET: UserCrafts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserCrafts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserCraftID,CraftID,UserID,IsComplete")] UserCraft userCraft)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userCraft);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(userCraft);
        }

        // GET: UserCrafts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userCraft = await _context.UserCraft.SingleOrDefaultAsync(m => m.UserCraftID == id);
            if (userCraft == null)
            {
                return NotFound();
            }
            return View(userCraft);
        }

        // POST: UserCrafts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserCraftID,CraftID,UserID,IsComplete")] UserCraft userCraft)
        {
            if (id != userCraft.UserCraftID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userCraft);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserCraftExists(userCraft.UserCraftID))
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
            return View(userCraft);
        }

        // GET: UserCrafts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userCraft = await _context.UserCraft
                .SingleOrDefaultAsync(m => m.UserCraftID == id);
            if (userCraft == null)
            {
                return NotFound();
            }

            return View(userCraft);
        }

        // POST: UserCrafts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userCraft = await _context.UserCraft.SingleOrDefaultAsync(m => m.UserCraftID == id);
            _context.UserCraft.Remove(userCraft);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool UserCraftExists(int id)
        {
            return _context.UserCraft.Any(e => e.UserCraftID == id);
        }
    }
}
