using EPI.CSharp.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPI.CSharp.Model
{
    public class Trades
    {
        public int TradeId { get; set; }

        public string OrderId { get; set; }

        public int UserId { get; set; }

        public string StrategyId { get; set; }

        public string Contract { get; set; }

        private EnumSide _eSide;
        /// <summary>
        /// 买卖（通讯字段）
        /// </summary>
        public EnumSide ESide
        {
            get { return _eSide; }
            set
            {
                _eSide = value;
                _side = _eSide.ToString();
            }
        }

        private string _side;
        /// <summary>
        /// 买卖（数据库字段）
        /// </summary>
        public string Side
        {
            get { return _side; }
            set
            {
                _side = value;
                _eSide = (EnumSide)Enum.Parse(typeof(EnumSide), _side);
            }
        }

        public double TradedPrice { get; set; }

        public double Fee { get; set; }

        public double SlippageFee { get; set; }

        public double Offset { get; set; }

        public string RealAccount { get; set; }

        public int TradedVolume { get; set; }

        public DateTime TradedTime { get; set; }

        public string TradeDay { get; set; }


    }
}
