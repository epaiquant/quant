using EPI.CSharp.Commons;
using EPI.CSharp.DataAccess;
using EPI.CSharp.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPI.CSharp.Services
{
    public class ContractService : BaseService
    {
        /// <summary>
        /// 获取合约信息
        /// </summary>
        /// <returns></returns>
        public List<Contracts> GetContracts()
        {
            try
            {
                ContractMySql contractMySql = new ContractMySql();
                return contractMySql.GetContracts();
            }
            catch(Exception ex)
            {
                Log("GetContracts", ex);
                return null;
            }
        }

        /// <summary>
        /// 获取合约信息
        /// </summary>
        /// <returns></returns>
        public List<Contracts> GetContracts(string[] contracts)
        {
            try
            {
                ContractMySql contractMySql = new ContractMySql();
                return contractMySql.GetContracts(contracts);
            }
            catch (Exception ex)
            {
                Log("GetContracts", ex);
                return null;
            }
        }
    }
}
