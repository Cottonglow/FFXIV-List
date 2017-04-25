using Microsoft.AspNetCore.Mvc;

namespace ffxivList.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
        
        public IActionResult ErrorFound(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (id.Equals("404"))
            {
                return RedirectToAction("PageNotFound");
            }

            return View();
        }

        public IActionResult PageNotFound()
        {
            return View();
        }
    }
}
