using System;
using System.Collections.Generic;

namespace EPI.CSharp.Model
{
    /// <summary>
    /// 策略接口
    /// </summary>
    public interface IStrategy
    {
        /// <summary>
        /// 加载数据次数
        /// </summary>
        int LoadDataCount { get; set; }
        /// <summary>
        /// 策略配置
        /// </summary>
        Settings Setting { get; set; }

        /// <summary>
        /// 加载Bar数据
        /// </summary>
        /// <param name="contract">合约</param>
        /// <param name="cycle">周期</param>
        /// <param name="start">开始时间（包含）</param>
        /// <param name="end">结束时间（不包含）</param>
        /// <param name="isBarBaseOnTick">Bar是基于Tick生成</param>
        bool LoadBarDatas(string contract, string cycle, string start, string end, bool isBarBaseOnTick);
        /// <summary>
        /// 加载Bar数据
        /// </summary>
        /// <param name="contract">合约</param>
        /// <param name="cycle">周期</param>
        /// <param name="takeNumber">条数</param>
        /// <param name="isBarBaseOnTick">Bar是基于Tick生成</param>
        /// <returns></returns>
        bool LoadBarDatas(string contract, string cycle, int takeNumber, bool isBarBaseOnTick);
        /// <summary>
        /// 数据加载完成
        /// </summary>
        /// <param name="contract">合约</param>
        /// <param name="cycle">周期</param>
        /// <param name="barDatas">数据</param>
        /// <param name="isLast">是否最后一批数据</param>
        void FinishedLoadData(string contract, string cycle, List<BarData> barDatas, bool isLast);
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sender">策略参数</param>
        /// <returns></returns>
        bool InitStrategy(object sender);
        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="setting">配置</param>
        /// <returns></returns>
        bool InitSetting(string setting);
        /// <summary>
        /// 初始化入库字段
        /// </summary>
        void InitDBFields();
        /// <summary>
        /// 保存入库字段
        /// </summary>
        void SaveDBFields();
        /// <summary>
        /// 初始化策略
        /// </summary>
        bool StartStrategy();
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="isError">是否错误</param>
        void Log(string msg, bool isError = false);
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="ex">错误</param>
        void Log(string title, Exception ex);
        /// <summary>
        /// 获取前根Bar
        /// </summary>
        /// <param name="contract">合约</param>
        /// <param name="cycle">周期</param>
        /// <param name="num">前面第几根(0:前一根,1:前前根)</param>
        /// <returns></returns>
        BarData GetPreData(string contract, string cycle, int num = 0);

        /// <summary>
        /// 买入
        /// </summary>
        /// <param name="contract">合约</param>
        /// <param name="number">数量</param>
        /// <returns></returns>
        string Buy(string contract, int number);

        /// <summary>
        /// 买入
        /// </summary>
        /// <param name="contract">合约</param>
        /// <param name="price">价格</param>
        /// <param name="number">数量</param>
        /// <returns></returns>
        string Buy(string contract, double price, int number);

        /// <summary>
        /// 卖出
        /// </summary>
        /// <param name="contract">合约</param>
        /// <param name="number">数量</param>
        /// <returns></returns>
        string Sell(string contract, int number);
        /// <summary>
        /// 卖出
        /// </summary>
        /// <param name="contract">合约</param>
        /// <param name="price">价格</param>
        /// <param name="number">数量</param>
        /// <returns></returns>
        string Sell(string contract, double price, int number);

        /// <summary>
        /// 撤销订单
        /// </summary>
        /// <param name="clientOrderId">订单号</param>
        bool Cancel(string clientOrderId);

        /// <summary>
        /// 用于接收Tick数据（如果在里面写大量逻辑，可能导致丢包）
        /// </summary>
        /// <param name="tickData"></param>
        void RtnTickData(TickData tickData);

        /// <summary>
        /// 用于接收Bar数据（如果在里面写大量逻辑，可能导致丢包）
        /// </summary>
        /// <param name="barData">Bar数据</param>
        /// <param name="isNewBar">是否新Bar</param>
        void RtnBarData(BarData barData, bool isNewBar);
        /// <summary>
        /// 返回交易回报
        /// </summary>
        /// <param name="order"></param>
        void RtnOrder(Orders order);
        /// <summary>
        /// 返回成交明细
        /// </summary>
        /// <param name="trade"></param>
        void RtnTrade(Trades trade);
        /// <summary>
        /// 返回消息
        /// </summary>
        /// <param name="msg"></param>
        void RtnMessage(Messages msg);
        /// <summary>
        /// 停止策略
        /// </summary>
        void StopStrategy();

        /// <summary>
        /// 获取持仓信息
        /// </summary>
        /// <returns></returns>
        Positions GetPosition(string contract);
        /// <summary>
        /// 获取策略报告
        /// </summary>
        /// <returns></returns>
        Reports GetReport();
        /// <summary>
        /// 获取合约信息
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        Contracts GetContract(string contract);
        /// 转换参数
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        string[] ConvertParams(object sender);
    }
}
