using EPI.CSharp.Commons;
using EPI.CSharp.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPI.CSharp.DataAccess
{
    public class ContractMySql: BaseMySql
    {
        /// <summary>
        /// 获取合约信息
        /// </summary>
        /// <returns></returns>
        public List<Contracts> GetContracts()
        {
            List<Contracts> contractList = new List<Contracts>();
            string sql = @"select * from contracts where status=1";
            DataTable dt = ExcuteDataTable(sql, null);
            if (dt!=null)
            {
                foreach(DataRow row in dt.Rows)
                {
                    Contracts contract = new Contracts()
                    {
                        Contract = row["Code"].ToString(),
                        Category = row["Category"].ToString(),
                        PriceTick = double.Parse(row["PriceTick"].ToString()),
                        VolumeMultiple = double.Parse(row["VolumeMultiple"].ToString()),
                        Fee = double.Parse(row["Fee"].ToString()),
                        Margin = double.Parse(row["Margin"].ToString())
                    };
                    contractList.Add(contract);
                }
            }
            return contractList;
        }

        /// <summary>
        /// 获取合约
        /// </summary>
        /// <param name="contracts"></param>
        /// <returns></returns>
        public List<Contracts> GetContracts(string[] contracts)
        {
            List<Contracts> contractList = new List<Contracts>();
            string sql = @"select * from contracts where Status=1 and Code in("+ StringHelper.CreateDBInStr(contracts, false) + ")";
            DataTable dt = ExcuteDataTable(sql, null);
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Contracts contract = new Contracts()
                    {
                        Contract = row["Code"].ToString(),
                        Category = row["Category"].ToString(),
                        PriceTick = double.Parse(row["PriceTick"].ToString()),
                        VolumeMultiple = double.Parse(row["VolumeMultiple"].ToString()),
                        Fee = double.Parse(row["Fee"].ToString()),
                        Margin = double.Parse(row["Margin"].ToString())
                    };
                    contractList.Add(contract);
                }
            }
            return contractList;
        }
    }
}
