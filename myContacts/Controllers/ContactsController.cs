using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace myContacts.Controllers
{
    public class ContactsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
