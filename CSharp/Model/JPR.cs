using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPI.CSharp.Model
{
    public static class JPR
    {
        /// <summary>
        /// 非有效数值
        /// </summary>
        public const double NaN = -3.1415926;
        /// <summary>
        /// 是非有效数
        /// </summary>
        public static bool IsNaN(double value)
        {
            return value == -3.142 || value == -3.1415926 || value == -3.141593 || value == -3.1415925;
        }
        /// <summary>
        /// 是非有效数
        /// </summary>
        public static bool IsNaN(float value)
        {
            return value == -3.142f || value == -3.141593f || value == -3.141592f || value == -3.1415925f;
        }

        /// <summary>
        /// 判断是否为正常价格
        /// </summary>
        public static bool IsValidPrice(double price, bool isGroup)
        {
            if (isGroup)
                return !JPR.IsNaN(price) && price != double.MaxValue && price != double.MinValue;
            else
                return !JPR.IsNaN(price) && price != 0 && price != double.MaxValue && price != double.MinValue;
        }
    }
}
