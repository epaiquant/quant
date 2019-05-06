using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPI.CSharp.Model.Enums
{
    public enum EnumOrderStatus : byte
    {
        /// <summary>
        /// 全部成交
        /// </summary>
        AllTraded = (byte)'0',
        /// <summary>
        /// 部分成交还在队列中
        /// </summary>
        PartTradedQueueing = (byte)'1',
        /// <summary>
        /// 部分成交不在队列中
        /// </summary>
        PartTradedNotQueueing = (byte)'2',
        /// <summary>
        /// 未成交还在队列中
        /// </summary>
        NoTradeQueueing = (byte)'3',
        /// <summary>
        /// 未成交不在队列中
        /// </summary>
        NoTradeNotQueueing = (byte)'4',
        /// <summary>
        /// 撤单
        /// </summary>
        Canceled = (byte)'5',
        /// <summary>
        /// 限制
        /// </summary>
        Limit = (byte)'6',
        /// <summary>
        /// 未知
        /// </summary>
        Unknown = (byte)'a',
        /// <summary>
        /// 尚未触发
        /// </summary>
        NotTouched = (byte)'b',
        /// <summary>
        /// 已触发
        /// </summary>
        Touched = (byte)'c',
        /// <summary>
        /// 交易中心已收到
        /// </summary>
        CenterReceived = (byte)'d',
        /// <summary>
        /// 算法服务器已接收
        /// </summary>
        AlgServerReceived = (byte)'e',
        /// <summary>
        /// 错误
        /// </summary>
        Error = (byte)'f',
        /// <summary>
        /// 待撤
        /// </summary>
        WaitCancel = (byte)'w'
    }
}
