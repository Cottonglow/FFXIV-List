using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ffxivList.Data;
using ffxivList.Models;

namespace ffxivList.Controllers
{
    public class CraftsController : Controller
    {
        private readonly FFListContext _context;
        ModelContainer modelContainer;

        public CraftsController(FFListContext context)
        {
            
            _context = context;    
        }

        // GET: Crafts
        public async Task<IActionResult> Index()
        {
            modelContainer = new ModelContainer();
            modelContainer.Craft = await _context.Craft.ToListAsync();
            return View(modelContainer);
        }
        
        // GET: Crafts
        public async Task<IActionResult> IndexAdmin()
        {
            ModelContainer modelContainer = new ModelContainer();
            modelContainer.Craft = await _context.Craft.ToListAsync();
            return View(modelContainer);
        }

        // GET: Crafts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var craft = await _context.Craft
                .SingleOrDefaultAsync(m => m.CraftID == id);
            if (craft == null)
            {
                return NotFound();
            }

            return View(craft);
        }

        // GET: Crafts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Crafts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CraftID,CraftName,CraftLevel")] Craft craft)
        {
            if (ModelState.IsValid)
            {
                _context.Add(craft);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(craft);
        }

        // GET: Crafts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var craft = await _context.Craft.SingleOrDefaultAsync(m => m.CraftID == id);
            if (craft == null)
            {
                return NotFound();
            }
            return View(craft);
        }

        // POST: Crafts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CraftID,CraftName,CraftLevel")] Craft craft)
        {
            if (id != craft.CraftID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(craft);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CraftExists(craft.CraftID))
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
            return View(craft);
        }

        // GET: Crafts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var craft = await _context.Craft
                .SingleOrDefaultAsync(m => m.CraftID == id);
            if (craft == null)
            {
                return NotFound();
            }

            return View(craft);
        }

        // POST: Crafts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var craft = await _context.Craft.SingleOrDefaultAsync(m => m.CraftID == id);
            _context.Craft.Remove(craft);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool CraftExists(int id)
        {
            return _context.Craft.Any(e => e.CraftID == id);
        }

        private bool UserCraftExists(int id)
        {
            return _context.UserCraft.Any(e => e.UserCraftID == id);
        }
    }
}
