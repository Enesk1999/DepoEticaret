using Microsoft.AspNetCore.Mvc;

namespace ETicaretWebUI.Areas.Admin.Controllers
{
	public class FakeController : Controller
	{
		public IActionResult Index()
		{
			var ss = "sds";
			var sss = "fakerepos";
			return View();
		}
	}
}
