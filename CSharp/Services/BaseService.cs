using EPI.CSharp.Commons;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPI.CSharp.Services
{
    public class BaseService
    {
        #region 私有方法
        private string _hostUrl;    // 主机Url
        private NetHelper _net;     // 网络类

        #endregion
        /// <summary>
        /// 网络帮助
        /// </summary>
        internal NetHelper Net { get { return _net; } }

        internal string HostUrl { get { return _hostUrl; } }

        public BaseService()
        {
            if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["HostUrl"]))
                _hostUrl = "http://localhost:3000";
            else
                _hostUrl = ConfigurationManager.AppSettings["HostUrl"];
            _net = new NetHelper();
        }
    }
}
