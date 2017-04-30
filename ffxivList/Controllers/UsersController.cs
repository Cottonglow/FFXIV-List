using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ffxivList.Data;
using ffxivList.Models;
using Microsoft.AspNetCore.Authorization;

namespace ffxivList.Controllers
{
    [Authorize(Policy = "RequireAdministratorRole")]
    public class UsersController : Controller
    {
        private readonly FfListContext _context;

        public UsersController(FfListContext context)
        {
            _context = context;    
        }

        // GET: Users
        public async Task<IActionResult> Index(string alertMessage)
        {
            if (alertMessage != null)
            {
                ViewData["Alert"] = alertMessage;
            }
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", new { alertMessage = Constants.IdNotFound });
            }

            var user = await _context.Users
                .SingleOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return RedirectToAction("Index", new { alertMessage = Constants.UserNotFound });
            }

            return View(user);
        }
        
        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", new { alertMessage = Constants.IdNotFound });
            }

            var user = await _context.Users.SingleOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return RedirectToAction("Index", new { alertMessage = Constants.UserNotFound });
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserId,UserName,UserEmail,UserRole")] User user)
        {
#if DEBUG
            user.UserId = id;
#endif
            if (id != user.UserId)
            {
                return RedirectToAction("Index", new { alertMessage = Constants.IdNotFound });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (!UserExists(user.UserId))
                    {
                        return RedirectToAction("Index", new { alertMessage = Constants.UserNotFound });
                    }
                    else
                    {
                        return RedirectToAction("Index", new { alertMessage = Constants.DbUpdateError + e.Message });
                    }
                }
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", new { alertMessage = Constants.IdNotFound });
            }

            var user = await _context.Users
                .SingleOrDefaultAsync(m => m.UserId == id);
            
            if (user == null)
            {
                return RedirectToAction("Index", new { alertMessage = Constants.UserNotFound });
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(m => m.UserId == id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
