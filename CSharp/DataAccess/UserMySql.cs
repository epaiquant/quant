using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPI.CSharp.DataAccess
{
    public class UserMySql: BaseMySql
    {
        public void GetUserInfo()
        {
            string sql = @"select * from log_account where (createtime between @startTime and @endTime) and (serverinfo like @serverinfo)";
            List<MySqlParameter> Paramter = new List<MySqlParameter>();
            //Paramter.Add(new MySqlParameter("@startTime", startTime));
            //Paramter.Add(new MySqlParameter("@endTime", endTime));
            //Paramter.Add(new MySqlParameter("@serverinfo", (ConfManager.Ins.currentConf.serverid + "-%")));
            //DataTable data = DbManager.Ins.ExcuteDataTable(sql, Paramter.ToArray());

        }
    }
}
