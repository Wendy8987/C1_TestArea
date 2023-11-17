using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model_Library.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;

namespace Model_Library.Controllers
{
    public class UserController : Controller
    {
        //ValidateCode
        public FileContentResult ValidateCode()
        {
            string code = "";
            MemoryStream ms = new Captcha().Create(out code);
            Session["gif"] = code;//验证码存储在Session中，供验证。
            Response.ClearContent();//清空输出流
            return File(ms.ToArray(), @"image/png");
        }
        public ActionResult MyProfile()
        {
            if (Session["Account"] == null) {
                return RedirectToAction("Index","Home");
            }
            return View();
        }

        // 
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        //Register
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserModel User)
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

            Usermanager usermanager = new Usermanager();

            //檢查是否重複
            bool isDuplicate = CheckDuplicateData(User.Username,User.Email);

            if (isDuplicate)
            {
                TempData["error"] = "郵箱/賬號已被註冊！";
                return View();
            }
            else
            {
                if (User.Password != null || User.Password_2 != null)
                {
                    if (User.Password.Equals(User.Password_2))
                    {
                        usermanager.NewUser(User, host, port, database, Username, password);
                        TempData["Successfully"] = "Register Successfully";

                    }
                    else
                    {
                        TempData["error"] = "Password not same!";
                    }
                }
                else if (User.First_Name == null || User.Last_Name == null || User.Username == null || User.Email == null || User.Location == null || User.Company_Organization == null || User.Occupation == null || User.Password == null || User.Password_2 == null)
                {
                    TempData["error"] = "Fill in All!";
                }


                return RedirectToAction("Login","User");
            }
        }

        private bool CheckDuplicateData(string username,string email)
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

            Usermanager check = new Usermanager();

            return check.checkeded(username,email, host, port, database, Username, password);
        
        }

        //Login
        public ActionResult Login()
        {
            return View();

        }

        [HttpPost]
        public ActionResult Login(UserModel user)
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

            string code = Request.Form["code"];
            if (Session["gif"] != null)
            {
                if (!string.IsNullOrEmpty(code) && code == Session["gif"].ToString())
                {
                    if (user.Email == null)
                    {
                        TempData["error"] = "Please Input your account!";
                    }
                    else
                    {
                        Usermanager usermanager = new Usermanager();
                        List<UserModel> users = usermanager.Login(user, host, port, database, Username, password);
                        if (users != null)
                        {
                            foreach (UserModel user_ in users)
                            {
                                Session["Account"] = user_.Email;
                                Session["Username"] = user_.Username;
                                Session["Id"] = user_.Id;
                                Session["First_Name"] = user_.First_Name;
                                Session["Last_Name"] = user_.Last_Name;
                                Session["Location"] = user_.Location;
                                Session["Company"] = user_.Company_Organization;
                                Session["Occupation"] = user_.Occupation;
                                Session["Permission"] = user_.Permission;
                            }
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            TempData["error"] = "Incorrect Email or Password!";
                        }
                    }
                }
                else
                {
                    TempData["error"] = "驗證碼錯誤";
                }

            }
            return View("Login");
        }
        //Logout
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}