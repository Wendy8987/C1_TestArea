using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Model_Library.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangeLanguage(string lang)
        {
            // 将所选的语言设置为会话变量或 Cookie，以便后续请求使用
            Session["Language"] = lang;

            return new EmptyResult();
        }

    }
}