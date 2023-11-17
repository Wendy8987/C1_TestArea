using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model_Library.Models;

namespace Model_Library.Controllers
{
    public class ManagementController : Controller
    {
        // GET: AccountManagement
        public ActionResult ShowAcc()
        {
            if (Session["Account"] == null) {
                return RedirectToAction("Index","Home");
            }
            if ((int)Session["Permission"] == 4)
            {
                string filePath = Server.MapPath("~/App_Start/Json/DBconfig.json");
                StreamReader r = new StreamReader(filePath);
                string json = r.ReadToEnd();
                var data = (JObject)JsonConvert.DeserializeObject(json);
                string host = data["dbhost"].Value<string>();
                string port = data["dbport"].Value<string>();
                string database = data["dbdatabase"].Value<string>();
                string Username = data["dbusername"].Value<string>();
                string password = data["dbpass"].Value<string>();

                Management usermanagement = new Management();

                List<UserModel> displayuser = usermanagement.ShowAccount(host, port, database, Username, password);

                ViewBag.User = displayuser;

                return View();
            }
            else
            {
                if (Session["Account"].Equals("Guest"))
                {
                    TempData["error"] = "Please Login your account!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["error"] = "You Not Admin!";
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        [HttpPost]
        public ActionResult EditAcc(UserModel user ,string permission, string permission_2) 
        {
            if (user.Email.Equals(Session["Account"]))
            {
                TempData["error"] = "Can't change own permissions!";
                return RedirectToAction("ShowAcc", "Management");
            }
            else if (permission_2 == permission)
            {
                TempData["error"] = "Can't change permissions!";
                return RedirectToAction("ShowAcc", "Management");
            }
            else if (permission == "") {
                return RedirectToAction("ShowAcc", "Management");
            }
            else
            {
                string filePath = Server.MapPath("~/App_Start/Json/DBconfig.json");
                StreamReader r = new StreamReader(filePath);
                string json = r.ReadToEnd();
                var data = (JObject)JsonConvert.DeserializeObject(json);
                string host = data["dbhost"].Value<string>();
                string port = data["dbport"].Value<string>();
                string database = data["dbdatabase"].Value<string>();
                string Username = data["dbusername"].Value<string>();
                string password = data["dbpass"].Value<string>();

                Management usermanagement = new Management();
                usermanagement.EditAccount(host, port, database, Username, password, permission, user);
                return RedirectToAction("ShowAcc", "Management");
            }
        }

        public ActionResult ShowObj() 
        {
            if (Session["Account"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if ((int)Session["Permission"] == 4)
            {
                string filePath = Server.MapPath("~/App_Start/Json/DBconfig.json");
                StreamReader r = new StreamReader(filePath);
                string json = r.ReadToEnd();
                var data = (JObject)JsonConvert.DeserializeObject(json);
                string host = data["dbhost"].Value<string>();
                string port = data["dbport"].Value<string>();
                string database = data["dbdatabase"].Value<string>();
                string Username = data["dbusername"].Value<string>();
                string password = data["dbpass"].Value<string>();

                Management objmanagement = new Management();

                List<Objectmodel> displayobj = objmanagement.ShowObject(host,port,database,Username,password);
                ViewBag.Obj = displayobj;

                return View();
            }
            else
            {
                if (Session["Account"].Equals("Guest"))
                {
                    TempData["error"] = "Please Login your account!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["error"] = "You Not Admin!";
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        [HttpPost]

        public ActionResult EditObj(Objectmodel obj,string Owner,string Owner_o ) {

            if (Owner == "")
            {
                return RedirectToAction("ShowObj", "Management");
            }
            else if (obj.Owner.Equals(Owner_o))
            {
                return RedirectToAction("ShowObj", "Management");
            }
            else
            {
                string filePath = Server.MapPath("~/App_Start/Json/DBconfig.json");
                StreamReader r = new StreamReader(filePath);
                string json = r.ReadToEnd();
                var data = (JObject)JsonConvert.DeserializeObject(json);
                string host = data["dbhost"].Value<string>();
                string port = data["dbport"].Value<string>();
                string database = data["dbdatabase"].Value<string>();
                string Username = data["dbusername"].Value<string>();
                string password = data["dbpass"].Value<string>();

                Management objmanagement = new Management();
                objmanagement.EditObject(host, port, database, Username, password, obj, Owner);
                return RedirectToAction("ShowObj", "Management");
            }
        }
    }
}