using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;

namespace Model_Library.Models
{
    public class ViewModel
    {
        public string ModelName { get; set; }
        public HttpPostedFileBase[] Object_M { get; set; }
    }

    public class Get_object
    {

        public void GetModelList(string ftp, string ftpUserName, string ftpPassword, HttpPostedFileBase file, string folder_Name)
        {
            string fileName = Path.GetFileName(file.FileName);
            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp + folder_Name + fileName);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
           
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            Console.WriteLine(reader);

            reader.Close();
            reader.Dispose();
            response.Close();
        }
    }
}