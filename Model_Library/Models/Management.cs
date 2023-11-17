using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Model_Library.Models
{
    public class Management
    {
        public List<UserModel> ShowAccount(string host, string port, string database, string Username, string password)
        {
            string DBconfig = "Host = " + host + " ;Port =" + port + " ;Database =" + database + " ;Username = " + Username + " ; Password =" + password + ";";
            List<UserModel> displayuser = new List<UserModel>();
            NpgsqlConnection conn = new NpgsqlConnection(DBconfig);
            conn.Open();
            NpgsqlCommand comm = new NpgsqlCommand();
            comm.Connection = conn;
            comm.CommandType = CommandType.Text;
            comm.CommandText = "select * from Users_List ORDER BY ID";
            NpgsqlDataReader reader = comm.ExecuteReader();

            while (reader.Read())
            {
                var user = new UserModel();
                user.Id = Convert.ToInt32(reader["Id"]);
                user.Email = reader["Email"].ToString();
                user.Password = reader["Password"].ToString();
                user.First_Name = reader["First_Name"].ToString();
                user.Last_Name = reader["Last_Name"].ToString();
                user.Location = reader["Location"].ToString();
                user.Company_Organization = reader["Company_Organization"].ToString();
                user.Occupation = reader["Occupation"].ToString();
                user.Permission = Convert.ToInt32(reader["Permission"]);
                displayuser.Add(user);
            }
            conn.Close();
            return displayuser;

        }

        public void EditAccount(string host, string port, string database, string Username, string password,string permission,UserModel user)
        {
            string DBconfig = "Host = " + host + " ;Port =" + port + " ;Database =" + database + " ;Username = " + Username + " ; Password =" + password + ";";
            NpgsqlConnection conn = new NpgsqlConnection(DBconfig);
            conn.Open();
            NpgsqlCommand comm = new NpgsqlCommand();
            comm.Connection = conn;
            comm.CommandType = CommandType.Text;
            comm.CommandText = "UPDATE Users_List SET Permission='"+ permission+"' WHERE Id='"+user.Id+"'";
            comm.ExecuteNonQuery();


        }

        public List<Objectmodel> ShowObject(string host,string port,string database,string Username, string password) {
            
            string DBconfig = "Host = " + host + " ;Port =" + port + " ;Database =" + database + " ;Username = " + Username + " ; Password =" + password + ";";
            List<Objectmodel> displayobject = new List<Objectmodel>();
            NpgsqlConnection conn = new NpgsqlConnection(DBconfig);
            conn.Open();
            NpgsqlCommand comm = new NpgsqlCommand();
            comm.Connection = conn;
            comm.CommandType = CommandType.Text;
            comm.CommandText = "select * from Object_List ORDER BY ObjectCategory";
            NpgsqlDataReader reader = comm.ExecuteReader();
            while (reader.Read())
            {
                var obj = new Objectmodel();
                obj.Id = Convert.ToInt32(reader["Id"]);
                obj.Author = reader["Author"].ToString();
                obj.OrganizationOfAuthor = reader["OrganizationOfAuthor"].ToString();
                obj.Owner = reader["Owner"].ToString();
                obj.ObjectName = reader["ObjectName"].ToString();
                obj.ObjectImage = reader["ObjectImage"].ToString();
                obj.ObjectCategory = reader["ObjectCategory"].ToString();
                obj.ObjectIntroduction = reader["ObjectIntroduction"].ToString();
                obj.ReleaseDate = reader["ReleaseDate"].ToString();
                obj.VersionNumber = reader["VersionNumber"].ToString();
                obj.Width = (float)Convert.ToDouble(reader["Width"]);
                obj.Height = (float)Convert.ToDouble(reader["Height"]);
                obj.Depth = (float)Convert.ToDouble(reader["Depth"]);
                obj.InstallSpecification = reader["InstallSpecification"].ToString();
                obj.TechnicalSpecification = reader["TechnicalSpecification"].ToString();
                obj.ProductMaterialStructure = reader["ProductMaterialStructure"].ToString();
                obj.Location = reader["Location"].ToString();
                displayobject.Add(obj);
            }
            conn.Close();

            return displayobject;
        }

        public void EditObject(string host, string port, string database, string Username, string password, Objectmodel obj, string Owner )
        {
            string DBconfig = "Host = " + host + " ;Port =" + port + " ;Database =" + database + " ;Username = " + Username + " ; Password =" + password + ";";
            NpgsqlConnection conn = new NpgsqlConnection(DBconfig);
            conn.Open();
            NpgsqlCommand comm = new NpgsqlCommand();
            comm.Connection = conn;
            comm.CommandType = CommandType.Text;
            comm.CommandText = "UPDATE Object_List SET Owner='"+Owner+"' WHERE Id='"+obj.Id+"'";
            comm.ExecuteNonQuery();


        }

    }
}