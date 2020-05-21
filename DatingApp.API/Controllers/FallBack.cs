using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    public class FallBack : Controller // we need view support so not inheriting from controllerbase
    {
        public IActionResult Index() {
            // this method is just finding the index file from the statuc wwwroot  folder
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/HTML");//
        }
    }
}