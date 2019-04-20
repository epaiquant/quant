using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPI.CSharp.Model
{
    public class BarData
    {
        public BarData()
        {

        }
        public BarData(BarData bar)
        {
            Contract = bar.Contract;
            Cycle = bar.Cycle;
            Open = bar.Open;
            High = bar.High;
            Low = bar.Low;
            Volume = bar.Volume;
            OpenInterest = bar.OpenInterest;
            Amount = bar.Amount;
            Close = bar.Close;
            Offset = bar.Offset;
            RealDay = bar.RealDay;
            TradingDay = bar.TradingDay;
            UpdateTime = bar.UpdateTime;
        }
        /// <summary>
        /// 合约名
        /// </summary>
        public string Contract { get; set; }
        /// <summary>
        /// 周期
        /// </summary>
        public string Cycle { get; set; }
        /// <summary>
        /// 开盘价
        /// </summary>
        public double Open { get; set; }
        /// <summary>
        /// 收盘价
        /// </summary>
        public double Close { get; set; }
        /// <summary>
        /// 最高价
        /// </summary>
        public double High { get; set; }
        /// <summary>
        /// 最低价
        /// </summary>
        public double Low { get; set; }
        /// <summary>
        /// 成交量
        /// </summary>
        public double Volume { get; set; }
        /// <summary>
        /// 持仓量
        /// </summary>
        public double OpenInterest { get; set; }
        /// <summary>
        /// 成交金额
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// 偏移量
        /// </summary>
        public double Offset { get; set; }
        /// <summary>
        /// 真实日期
        /// </summary>
        public string RealDay { get; set; }
        /// <summary>
        /// 交易日
        /// </summary>
        public string TradingDay { get; set; }
        /// <summary>
        /// 更新时间(HH:mm:ss)
        /// </summary>
        public string UpdateTime { get; set; }

        /// <summary>
        /// 真实时间
        /// </summary>
        public DateTime RealDateTime
        {
            get
            {
                return DateTime.ParseExact(string.Format("{0} {1}", RealDay, UpdateTime.Length == 5 ? UpdateTime + ":00" : UpdateTime), "yyyyMMdd HH:mm:ss", null);
            }
        }

    }
}
