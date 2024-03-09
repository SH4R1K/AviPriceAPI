using Microsoft.AspNetCore.Mvc;

namespace AviPriceUI.Controllers
{
    public class HistoryController : Controller
    {
        public IActionResult Index() // History
        {
            return View();
        }
    }
}
