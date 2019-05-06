using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPI.CSharp.Model
{
    public class Reports
    {
        public Reports()
        {

        }

        public Reports Clone()
        {
            return (Reports)this.MemberwiseClone();
        }

        public string Id { get; set; }

        public string StrategyId { get; set; }

        public string StrategyName { get; set; }

        public int UserId { get; set; }

        public double InitRight { get; set; }

        public double TotalProfit { get; set; }

        public double TotalFee { get; set; }

        public double TotalSlippageFee { get; set; }

        public double MaxRight { get; set; }

        public double MaxUsedMargin { get; set; }

        public double MaxMarketValue { get; set; }

        public double MaxBack { get; set; }

        public DateTime MaxBackTime { get; set; }

        public double MaxWin { get; set; }

        public double MaxLoss { get; set; }

        public int TradedDays { get; set; }

        public double LongWinMoney { get; set; }

        public int LongWinCount { get; set; }

        public double ShortWinMoney { get; set; }

        public int ShortWinCount { get; set; }

        public double LongLossMoney { get; set; }

        public int LongLossCount { get; set; }

        public double ShortLossMoney { get; set; }

        public int ShortLossCount { get; set; }

        public int CurHoldDayCount { get; set; }

        public int MaxHoldDayCount { get; set; }

        public string UserFields { get; set; }
    }
}
