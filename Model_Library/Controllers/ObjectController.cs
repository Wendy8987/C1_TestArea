using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ionic.Zip;
using java.sql;
using javax.sql.rowset.serial;
using Model_Library.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Model_Library.Controllers
{
    public class ObjectController : Controller
    {
        // GET: Object
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult ShowUploaded() {
            if (Session["Account"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (Session["Account"].Equals("Guest")) {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                string Account = (string)Session["Account"];
                string filePath = Server.MapPath("~/App_Start/Json/DBconfig.json");
                StreamReader r = new StreamReader(filePath);
                string json = r.ReadToEnd();
                var data = (JObject)JsonConvert.DeserializeObject(json);
                string host = data["dbhost"].Value<string>();
                string port = data["dbport"].Value<string>();
                string database = data["dbdatabase"].Value<string>();
                string Username = data["dbusername"].Value<string>();
                string password = data["dbpass"].Value<string>();


                Object_f obj = new Object_f();
                List<Objectmodel> objs = obj.GetUploadedModels(Account, host, port, database, Username, password);
                if (objs.Count == 0)
                {
                    TempData["Message"] = "You haven Uploaded model!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.User = objs;

                    return View();
                }
            }
        }

        [HttpGet]
        public ActionResult Download(Objectmodel objectmodel) {
            if (Session["Permission"].Equals("0"))
            {
                TempData["error"] = "You must login!";
                return RedirectToAction("ObjectDetail", "Object", new { ObjectName = objectmodel.ObjectName, ReleaseDate = objectmodel.ReleaseDate, Author = objectmodel.Author });
            }
            Object_f obj = new Object_f();
            string filePath = Server.MapPath("~/App_Start/Json/FTPconfig.json");
            StreamReader r = new StreamReader(filePath);
            string json = r.ReadToEnd();
            var data = (JObject)JsonConvert.DeserializeObject(json);
            string ftp = data["ftpserver"].Value<string>();
            string ftpUserName = data["ftpuser"].Value<string>();
            string ftpPassword = data["ftppass"].Value<string>();
            string ftpMainFolder = data["ftpMainFolder"].Value<string>();

            string Foldername = objectmodel.ReleaseDate.Replace("/", "").Replace(":", "").Replace("_", "") + "_" + objectmodel.ObjectName + "_" + objectmodel.Location;

            List<Files> fileList = obj.CreateZipFile(ftp, ftpUserName, ftpPassword, ftpMainFolder, Foldername);
            if (fileList.Count >= 1)
            {
                using (ZipFile zip = new ZipFile())
                {
                    zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                    foreach (Files file in fileList)
                    {
                        zip.AddEntry(file.FileName, file.Bytes);
                    }

                    Response.Clear();
                    Response.BufferOutput = false;
                    string zipName = String.Format("{0}.zip", objectmodel.ObjectName);
                    Response.ContentType = "application/zip";
                    Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                    zip.Save(Response.OutputStream);
                    Response.End();
                }
            }
            else {
                TempData["error"] = "Download Fail,Please Contact us!";
            }

            return RedirectToAction("ObjectDetail", "Object", new { ObjectName = objectmodel.ObjectName, ReleaseDate = objectmodel.ReleaseDate, Author = objectmodel.Author });
        }

        public ActionResult ObjectDetail() {
           return View();
        }
        [HttpPost]
        public ActionResult UploadComment(Objectmodel objectmodel, CommentModel comment) {
            if (Session["Account"].Equals("Guest"))
            {
                TempData["error"] = "Please Login First!";
                return RedirectToAction("ObjectDetail", "Object", new { ObjectName = objectmodel.ObjectName, ReleaseDate = objectmodel.ReleaseDate, Author = objectmodel.Author });
            } else if(Session["Account"] == null){
                TempData["error"] = "Already auto Logout,Please Login again!";
                return RedirectToAction("Login", "User");
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

                Comment com = new Comment();
                comment.CommentUser = (string)Session["Username"];
                comment.CommentUserEmail = (string)Session["Account"];
                comment.CommentTime = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm");
                comment.ModelName = objectmodel.ReleaseDate.Replace("/", "").Replace(":", "").Replace("_", "") + objectmodel.ObjectName;
                com.UploadComment(comment, host, port, database, Username, password);
                return RedirectToAction("ObjectDetail", "Object", new { ObjectName = objectmodel.ObjectName, ReleaseDate = objectmodel.ReleaseDate, Author = objectmodel.Author });
            }
        }
        [HttpGet]
        public ActionResult threeD(Objectmodel objectmodel, CommentModel comment)
        {
            if (Session["Permission"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string filePath = Server.MapPath("~/App_Start/Json/DBconfig.json");
            StreamReader r = new StreamReader(filePath);
            string json = r.ReadToEnd();
            var data = (JObject)JsonConvert.DeserializeObject(json);
            string host = data["dbhost"].Value<string>();
            string port = data["dbport"].Value<string>();
            string database = data["dbdatabase"].Value<string>();
            string Username = data["dbusername"].Value<string>();
            string password = data["dbpass"].Value<string>();

            Comment com = new Comment();
            Object_f obj = new Object_f();

            comment.ModelName = objectmodel.ReleaseDate.Replace("/", "").Replace(":", "").Replace("_", "") + objectmodel.ObjectName;
            List<Objectmodel> objs = obj.GetObjectDetail(objectmodel, host, port, database, Username, password);
            List<CommentModel> comments = com.GetComment(comment, host, port, database, Username, password);
            ViewBag.obj = objs;
            ViewBag.com = comments;

            string FTPfilePath = Server.MapPath("~/App_Start/Json/FTPconfig.json");
            StreamReader reader = new StreamReader(FTPfilePath);
            string jsonFTP = reader.ReadToEnd();
            var dataFTP = (JObject)JsonConvert.DeserializeObject(jsonFTP);
            string ftp = dataFTP["ftpserver"].Value<string>();
            string ftpUserName = dataFTP["ftpuser"].Value<string>();
            string ftpPassword = dataFTP["ftppass"].Value<string>();
            string ftpMainFolder = dataFTP["ftpMainFolder"].Value<string>();

            string Foldername = objectmodel.ReleaseDate.Replace("/", "").Replace(":", "").Replace("_", "") + "_" + objectmodel.ObjectName + "_" + objectmodel.Location;
           
            List<Files> ftp_objs = obj.CreateZipFile(ftp, ftpUserName, ftpPassword, ftpMainFolder, Foldername);

            if (ftp_objs.Count >= 1)
            {
                foreach(Files file in ftp_objs)
                {
                    string filename = Path.GetFileName(file.FileName);
                    List<string> filename_ = filename.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (!filename_.Last(   ).ToLower().Equals("ifc"))
                    {
                        TempData["error"] = "Your model upload wrong file type, Please change to File type(.ifc)";
                        return RedirectToAction("ObjectDetail", "Object", new { ObjectName = objectmodel.ObjectName, ReleaseDate = objectmodel.ReleaseDate, Author = objectmodel.Author });
                    }
                    else
                    {
                        //Blob blob = new SerialBlob(file.Bytes);
                        string hexString = BitConverter.ToString(file.Bytes).Replace("-", string.Empty).ToLower();
                        TempData["url"] = hexString;  
                       
                        
                        //---------------------
                        return RedirectToAction("ObjectDetail", "Object", new { ObjectName = objectmodel.ObjectName, ReleaseDate = objectmodel.ReleaseDate, Author = objectmodel.Author });
                    }
                }
                
            }

            return RedirectToAction("ObjectDetail", "Object");
        }

        [HttpGet]
        public ActionResult ObjectDetail(Objectmodel objectmodel,CommentModel comment)
        {
            if (Session["Permission"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string filePath = Server.MapPath("~/App_Start/Json/DBconfig.json");
            StreamReader r = new StreamReader(filePath);
            string json = r.ReadToEnd();
            var data = (JObject)JsonConvert.DeserializeObject(json);
            string host = data["dbhost"].Value<string>();
            string port = data["dbport"].Value<string>();
            string database = data["dbdatabase"].Value<string>();
            string Username = data["dbusername"].Value<string>();
            string password = data["dbpass"].Value<string>();

            Comment com = new Comment();
            Object_f obj = new Object_f();

            comment.ModelName = objectmodel.ReleaseDate.Replace("/", "").Replace(":", "").Replace("_", "")+objectmodel.ObjectName;
            List<Objectmodel> objs = obj.GetObjectDetail(objectmodel, host, port, database, Username, password);
            List<CommentModel> comments = com.GetComment(comment,host,port,database,Username,password);
            ViewBag.obj = objs;
            ViewBag.com = comments;

            string FTPfilePath = Server.MapPath("~/App_Start/Json/FTPconfig.json");
            StreamReader reader = new StreamReader(FTPfilePath);
            string jsonFTP = reader.ReadToEnd();
            var dataFTP = (JObject)JsonConvert.DeserializeObject(jsonFTP); 
            string ftp = dataFTP["ftpserver"].Value<string>();
            string ftpUserName = dataFTP["ftpuser"].Value<string>();
            string ftpPassword = dataFTP["ftppass"].Value<string>();
            string ftpMainFolder = dataFTP["ftpMainFolder"].Value<string>();

            foreach (Objectmodel specpath in objs)
            {
                string Foldername = specpath.TechnicalSpecification;
                List<Files> ftp_objs = obj.FindSpec(ftp, ftpUserName, ftpPassword, ftpMainFolder, Foldername);
                if (ftp_objs.Count >= 1)
                {
                    List<string> name = new List<string>();
                    foreach (Files file in ftp_objs)
                    {
                        string filename = Path.GetFileName(file.FileName);  
                        name.Add(filename);
                        
                    }
                    ViewBag.specs = name;
                }
                else
                {
                    List<string> name = new List<string>();
                    name.Add("No have Specification");
                    ViewBag.specs = name;
                }
            }
            


            return View();

        }


        public ActionResult Category() {
                return View();
        }

        [HttpGet]
        public ActionResult Category(Objectmodel objectmodel) {
            Session["Category"] = objectmodel.ObjectCategory;
            string filePath = Server.MapPath("~/App_Start/Json/DBconfig.json");
            StreamReader r = new StreamReader(filePath);
            string json = r.ReadToEnd();
            var data = (JObject)JsonConvert.DeserializeObject(json);
            string host = data["dbhost"].Value<string>();
            string port = data["dbport"].Value<string>();
            string database = data["dbdatabase"].Value<string>();
            string Username = data["dbusername"].Value<string>();
            string password = data["dbpass"].Value<string>();

            objectmodel.ObjectCategory = objectmodel.ObjectCategory.ToLower();
            Object_f obj = new Object_f();
            List<Objectmodel> objs = obj.Category(objectmodel, host, port, database, Username, password);
            string[] category = {"sofa","lamp","table","door","window","cabinet","chair","roof", "facadecurtainwall" };
            if (objs.Count == 0)
            {
                if (!category.Contains(objectmodel.ObjectCategory.ToLower())) {
                    TempData["Message"] = "No have this category!";
                    return RedirectToAction("Index", "Home");
                }
                TempData["Message"] = "No have any model!";
                return RedirectToAction("Index", "Home");
            }
            else{ 
                ViewBag.obj = objs;
                
                return View();
            }

            
        }

        public ActionResult UploadObject() {
            if (Session["Account"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (Session["Account"].Equals("Guest"))
            { 
                TempData["error"] = "You Must Login!";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult UploadObject(HttpPostedFileBase[] Specification, Objectmodel objectmodel, HttpPostedFileBase img, HttpPostedFileBase[] Object_M)
        {

            Object_f Object = new Object_f();
            string filePath = Server.MapPath("~/App_Start/Json/FTPconfig.json");
            StreamReader r = new StreamReader(filePath);
            string json = r.ReadToEnd();
            var data = (JObject)JsonConvert.DeserializeObject(json);
            string ftp = data["ftpserver"].Value<string>();
            string ftpUserName = data["ftpuser"].Value<string>();
            string ftpPassword = data["ftppass"].Value<string>();
            string ftpMainFolder = data["ftpMainFolder"].Value<string>();

            string DBfilePath = Server.MapPath("~/App_Start/Json/DBconfig.json");
            StreamReader read = new StreamReader(DBfilePath);
            string Json = read.ReadToEnd();
            var DBdata = (JObject)JsonConvert.DeserializeObject(Json);
            string host = DBdata["dbhost"].Value<string>();
            string port = DBdata["dbport"].Value<string>();
            string database = DBdata["dbdatabase"].Value<string>();
            string Username = DBdata["dbusername"].Value<string>();
            string password = DBdata["dbpass"].Value<string>();

            objectmodel.ReleaseDate = DateTime.Now.ToString("yyyy/MM/dd_HH:mm:ss");
            objectmodel.Author = (string)Session["Account"];

            objectmodel.Location = (string)Session["Location"];
            string FileTime = objectmodel.ReleaseDate.Replace("/", "").Replace(":", "").Replace("_", "");
            string ftpFolder = ftpMainFolder + FileTime + "_" + objectmodel.ObjectName + "_" + objectmodel.Location;

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp + ftpFolder);
            request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
            // create FTP Request
            string SpecFolder = ftpFolder + "/" + "Specification";

            if (!Object.DoesFtpDirectoryExist(ftpFolder, ftp, ftpUserName, ftpPassword))
            {
                
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;

                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
 
                if (!Object.DoesFtpDirectoryExist(SpecFolder, ftp, ftpUserName, ftpPassword))
                {
                    FtpWebRequest request_i = (FtpWebRequest)WebRequest.Create(ftp + ftpFolder + "/" + "Specification");
                    request_i.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                    request_i.UsePassive = true;
                    request_i.UseBinary = true;
                    request_i.KeepAlive = false;

                    request_i.Method = WebRequestMethods.Ftp.MakeDirectory;
                    FtpWebResponse response_i = (FtpWebResponse)request_i.GetResponse();
                    response_i.Close();
                }
                

            }

            if (Specification.Length >=0 ) {
                foreach (HttpPostedFileBase File in Specification) {
                    if (File != null)
                    {
                        string filename = Path.GetFileName(File.FileName);
                        List<string> filename_ = filename.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        if (!filename_.Last().ToLower().Equals("pdf"))
                        {
                            TempData["error"] = "Your Specification upload wrong file type, Please change to .pdf";
                            return View();
                        }
                        string folder_Name = ftpFolder + "/" + "Specification" + "/";
                        Object.UploadFileToFTP(ftp, ftpUserName, ftpPassword, File, folder_Name);
                        Session["check"] = "done";
                    }
                    else
                    {
                        Session["check"] = "fail";
                    }
                }
            }
            if (img != null)
            {
                string filename = Path.GetFileName(img.FileName);
                List<string> filename_ = filename.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (!(filename_.Last().ToLower().Equals("jpg") || filename_.Last().ToLower().Equals("png")))
                {
                    TempData["error"] = "Your images upload wrong file type, Please change to .jpg or .png";
                    return View();
                }
                //change img type
                byte[] uploadedimg = new byte[img.InputStream.Length];
                if (uploadedimg == null)
                {
                    TempData["error"] = "Our system cannot read your images,please contact us.";
                    return View();
                }
                img.InputStream.Read(uploadedimg, 0, uploadedimg.Length);
                string img64 = Convert.ToBase64String(uploadedimg);
                objectmodel.ObjectImage = img64;
            }
            if (Object_M.Length >= 0)
            {
                foreach (HttpPostedFileBase File in Object_M)
                {
                    if (File != null)
                    {
                        string filename = Path.GetFileName(File.FileName);
                        List<string> filename_ = filename.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        if (!(filename_.Last().ToLower().Equals("3dm") || filename_.Last().ToLower().Equals("ifc") || filename_.Last().ToLower().Equals("rvt") || filename_.Last().ToLower().Equals("dwg")))
                        {
                            TempData["error"] = "Your model upload wrong file type, Please change to File type(.3dm/.ifc/.rvt/.dwg)";
                            return View();
                        }
                        objectmodel.InstallSpecification = filename_.Last().ToLower();
                        string folder_Name = ftpFolder + "/";
                        Object.UploadFileToFTP(ftp, ftpUserName, ftpPassword, File, folder_Name);
                        Session["check"] = "done";
                    }
                    else
                    {
                        Session["check"] = "fail";
                    }
                }
            }



            if (Session["check"].Equals("done"))
            {
                
                Object.objectUploadDB(objectmodel, host, port, database, Username, password);
                Session["check"] = "";
                TempData["Successfully"] = "Upload Successfully";
                return RedirectToAction("Category","Object",new { ObjectCategory = objectmodel.ObjectCategory});
            }


            TempData["error"] = "Upload Fail!Please Contact us!";
            if (Object.DoesFtpDirectoryExist(SpecFolder, ftp, ftpUserName, ftpPassword))
            {
                FtpWebRequest request_d = (FtpWebRequest)WebRequest.Create(ftp + ftpFolder + "/" + "Specification");
                request_d.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                request_d.UsePassive = true;
                request_d.UseBinary = true;
                request_d.KeepAlive = false;

                request_d.Method = WebRequestMethods.Ftp.RemoveDirectory;
                FtpWebResponse response_d = (FtpWebResponse)request_d.GetResponse();
                response_d.Close();

            }
            if (Object.DoesFtpDirectoryExist(ftpFolder, ftp, ftpUserName, ftpPassword))
            {
                FtpWebRequest request_d = (FtpWebRequest)WebRequest.Create(ftp + ftpFolder);
                request_d.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                request_d.UsePassive = true;
                request_d.UseBinary = true;
                request_d.KeepAlive = false;

                request_d.Method = WebRequestMethods.Ftp.RemoveDirectory;
                FtpWebResponse response_d = (FtpWebResponse)request_d.GetResponse();
                response_d.Close();
            }

            return RedirectToAction("UploadObject","Object");
        }
    }
}