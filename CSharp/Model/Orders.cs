using EPI.CSharp.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPI.CSharp.Model
{
    public class Orders
    {
        public string OrderId { get; set; }

        public int UserId { get; set; }

        public string StrategyId { get; set; }

        public string OrderType { get; set; }

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

        public double InsertPrice { get; set; }

        public int InsertVolume { get; set; }

        public DateTime InsertTime { get; set; }

        public double TradedPrice { get; set; }

        public int TradedVolume { get; set; }

        public DateTime TradedTime { get; set; }

        public string RealAccount { get; set; }

        private EnumOrderStatus _eOrderStatus;
        /// <summary>
        /// 订单状态（通讯字段）
        /// </summary>
        public EnumOrderStatus EOrderStatus
        {
            get { return _eOrderStatus; }
            set
            {
                _eOrderStatus = value;
                _orderStatus = _eOrderStatus.ToString();
            }
        }
        private string _orderStatus;
        /// <summary>
        /// 订单状态（数据库字段）
        /// </summary>
        public string OrderStatus
        {
            get { return _orderStatus; }
            set
            {
                _orderStatus = value;
                _eOrderStatus = (EnumOrderStatus)Enum.Parse(typeof(EnumOrderStatus), _orderStatus);
            }
        }

        public string TradeDay { get; set; }
    }
}
