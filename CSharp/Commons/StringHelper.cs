using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPI.CSharp.Commons
{
    public static class StringHelper
    {
        /// <summary>
        /// 转换周期为分钟数
        /// </summary>
        /// <param name="cycle"></param>
        /// <returns></returns>
        public static int ConvertCycleToMinute(string cycle)
        {
            cycle = cycle.ToUpper();
            var sign = cycle[cycle.Length - 1];
            var numStr = cycle.Substring(0, cycle.Length - 1);
            var num = 0;
            if (int.TryParse(numStr, out num))
            {
                if (sign == 'H')
                {
                    return num * 60;
                }
                else if (sign == 'D')
                {
                    return num * 60 * 24;
                }
                else if (sign == 'W')
                {
                    return num * 60 * 24 * 7;
                }
                else if (sign == 'U')
                {
                    return num * 60 * 24 * 30;
                }
                else if (sign == 'Y')
                {
                    return num * 60 * 24 * 365;
                }
                else
                {
                    return num;
                }
            }
            else return -1;
        }

        /// <summary>
        /// 是否标准合约
        /// </summary>
        /// <param name="cycle">周期</param>
        /// <returns></returns>
        public static bool IsStandardCycle(string cycle)
        {
            cycle = cycle.ToUpper();
            return cycle == "1M" || cycle == "3M" || cycle == "5M" || cycle == "15M" || cycle == "1D";
        }

        /// <summary>
        /// 获取提取数据数量
        /// </summary>
        /// <param name="cycle"></param>
        /// <param name="minCycle"></param>
        /// <param name="takeNumber"></param>
        /// <returns></returns>
        public static int GetTakeNumber(string cycle, string minCycle, int takeNumber)
        {
            char symbol = cycle[cycle.Length - 1];
            int usrNum = int.Parse(RemoveNotNumber(cycle));
            if (symbol == 'M')
            {
                int realNum = int.Parse(RemoveNotNumber(minCycle));
                return takeNumber * (usrNum / realNum);
            }
            else if (symbol == 'H')
            {
                return takeNumber * 4 * usrNum;
            }
            else if (symbol == 'W')
            {
                return takeNumber * 7 * usrNum;
            }
            else if (symbol == 'U')
            {
                return takeNumber * 30 * usrNum;
            }
            else
            {
                return takeNumber * 360 * usrNum;
            }
        }

        /// <summary>
        /// 创建数据库字符串（In内字符串）
        /// </summary>
        /// <param name="strArray">字符串队列</param>
        /// <param name="isNumber">是否数字</param>
        /// <returns></returns>
        public static string CreateDBInStr(string[] strArray, bool isNumber = true)
        {
            for (int i = 0; i < strArray.Length; i++)
            {
                strArray[i] = strArray[i].Replace("'", "''");
            }
            if (isNumber)
                return string.Join(",", strArray);
            else
                return string.Format("'{0}'", string.Join("','", strArray));
        }

        /// <summary>
        /// 返回构成周期
        /// </summary>
        /// <param name="objCycle"></param>
        /// <returns></returns>
        public static string ComposeCycle(string objCycle)
        {
            if (RemoveNumber(objCycle) == "U" || RemoveNumber(objCycle) == "W" || RemoveNumber(objCycle) == "Y" || RemoveNumber(objCycle) == "D")
            { return "1D"; }
            else if (RemoveNumber(objCycle) == "H")
            { return "15M"; }
            else if (IsStandardCycle(objCycle))
            { return objCycle; }
            else if (RemoveNumber(objCycle) == "M")
            {
                int Number = 1;
                if (int.TryParse(RemoveNotNumber(objCycle), out Number))
                {
                    if (Number % 15 == 0)
                    { return "15M"; }
                    else if (Number % 5 == 0)
                    { return "5M"; }
                    else if (Number % 3 == 0)
                    { return "3M"; }
                    else
                    { return "1M"; }
                }
                else
                { return "1M"; }
            }
            else
            { return null; }
        }

        /// <summary>
        /// 去掉字符串中的数字
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        public static string RemoveNumber(string key)
        {
            return System.Text.RegularExpressions.Regex.Replace(key, @"\d", "");
        }

        /// <summary>
        /// 去掉字符串中的非数字
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        public static string RemoveNotNumber(string key)
        {
            return System.Text.RegularExpressions.Regex.Replace(key, @"[^\d]*", "");
        }

    }
}
