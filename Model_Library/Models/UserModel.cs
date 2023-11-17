using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Npgsql;
using System.ComponentModel.DataAnnotations;

namespace Model_Library.Models
{

    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Password_2 { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Company_Organization { get; set; }
        public string Occupation { get; set; }
        public string Location { get; set; }
        public int Permission { get; set; }

    }

    public class Usermanager
    {
        public bool checkeded(string username,string email, string host, string port, string database, string Username, string password)
        {

            string DBconfig = "Host = " + host + " ;Port =" + port + " ;Database =" + database + " ;Username = " + Username + " ; Password =" + password + ";";

            NpgsqlConnection conn = new NpgsqlConnection(DBconfig);
            conn.Open();
            NpgsqlCommand comm = new NpgsqlCommand();
            comm.Connection = conn;
            comm.CommandType = CommandType.Text;
            comm.CommandText = "select Email,Username from Users_List where Email ='" + email + "'or Username = '"+ username +"'";
            NpgsqlDataReader reader = comm.ExecuteReader();

            if (reader.Read())
            {
                return true;
            }
            return false;
        }
        public List<UserModel> Login(UserModel User, string host, string port, string database, string Username, string password)
        {
            string DBconfig = "Host = " + host + " ;Port =" + port + " ;Database =" + database + " ;Username = " + Username + " ; Password =" + password + ";";
            string Pwd_1 = User.Password;
            List<UserModel> displayuser = new List<UserModel>();
            NpgsqlConnection conn = new NpgsqlConnection(DBconfig);
            conn.Open();
            NpgsqlCommand comm = new NpgsqlCommand();
            comm.Connection = conn;
            comm.CommandType = CommandType.Text;
            comm.CommandText = "select * from Users_List where Email ='" + User.Email + "'and Password = '" + User.Password + "'";
            NpgsqlDataReader reader = comm.ExecuteReader();
            
            while (reader.Read())
            {
                var user = new UserModel();
                user.Id = Convert.ToInt32(reader["Id"]);
                user.Username = reader["Username"].ToString();
                user.Email = reader["Email"].ToString();
                user.Password = reader["Password"].ToString();
                user.First_Name = reader["First_Name"].ToString();
                user.Last_Name = reader["Last_Name"].ToString();
                user.Location = reader["Location"].ToString();
                user.Company_Organization = reader["Company_Organization"].ToString();
                user.Occupation = reader["Occupation"].ToString();
                user.Permission = Convert.ToInt32(reader["Permission"]);
                displayuser.Add(user);
                if (user.Password.Equals(Pwd_1))
                {
                    return displayuser;
                }
                else {
                    displayuser = null;
                }
            }
            conn.Close();
            
            return null;

        }


        public void NewUser(UserModel User,string host, string port, string database, string Username, string password)
        {
            string DBconfig = "Host = " + host + " ;Port =" + port + " ;Database =" + database + " ;Username = " + Username + " ; Password =" + password + ";";
            string Permission = "1";
            if (User.Email != null)
            {

                if (User.Email.Equals("admin@gmail.com"))
                {
                    Permission = "4";
                }
                else
                {
                    Permission = "1";
                }
                NpgsqlConnection conn = new NpgsqlConnection(DBconfig);
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO users_list(Username,Email,Password,First_Name,Last_Name,Company_Organization,Occupation,Location,Permission) VALUES (:Username, :Email, :Password, :First_Name, :Last_Name, :Company_Organization, :Occupation, :Location, :Permission)";
                cmd.Parameters.Add(new NpgsqlParameter("Username", User.Username));
                cmd.Parameters.Add(new NpgsqlParameter("Email", User.Email));
                cmd.Parameters.Add(new NpgsqlParameter("Password", User.Password));
                cmd.Parameters.Add(new NpgsqlParameter("First_Name", User.First_Name));
                cmd.Parameters.Add(new NpgsqlParameter("Last_Name", User.Last_Name));
                cmd.Parameters.Add(new NpgsqlParameter("Company_Organization", User.Company_Organization));
                cmd.Parameters.Add(new NpgsqlParameter("Occupation", User.Occupation));
                cmd.Parameters.Add(new NpgsqlParameter("Location", User.Location));
                cmd.Parameters.Add(new NpgsqlParameter("Permission", Convert.ToInt32(Permission)));
                cmd.ExecuteNonQuery();
                conn.Close();
            }

        }


    }



}