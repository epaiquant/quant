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

        public delegate void OnRtnLogDelegate(string msg, bool isError = false);
        /// <summary>
        /// 日志事件
        /// </summary>
        public event OnRtnLogDelegate OnRtnLogEvent;

        public delegate void OnRtnErrorDelegate(string title, Exception ex);
        /// <summary>
        /// 错误事件
        /// </summary>
        public event OnRtnErrorDelegate OnRtnErrorEvent;

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

        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="isError">是否错误消息</param>
        public void Log(string msg, bool isError = false)
        {
            if (OnRtnLogEvent != null)
                OnRtnLogEvent(msg, isError);
        }
        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="ex">异常</param>
        public void Log(string title, Exception ex)
        {
            if (OnRtnErrorEvent != null)
                OnRtnErrorEvent(title, ex);
        }
    }
}
