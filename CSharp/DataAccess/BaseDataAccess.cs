using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPI.CSharp.DataAccess
{
    public class BaseDataAccess
    {


        public BaseDataAccess()
        {
            string constr = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString.ToString();
            //string constr = "Server=localhost;Data Source=127.0.0.1;User ID=root;Password=1234;DataBase=mysql;Charset=gb2312;";
            MySqlConnection conn = new MySqlConnection(constr);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("select * from Users", conn);
            MySqlDataReader dr = cmd.ExecuteReader();

            //dr.Close();
            conn.Close();

        }
        
    }
}
