using Microsoft.AspNetCore.Mvc;

namespace ETicaretWebUI.Areas.Customer.Controllers
{
	public class FakeByDeletedController : Controller
	{
		public IActionResult Index()
		{
			var las = "sasd";
			return View();
		}
	}
}
