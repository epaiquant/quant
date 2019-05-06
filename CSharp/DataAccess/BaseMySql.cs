using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPI.CSharp.DataAccess
{
    public class BaseMySql
    {
        //连接用的字符串
        private string connStr;
        public string ConnStr
        {
            get { return this.connStr; }
            set { this.connStr = value; }
        }

        public BaseMySql() {
            connStr = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString.ToString();
        }

        ////DbManager单实例
        //private static BaseMySql _instance = null;
        //public static BaseMySql Ins
        //{
        //    get { if (_instance == null) { _instance = new BaseMySql(); } return _instance; }
        //}

        /// <summary>
        /// 需要获得多个结果集的时候用该方法，返回DataSet对象。
        /// </summary>
        /// <param name="sql语句"></param>
        /// <returns></returns>

        public DataSet ExecuteDataSet(string sql, params MySqlParameter[] paras)
        {
            using (MySqlConnection con = new MySqlConnection(ConnStr))
            {
                //数据适配器
                MySqlDataAdapter sqlda = new MySqlDataAdapter(sql, con);
                if (paras != null)
                    sqlda.SelectCommand.Parameters.AddRange(paras);
                DataSet ds = new DataSet();
                sqlda.Fill(ds);
                return ds;
                //不需要打开和关闭链接.
            }
        }

        /// <summary>
        /// 获得单个结果集时使用该方法，返回DataTable对象。
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>

        public DataTable ExcuteDataTable(string sql, params MySqlParameter[] paras)
        {
            using (MySqlConnection con = new MySqlConnection(ConnStr))
            {
                MySqlDataAdapter sqlda = new MySqlDataAdapter(sql, con);
                if (paras != null)
                {
                    sqlda.SelectCommand.Parameters.AddRange(paras);
                }
                DataTable dt = new DataTable();
                sqlda.Fill(dt);
                return dt;
            }
        }


        /// <summary>   
        /// 执行一条计算查询结果语句，返回查询结果（object）。   
        /// </summary>   
        /// <param name="SQLString">计算查询结果语句</param>   
        /// <returns>查询结果（object）</returns>   
        public object ExecuteScalar(string SQLString, params MySqlParameter[] paras)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        if (paras != null)
                            cmd.Parameters.AddRange(paras);
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 执行Update,Delete,Insert操作
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteNonquery(string sql, params MySqlParameter[] paras)
        {
            using (MySqlConnection con = new MySqlConnection(ConnStr))
            {
                MySqlCommand cmd = new MySqlCommand(sql, con);
                if (paras != null)
                    cmd.Parameters.AddRange(paras);
                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 调用存储过程 无返回值
        /// </summary>
        /// <param name="procname">存储过程名</param>
        /// <param name="paras">sql语句中的参数数组</param>
        /// <returns></returns>
        public int ExecuteProcNonQuery(string procname, params MySqlParameter[] paras)
        {
            using (MySqlConnection con = new MySqlConnection(ConnStr))
            {
                MySqlCommand cmd = new MySqlCommand(procname, con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (paras != null)
                    cmd.Parameters.AddRange(paras);
                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 存储过程 返回Datatable
        /// </summary>
        /// <param name="procname"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public DataTable ExecuteProcQuery(string procname, params MySqlParameter[] paras)
        {
            using (MySqlConnection con = new MySqlConnection(ConnStr))
            {
                MySqlCommand cmd = new MySqlCommand(procname, con);
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlDataAdapter sqlda = new MySqlDataAdapter(procname, con);
                if (paras != null)
                    sqlda.SelectCommand.Parameters.AddRange(paras);
                DataTable dt = new DataTable();
                sqlda.Fill(dt);
                return dt;
            }
        }

        /// <summary>
        /// 多语句的事物管理
        /// </summary>
        /// <param name="cmds">命令数组</param>
        /// <returns></returns>
        public bool ExcuteCommandByTran(params MySqlCommand[] cmds)
        {
            using (MySqlConnection con = new MySqlConnection(ConnStr))
            {
                con.Open();
                MySqlTransaction tran = con.BeginTransaction();
                foreach (MySqlCommand cmd in cmds)
                {
                    cmd.Connection = con;
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                }
                tran.Commit();
                return true;
            }
        }

        ///分页
        public DataTable ExcuteDataWithPage(string sql, ref int totalCount, params MySqlParameter[] paras)
        {
            using (MySqlConnection con = new MySqlConnection(ConnStr))
            {
                MySqlDataAdapter dap = new MySqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                if (paras != null)
                    dap.SelectCommand.Parameters.AddRange(paras);
                dap.Fill(dt);
                MySqlParameter ttc = dap.SelectCommand.Parameters["@totalCount"];
                if (ttc != null)
                {
                    totalCount = Convert.ToInt32(ttc.Value);
                }
                return dt;
            }
        }

    }
}
