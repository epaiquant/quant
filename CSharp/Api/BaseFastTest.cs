using EPI.CSharp.Commons;
using EPI.CSharp.Model;
using EPI.CSharp.Model.Enums;
using EPI.CSharp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EPI.CSharp.Api
{
    public class BaseFastTest:IStrategy
    {
        #region 私有变量
        string preTradeDay;
        string minCycle;
        bool isChangedDay;

        Reports report;
        List<BarData> totalCsvDataList;                                     //Csv文件数据
        List<DailyMoneys> dailyMoneyList;                                   //每日资金队列
        List<BarData> testBarDataList;                                      //测试Bar数据集
        List<string> pendingOrderList;                                      //挂单队列
        List<Trades> strategyRspTradeList;                                  //策略成交队列
        List<Orders> strategyRspOrderList;                                  //策略委托队列

        Dictionary<string, DateTime> startTimeDict;                         //测试开始时间字典
        Dictionary<string, List<BarData>> preBarsDict;                      //获取前几根Bar字典
        Dictionary<string, BarData> curBarDict;                             //实时bar数据字典
        Dictionary<string, Contracts> contractDict;                         //合约信息字典
        Dictionary<string, Positions> positionDict;                         //持仓字典
        #endregion

        public BaseFastTest(int userId, string strategyId)
        {
            report = new Reports()
            {
                Id = Guid.NewGuid().ToString("N"),
                StrategyId = strategyId,
                UserId = userId
            };
            totalCsvDataList = new List<BarData>();
            pendingOrderList = new List<string>();
            testBarDataList = new List<BarData>();
            dailyMoneyList = new List<DailyMoneys>();
            preBarsDict = new Dictionary<string, List<BarData>>();
            curBarDict = new Dictionary<string, BarData>();
            contractDict = new Dictionary<string, Contracts>();
            positionDict = new Dictionary<string, Positions>();
            startTimeDict = new Dictionary<string, DateTime>();
            strategyRspTradeList = new List<Trades>();
            strategyRspOrderList = new List<Orders>();
        }

        #region 委托方法
        public delegate void OnRtnLogDelegate(string msg, bool isError = false);
        public event OnRtnLogDelegate OnRtnLogEvent;

        public delegate void OnRtnErrorDelegate(string title, Exception ex);
        public event OnRtnErrorDelegate OnRtnErrorEvent;

        public delegate void TestFinishedDelegate(
            Reports report,
            Dictionary<string, Positions> positionDict,
            List<DailyMoneys> dailyMoneys,
            List<Trades> trades);
        public event TestFinishedDelegate OnTestFinishedEvent;
        #endregion

        #region 公共属性
        /// <summary>
        /// 策略配置
        /// </summary>
        public Settings Setting { get; set; }

        /// <summary>
        /// 加载数据次数
        /// </summary>
        public int LoadDataCount { get; set; }

        public bool IsSavedToCsv { get; set; }

        #endregion

        #region 私有方法

        /// <summary>
        /// 设置最小周期
        /// </summary>
        /// <param name="cycleArray"></param>
        void SetMinCycle(string[] cycleArray)
        {
            if (cycleArray != null && cycleArray.Length > 0)
            {
                int minNumber = int.MaxValue;
                minCycle = "1M";
                foreach (var cycle in cycleArray)
                {
                    int number = int.Parse(cycle.Substring(0, cycle.Length - 1));
                    switch (cycle[cycle.Length - 1])
                    {
                        case 'H': number = number * 60; break;
                        case 'D': number = number * 60 * 24; break;
                        case 'W': number = number * 60 * 24 * 7; break;
                        case 'Y': number = number * 60 * 24 * 365; break;
                    }
                    if (number < minNumber)
                    {
                        minNumber = number;
                        minCycle = cycle;
                    }
                }
            }
        }

        /// <summary>
        /// 开始Bar回测
        /// </summary>
        void BeginBarTest()
        {
            List<BarData> totalTestBarList = new List<BarData>();
            totalTestBarList.AddRange(testBarDataList);
            totalTestBarList = totalTestBarList.OrderBy(b => b.RealDateTime).ThenByDescending(b => StringHelper.ConvertCycleToMinute(b.Cycle)).ToList();
            for (int i = 0; i < totalTestBarList.Count; i++)
            {
                var data = totalTestBarList[i];
                
                if (string.IsNullOrEmpty(preTradeDay))
                    preTradeDay = data.TradingDay;
                if (preTradeDay != data.TradingDay)
                {
                    DoChangedDay(preTradeDay);
                    preTradeDay = data.TradingDay;
                }
                string key = string.Format("{0}_{1}", data.Cycle, data.Contract);
                if (curBarDict.ContainsKey(key))
                    curBarDict[key] = data;
                else
                    curBarDict.Add(key, data);

                if (!preBarsDict.ContainsKey(key))
                    preBarsDict.Add(key, new List<BarData>());

                if (data.Cycle == minCycle)
                {
                    foreach (var order in pendingOrderList)
                    {
                        var values = order.Split(',');
                        SendOrder(values[0], values[1], string.IsNullOrEmpty(values[2]) ? JPR.NaN : double.Parse(values[2]),
                            values[3] == "1" ? EnumSide.Buy : EnumSide.Sell, int.Parse(values[4]));
                    }
                    pendingOrderList.Clear();
                }
                try
                {
                    RtnBarData(data, true);
                }
                catch (Exception ex)
                {
                    Log("RtnBarData Error", ex);
                    break;
                }

                if (preBarsDict[key].Count > 10)
                {
                    preBarsDict[key].RemoveAt(0);
                }
                preBarsDict[key].Add(data);
                isChangedDay = false;
            }
            CalculateLeftProfit();
            //保存用户入库变量
            Log("Bar测试完成");
        }

        /// <summary>
        /// 换日处理
        /// </summary>
        /// <param name="preTradeDay"></param>
        private void DoChangedDay(string preTradeDay)
        {
            lock (report)
            {
                var netVolume = 0;
                for (int i = 0; i < positionDict.Count; i++)
                {
                    var std = positionDict.ElementAt(i);
                    if (std.Value.NetVolume != 0)
                    {
                        netVolume = std.Value.NetVolume;
                    }
                }
                if (netVolume != 0)
                {
                    report.CurHoldDayCount++;
                    report.MaxHoldDayCount = Math.Max(report.MaxHoldDayCount, report.CurHoldDayCount);
                }
                else
                {
                    report.CurHoldDayCount = 0;
                }
                report.TradedDays++;
                CalculateDailyProfit(preTradeDay);
                isChangedDay = true;
            }
        }
        /// <summary>
        /// 计算日盈亏
        /// </summary>
        /// <param name="preTradeDay"></param>
        void CalculateDailyProfit(string preTradeDay)
        {
            double totalProfit = 0, totalCommission = 0, totalSlippedFee = 0, totalMargin = 0;
            int totalNetVolume = 0;
            double closePrice = 0;
            foreach (var contract in Setting.Contracts)
            {
                double leftLongMarketValue = 0;
                double leftShortMarketValue = 0;
                var leftVolume = positionDict[contract].NetVolume;
                if (leftVolume != 0)
                {
                    DateTime lastTime = DateTime.Now;
                    if (contractDict.ContainsKey(contract))
                    {
                        double singleOffset = 0;
                        BarData preData = null;//GetPreData(Setting.Contract, minCycle);
                        
                        preData = GetPreData(contract, minCycle);
                        string key = minCycle + "_" + contract;
                        if (preData != null)
                        {
                            closePrice = preData.Close;
                            lastTime = preData.RealDateTime;
                            singleOffset = preData.Offset;
                        }
                        else
                        {
                            closePrice = curBarDict[key].Open;
                            lastTime = curBarDict[key].RealDateTime;
                            singleOffset = curBarDict[key].Offset;
                        }
                        
                        if (leftVolume > 0)
                        {
                            positionDict[contract].LongPrice = closePrice;
                            positionDict[contract].ShortPrice = 0;
                            positionDict[contract].LongVolume = leftVolume;
                            positionDict[contract].ShortVolume = 0;
                            leftShortMarketValue = (closePrice) * Math.Abs(leftVolume) * contractDict[contract].VolumeMultiple;
                            positionDict[contract].ShortMarketValue += leftShortMarketValue;
                        }
                        else if (leftVolume < 0)
                        {
                            positionDict[contract].LongPrice = 0;
                            positionDict[contract].ShortPrice = closePrice;
                            positionDict[contract].LongVolume = 0;
                            positionDict[contract].ShortVolume = Math.Abs(leftVolume);
                            leftLongMarketValue = (closePrice) * Math.Abs(leftVolume) * contractDict[contract].VolumeMultiple;
                            positionDict[contract].LongMarketValue += leftLongMarketValue;
                        }
                    }
                }
                else
                {
                    positionDict[contract].LongPrice = 0;
                    positionDict[contract].ShortPrice = 0;
                    positionDict[contract].LongVolume = 0;
                    positionDict[contract].ShortVolume = 0;
                    positionDict[contract].LongMarketValue = 0;
                    positionDict[contract].ShortMarketValue = 0;
                }
                double dailyProfit = positionDict[contract].ShortMarketValue - positionDict[contract].LongMarketValue;
                totalProfit += dailyProfit;
                totalCommission += positionDict[contract].Fee;
                totalSlippedFee += positionDict[contract].SlippageFee;
                totalMargin += positionDict[contract].UsedMargin;
                positionDict[contract].LongMarketValue = leftShortMarketValue;
                positionDict[contract].ShortMarketValue = leftLongMarketValue;
                if (leftVolume > 0)
                {
                    positionDict[contract].LongVolume = leftVolume;
                    positionDict[contract].ShortVolume = 0;
                }
                else
                {
                    positionDict[contract].ShortVolume = Math.Abs(leftVolume);
                    positionDict[contract].LongVolume = 0;
                }
                totalNetVolume += leftVolume;
            }


            lock (report)
            {
                report.TotalProfit += totalProfit;
                report.MaxRight = Math.Max(report.MaxRight, report.InitRight + totalProfit);

                if (totalProfit > 0)
                {
                    report.MaxWin = Math.Max(totalProfit, report.MaxWin);
                }
                else
                {
                    report.MaxLoss = Math.Min(totalProfit, report.MaxLoss);
                    if (report.MaxBack < report.MaxRight - (report.InitRight+report.TotalProfit))
                    {
                        report.MaxBack = report.MaxRight - (report.InitRight + report.TotalProfit);
                        report.MaxBackTime = DateTime.ParseExact(preTradeDay, "yyyyMMdd", null);
                    }
                }
                //计算日报表
                var dailyMoney = new DailyMoneys()
                {
                    StrategyId = report.StrategyId,
                    UserId = report.UserId,
                    Right = report.InitRight + report.TotalProfit,
                    Fee = totalCommission,
                    Profit = totalProfit,
                    NetVolume = totalNetVolume,
                    UsedMargin = totalMargin,
                    CreateTime = DateTime.ParseExact(preTradeDay, "yyyyMMdd", null)
                };
                DailyMoneys preDaiyMoney = dailyMoneyList.Count > 0 ? dailyMoneyList[dailyMoneyList.Count - 1] : null;

                if (preDaiyMoney == null)
                {
                    dailyMoney.Right = report.InitRight + totalProfit;
                    dailyMoney.BaseYield = totalProfit / report.InitRight;
                    dailyMoney.CompoundYield = totalProfit / report.InitRight;
                    dailyMoney.NetValue = (totalProfit + report.InitRight) / report.InitRight;
                }
                else
                {
                    dailyMoney.Right = preDaiyMoney.Right + totalProfit;
                    dailyMoney.BaseYield = totalProfit / report.InitRight;
                    dailyMoney.CompoundYield = totalProfit / preDaiyMoney.Right;
                    dailyMoney.NetValue = dailyMoney.Right / report.InitRight;
                }
                dailyMoneyList.Add(dailyMoney);
            }
        }

        /// <summary>
        /// 计算逐笔盈亏
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        double CalculateOnePairProfit(string contract)
        {
            if (!string.IsNullOrEmpty(positionDict[contract].LongUnitMarketValues) && !string.IsNullOrEmpty(positionDict[contract].ShortUnitMarketValues))
            {
                var longIndex = positionDict[contract].LongUnitMarketValues.IndexOf(',', 1);
                if (longIndex == -1)
                {
                    longIndex = positionDict[contract].LongUnitMarketValues.Length;
                }
                var longMarketValues = positionDict[contract].LongUnitMarketValues.Substring(1, longIndex - 1).Split(':');
                positionDict[contract].LongUnitMarketValues = positionDict[contract].LongUnitMarketValues.Substring(longIndex, positionDict[contract].LongUnitMarketValues.Length - longIndex);

                var shortIndex = positionDict[contract].ShortUnitMarketValues.IndexOf(',', 1);
                if (shortIndex == -1)
                {
                    shortIndex = positionDict[contract].ShortUnitMarketValues.Length;
                }
                var shortMarketValues = positionDict[contract].ShortUnitMarketValues.Substring(1, shortIndex - 1).Split(':');
                positionDict[contract].ShortUnitMarketValues = positionDict[contract].ShortUnitMarketValues.Substring(shortIndex, positionDict[contract].ShortUnitMarketValues.Length - shortIndex);
                //多头的子空头+空头的子空头 - （多头的子多头 + 空头的子多头） 
                return float.Parse(longMarketValues[1]) + float.Parse(shortMarketValues[1]) - (float.Parse(longMarketValues[0]) + float.Parse(shortMarketValues[0]));
            }
            else
            {
                return JPR.NaN;
            }
        }

        /// <summary>
        /// 计算剩余盈亏
        /// </summary>
        void CalculateLeftProfit()
        {
            string tradeDay = testBarDataList[testBarDataList.Count - 1].TradingDay;
            foreach (var contract in Setting.Contracts)
            {
                var leftVolume = positionDict[contract].NetVolume;
                if (leftVolume != 0)
                {
                    DateTime lastTime = DateTime.Now;
                    double leftLongMarketValue = 0;
                    double leftShortMarketValue = 0;
                    //计算剩余盈亏
                    if (contractDict.ContainsKey(contract))
                    {
                        double closePrice = 0;
                        double singleOffset = 0;
                        string key = minCycle + "_" + contract;
                        closePrice = curBarDict[key].Close;
                        lastTime = curBarDict[key].RealDateTime;
                        singleOffset = curBarDict[key].Offset;
                        
                        if (leftVolume > 0)
                        {
                            leftShortMarketValue += (closePrice) * Math.Abs(leftVolume) * contractDict[contract].VolumeMultiple;
                        }
                        else
                        {
                            leftLongMarketValue += (closePrice) * Math.Abs(leftVolume) * contractDict[contract].VolumeMultiple;
                        }
                    }

                    positionDict[contract].LeftProfit = (positionDict[contract].ShortMarketValue + leftShortMarketValue) - (positionDict[contract].LongMarketValue + leftLongMarketValue);
                    if (leftVolume != 0)
                    {
                        //计算逐笔剩余盈亏
                        var unitLongMarketValue = leftLongMarketValue / Math.Abs(leftVolume);
                        var unitShortMarketValue = leftShortMarketValue / Math.Abs(leftVolume);
                        double leftOnePairProfit = 0;
                        var oldLongUnitMarketValues = positionDict[contract].LongUnitMarketValues;
                        var oldShortUnitMarketValues = positionDict[contract].ShortUnitMarketValues;
                        for (int i = 0; i < Math.Abs(leftVolume); i++)
                        {
                            if (leftVolume < 0)
                            {
                                positionDict[contract].LongUnitMarketValues += string.Format(",{0}:{1}", unitLongMarketValue, unitShortMarketValue);
                            }
                            else
                            {
                                positionDict[contract].ShortUnitMarketValues += string.Format(",{0}:{1}", unitLongMarketValue, unitShortMarketValue);
                            }
                            var profit = CalculateOnePairProfit(contract);
                            if (!JPR.IsNaN(profit))
                            {
                                leftOnePairProfit += profit;
                            }
                        }
                        positionDict[contract].LeftProfit = leftOnePairProfit;
                        positionDict[contract].LongUnitMarketValues = oldLongUnitMarketValues;
                        positionDict[contract].ShortUnitMarketValues = oldShortUnitMarketValues;
                    }
                }
            }
            if (strategyRspTradeList.Count > 0)
            {
                CalculateDailyProfit(tradeDay);

            }
            if (OnTestFinishedEvent != null)
            {
                OnTestFinishedEvent(report, positionDict, dailyMoneyList, strategyRspTradeList);
            }

            if (IsSavedToCsv)
            {
                if (!Directory.Exists("output"))
                    Directory.CreateDirectory("output");

                var tradeFileName = string.Format("output//{0}_trade.csv", DateTime.Now.ToString("yyyyMMdd"));
                if (File.Exists(tradeFileName))
                    File.Delete(tradeFileName);
                ExportToCsv(tradeFileName, strategyRspTradeList);
                var reportFileName = string.Format("output//{0}_report.csv", DateTime.Now.ToString("yyyyMMdd"));
                if (File.Exists(reportFileName))
                    File.Delete(reportFileName);
                ExportToCsv(reportFileName, report);

                Log("导出报表完成");
            }
        }

        /// <summary>
        /// 导出到Csv
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="report"></param>
        public void ExportToCsv(string fullName, Reports report)
        {
            try
            {
                if (!Directory.Exists("output"))
                    Directory.CreateDirectory("output");
                StringBuilder sb = new StringBuilder();
                //生成CSV头
                sb.AppendLine("报表概况");
                sb.AppendLine(string.Format("策略标识,{0}", report.StrategyId));
                sb.AppendLine(string.Format("用户编号,{0}", report.UserId));
                sb.AppendLine(string.Format("初始资金,{0}", report.InitRight));
                sb.AppendLine(string.Format("当前资金,{0}", report.InitRight + report.TotalProfit));
                sb.AppendLine(string.Format("最大资金,{0}", report.MaxRight));
                sb.AppendLine(string.Format("最大盈利,{0}", report.MaxWin));
                sb.AppendLine(string.Format("最大亏损,{0}", report.MaxLoss));
                sb.AppendLine(string.Format("最大回撤,{0}", report.MaxBack));
                sb.AppendLine(string.Format("最大回撤时间,{0}", report.MaxBackTime));
                sb.AppendLine(string.Format("最大市值,{0}", report.MaxMarketValue));
                sb.AppendLine(string.Format("最大占用保证金,{0}", report.MaxUsedMargin));
                sb.AppendLine(string.Format("总手续费,{0}", report.TotalFee));
                sb.AppendLine(string.Format("交易天数,{0}", report.TradedDays));
                sb.AppendLine(string.Format("最大持仓天数,{0}", report.MaxHoldDayCount));
                
                string header = "资金,盈亏,净值,本金收益率,复利收益率,手续费,占用保证金,净头寸,时间";
                StringBuilder sb2 = new StringBuilder();
                sb2.AppendLine(string.Format("{0},逐日结算明细表", report.StrategyName));
                sb2.AppendLine(header);
                for (int i = 0; i < dailyMoneyList.Count; i++)
                {
                    var rptDtl = dailyMoneyList[i];
                    string row = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                        rptDtl.Right, rptDtl.Profit, rptDtl.NetValue, rptDtl.BaseYield, rptDtl.CompoundYield, rptDtl.Fee,
                        rptDtl.UsedMargin, rptDtl.NetVolume, rptDtl.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    sb2.AppendLine(row);
                }

                using (FileStream fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                    sw.Flush();
                    sw.Write(sb.ToString());
                    sw.Flush();
                    sw.Close();
                }
                fullName = fullName.Replace("report", "daily");
                using (FileStream fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                    sw.Flush();
                    sw.Write(sb2.ToString());
                    sw.Flush();
                    sw.Close();
                }
                //Log("导出Csv文件成功");
            }
            catch (Exception ex)
            {
                Log("ExportToCsv", ex);
            }
        }

        /// <summary>
        /// 导出到Csv
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="tradeList"></param>
        public void ExportToCsv(string fullName, List<Trades> tradeList)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                //生成CSV头
                string header = "合约,买卖,成交价格,成交数量,手续费,成交时间,策略标识,用户编号,交易日";
                sb.AppendLine(header);
                for (int i = 0; i < tradeList.Count; i++)
                {
                    var trade = tradeList[i];
                    string row = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                        trade.Contract, trade.Side, trade.TradedPrice, trade.TradedVolume, trade.Fee,
                        trade.TradedTime, trade.StrategyId, trade.UserId, trade.TradeDay);
                    sb.AppendLine(row);
                }
                using (FileStream fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                    sw.Flush();
                    sw.Write(sb.ToString());
                    sw.Flush();
                    sw.Close();
                }
                //Log("导出Csv文件成功");
            }
            catch (Exception ex)
            {
                Log("ExportToCsv", ex);
            }
        }

        /// <summary>
        /// 检查开仓数量
        /// </summary>
        /// <param name="contract">合约</param>
        /// <param name="openVol">本次开仓量（区分正负）</param>
        /// <returns></returns>
        int CheckOpenVol(string contract, int openVol)
        {
            int curVol = 0;
            lock (positionDict)
            {
                curVol = positionDict[contract].NetVolume + openVol;
            }
            if ((curVol > 0 && Math.Abs(curVol) > Setting.MaxLongVolumeAllowed)
                || (curVol < 0 && Math.Abs(curVol) > Setting.MaxShortVolumeAllowed))
            {
                return 0;
            }
            else
            {
                return Math.Abs(openVol);
            }
        }
        /// <summary>
        /// 发送订单
        /// </summary>
        /// <param name="clientOrderId"></param>
        /// <param name="contract">合约</param>
        /// <param name="price">价格</param>
        /// <param name="side">买卖</param>
        /// <param name="volume">数量</param>
        /// <returns></returns>
        string SendOrder(string clientOrderId, string contract, double price, EnumSide side, int volume)
        {
            lock (report)
            {
                try
                {
                    var cInfo = GetContract(contract);
                    int openNum = CheckOpenVol(contract, side == EnumSide.Buy ? volume : volume * -1);
                    if (openNum <= 0)
                    {
                        Log(string.Format("超出最大手数限制,允许最大多头:{0},允许最大空头:{1},当前多头:{2},当前空头:{3},本次{4}{5}手",
                            Setting.MaxLongVolumeAllowed, Setting.MaxShortVolumeAllowed, positionDict[contract].LongVolume, positionDict[contract].ShortVolume,
                            side == EnumSide.Buy ? "买入" : "卖出", volume));
                        return "-1";
                    }
                    string key = string.Format("{0}_{1}", minCycle, contract);
                    BarData tradedBar = null;
                    double tradePrice = price;
                    DateTime createTime = DateTime.Now;
                    string tradeDay = "";
                    string updateTime = "";
                    List<double> singleTradeList = new List<double>();
                    
                    if (curBarDict.ContainsKey(key))
                    {
                        tradedBar = curBarDict[key];
                        if (tradePrice == JPR.NaN)
                            tradePrice = tradedBar.Open;
                    }
                    if (tradedBar != null)
                    {
                        createTime = tradedBar.RealDateTime;
                        tradeDay = tradedBar.TradingDay == null ? tradedBar.RealDay : tradedBar.TradingDay;
                        updateTime = tradedBar.RealDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    List<string> tradeContractList = new List<string>();
                    List<int> tradeVolList = new List<int>();
                    List<EnumSide> tradeSideList = new List<EnumSide>();
                    
                    tradeSideList.Add(EnumSide.Buy);
                    tradeVolList.Add(1);
                    tradeContractList.Add(contract);
                    List<Trades> curTradeList = new List<Trades>();
                    for (int i = 0; i < tradeContractList.Count; i++)
                    {
                        var subTradeContract = tradeContractList[i];
                        string subKey = string.Format("{0}_{1}", minCycle, subTradeContract);
                        double singleTradePrice = tradePrice;
                        double singleOffset = 0;
                        if (curBarDict.ContainsKey(subKey))
                        {
                            if (singleTradePrice == JPR.NaN)
                            {
                                singleTradePrice = curBarDict[subKey].Open;
                            }
                            singleOffset = curBarDict[subKey].Offset;
                        }
                        if (singleTradePrice - singleOffset <= 0)
                        {
                            Log("当前处于涨跌停价格");
                            return "-1";
                        }

                        var subInfo = contractDict[subTradeContract];
                        double commission = subInfo.Fee > 1 ? subInfo.Fee
                            : subInfo.Fee * (singleTradePrice - singleOffset) * subInfo.VolumeMultiple;
                        Trades rspTrade = new Trades()
                        {
                            Contract = subTradeContract,
                            TradedVolume = openNum * tradeVolList[i],
                            TradedPrice = singleTradePrice,
                            ESide = ((side == EnumSide.Buy && tradeSideList[i] == EnumSide.Buy)
                                || (side == EnumSide.Sell && tradeSideList[i] == EnumSide.Sell))
                                ? EnumSide.Buy : EnumSide.Sell,
                            Fee = commission * openNum * tradeVolList[i],
                            StrategyId = Setting.StrategyName,
                            TradedTime = createTime,
                            SlippageFee = subInfo.PriceTick * subInfo.VolumeMultiple * openNum * tradeVolList[i],
                            TradeDay = tradeDay,
                            Offset = singleOffset
                        };
                        curTradeList.Add(rspTrade);
                    }

                    Orders rspOrder = new Orders()
                    {
                        OrderId = clientOrderId,
                        StrategyId = report.StrategyId,
                        UserId = report.UserId,
                        Contract = contract,
                        ESide = side,
                        EOrderStatus = EnumOrderStatus.AllTraded,
                        InsertPrice = tradePrice,
                        InsertVolume = volume,
                        InsertTime = tradedBar.RealDateTime,
                        TradedPrice = tradePrice,
                        TradedVolume = volume,
                        TradedTime = tradedBar.RealDateTime,
                        TradeDay = tradeDay
                    };
                    strategyRspOrderList.Add(rspOrder);
                    RtnOrder(rspOrder);
                    double curCommission = 0;
                    double curSlippageFee = 0;
                    double curLongMarketValue = 0;
                    double curShortMarketValue = 0;
                    double curUsedMargin = 0;
                    foreach (var trade in curTradeList)
                    {
                        curCommission += trade.Fee;
                        curSlippageFee += trade.SlippageFee;
                        double marketValue1 = trade.TradedPrice * trade.TradedVolume
                                * contractDict[trade.Contract].VolumeMultiple;
                        double marketValue2 = (trade.TradedPrice - trade.Offset) * trade.TradedVolume
                                * contractDict[trade.Contract].VolumeMultiple;
                        if (trade.ESide == EnumSide.Buy)
                            curLongMarketValue += marketValue1;
                        else
                            curShortMarketValue += marketValue1;
                        curUsedMargin +=marketValue2 * 0.1;
                        strategyRspTradeList.Add(trade);
                        RtnTrade(trade);
                    }
                    // 单笔成交市值
                    var unitLongMarketValue = curLongMarketValue / rspOrder.TradedVolume;
                    var unitShortMarketValue = curShortMarketValue / rspOrder.TradedVolume;
                    var unitCommission = curCommission / rspOrder.TradedVolume;
                    var unitSlippageFee = curSlippageFee / rspOrder.TradedVolume;
                    var unitUsedMargin = curUsedMargin / rspOrder.TradedVolume;
                    positionDict[contract].Fee += curCommission;
                    positionDict[contract].SlippageFee += curSlippageFee;

                    for (int i = 0; i < rspOrder.TradedVolume; i++)
                    {
                        if (rspOrder.ESide == EnumSide.Buy)
                        {
                            positionDict[contract].LongVolume += 1;
                            positionDict[contract].LongUnitMarketValues += string.Format(",{0}:{1}", unitLongMarketValue, unitShortMarketValue);
                        }
                        else
                        {
                            positionDict[contract].ShortVolume += 1;
                            positionDict[contract].ShortUnitMarketValues += string.Format(",{0}:{1}", unitLongMarketValue, unitShortMarketValue);
                        }

                        positionDict[contract].LongMarketValue += unitLongMarketValue;
                        positionDict[contract].ShortMarketValue += unitShortMarketValue;

                        if (positionDict[contract].NetVolume != 0)
                        {
                            if ((positionDict[contract].NetVolume > 0 && rspOrder.ESide == EnumSide.Buy) ||
                                (positionDict[contract].NetVolume < 0 && rspOrder.ESide == EnumSide.Sell))
                            {
                                positionDict[contract].UsedMargin += unitUsedMargin;
                            }
                            else
                            {
                                positionDict[contract].UsedMargin -= unitUsedMargin;
                            }
                        }
                        else
                        {
                            positionDict[contract].UsedMargin = 0;
                        }
                        report.TotalFee += unitCommission;
                        report.TotalSlippageFee += unitSlippageFee;
                        report.MaxUsedMargin = Math.Max(positionDict.Sum(d => d.Value.UsedMargin), report.MaxUsedMargin);
                        report.MaxMarketValue = Math.Max(positionDict.Sum(d => Math.Abs(d.Value.LongMarketValue - d.Value.ShortMarketValue)), report.MaxMarketValue);
                        //计算逐笔报告
                        CalculateOnePairProfit(contract);
                    }
                    Log(string.Format("以{0}{1}{2}手{3}[{4}]", tradePrice, side == EnumSide.Buy ? "买入" : "卖出", openNum, contract, createTime));
                    return rspOrder.OrderId;
                }
                catch (Exception ex)
                {
                    Log("SendOrder", ex);
                }
                return "-1";
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="isError">是否错误</param>
        public void Log(string msg, bool isError = false)
        {
            if (OnRtnLogEvent != null)
                OnRtnLogEvent(msg, isError);
        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="ex">错误</param>
        public void Log(string title, Exception ex)
        {
            if (OnRtnErrorEvent != null)
                OnRtnErrorEvent(title, ex);
        }

        /// <summary>
        /// 获取策略报告
        /// </summary>
        /// <returns></returns>
        public Reports GetReport()
        {
            lock (report)
            {
                return report.Clone();
            }
        }
        /// <summary>
        /// 获取持仓副本（当有新的买卖时请重新获取持仓信息）
        /// </summary>
        /// <returns></returns>
        public Positions GetPosition(string contract)
        {
            lock (positionDict)
            {
                return positionDict[contract].Clone();
            }
        }

        /// <summary>
        /// 获取合约信息
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        public Contracts GetContract(string contract)
        {
            if (contractDict.ContainsKey(contract))
                return contractDict[contract];
            else
                return null;
        }

        /// <summary>
        /// 转换参数
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public string[] ConvertParams(object sender)
        {
            return sender.ToString().Split(',');
        }

        /// <summary>
        /// 初始化数据集
        /// </summary>
        public void InitDBFields()
        {

        }

        /// <summary>
        /// 保存数据库字段
        /// </summary>
        public void SaveDBFields()
        {
            
        }


        public bool CreateMockBarDatas(string barContracts, string start, string end, List<BarData> cacheBarData)
        {
            return true;
        }

        /// <summary>
        /// 创建模拟数据
        /// </summary>
        /// <param name="barContracts">合约</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="fileName">文件路径</param>
        /// <param name="cacheBarData">缓存数据</param>
        /// <returns></returns>
        public bool CreateMockBarDatas(string barContracts, string start, string end, string fileName, List<BarData> cacheBarData)
        {
            var result = false;
            if (cacheBarData == null)
            {
                string fullName = AppDomain.CurrentDomain.BaseDirectory + "/data/" + fileName;
                if (File.Exists(fullName))
                {
                    StreamReader sr = new StreamReader(fullName);
                    var line = "";
                    int i = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (i > 0)
                        {
                            line = line.Replace("\"", "");
                            var values = line.Split(',');
                            try
                            {
                                DateTime createTime = DateTime.Parse(values[9]);
                                BarData data = new BarData()
                                {
                                    Contract = values[0],
                                    Cycle = values[1],
                                    Open = double.Parse(values[2]),
                                    High = double.Parse(values[3]),
                                    Low = double.Parse(values[4]),
                                    Close = double.Parse(values[5]),
                                    Volume = int.Parse(values[6]),
                                    OpenInterest = double.Parse(values[7]),
                                    Amount = double.Parse(values[8]),
                                    UpdateTime = createTime.ToString("HH:mm:00"),
                                    RealDay = createTime.ToString("yyyyMMdd"),
                                    TradingDay = values[10],
                                    Offset = double.Parse(values[11])
                                };
                                if (data.Cycle == "1D")
                                {
                                    data.UpdateTime = "23:59:59";
                                }
                                totalCsvDataList.Add(data);
                                if (createTime >= DateTime.Parse(start) && createTime < DateTime.Parse(end))
                                {
                                    testBarDataList.Add(data);
                                }
                            }
                            catch (Exception ex)
                            {
                                Log("CreateMockBarDatas", ex);
                                break;
                            }
                        }
                        i++;
                    }
                    startTimeDict.Add(testBarDataList[0].Contract, testBarDataList[0].RealDateTime);
                    sr.Close();
                    result = true;
                }
                else
                {
                    Log("读取文件失败");
                    result = false;
                }
            }
            else
            {
                testBarDataList.AddRange(cacheBarData.OrderBy(d => d.RealDateTime));
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 获取所有测试数据
        /// </summary>
        /// <returns></returns>
        public List<BarData> GetTestBarDatas()
        {
            return testBarDataList;
        }

        /// <summary>
        /// 获取前根Bar
        /// </summary>
        /// <param name="contract">合约</param>
        /// <param name="cycle">周期</param>
        /// <param name="num">前面第几根(0:前一根,1:前前根)</param>
        /// <returns></returns>
        public BarData GetPreData(string contract, string cycle, int num = 0)
        {
            string key = string.Format("{0}_{1}", cycle, contract);
            if (preBarsDict.ContainsKey(key) && preBarsDict[key].Count > num)
            {
                int index = preBarsDict[key].Count - (1 + num);
                return preBarsDict[key][index];
            }
            return null;
        }

        /// <summary>
        /// 加载Bar数据
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="cycle"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="isBarBaseOnTick"></param>
        /// <returns></returns>
        public bool LoadBarDatas(string contract, string cycle, string start, string end, bool isBarBaseOnTick)
        {
            return true;
        }

        /// <summary>
        /// 加载Bar数据
        /// </summary>
        /// <param name="contract">合约</param>
        /// <param name="cycle">周期</param>
        /// <param name="takeNumber">获取最近N条</param>
        /// <param name="isBarBaseOnTick">是否基于Tick动态生成Bar</param>
        /// <returns></returns>
        public bool LoadBarDatas(string contract, string cycle, int takeNumber, bool isBarBaseOnTick)
        {
            try
            {
                var realCycle = StringHelper.ComposeCycle(cycle);
                takeNumber = StringHelper.GetTakeNumber(cycle, realCycle, takeNumber);
                //BarOracleService barService = new BarOracleService();
                //barService.OnRtnErrorEvent += Log;
                //barService.OnRtnLogEvent += Log;
                LoadDataCount--;
                
                List<BarData> resultDatas = null;
                //var key2 = string.Format("{0}_{1}", cycle, contract);
                DateTime startTime = DateTime.Now;
                if (startTimeDict.ContainsKey(contract))
                {
                    startTime = startTimeDict[contract];
                }
                else
                {
                    startTime = startTimeDict[Setting.Contracts[0]];
                }
                if (totalCsvDataList.Count>0)
                {
                    resultDatas = totalCsvDataList.FindAll(d => d.Contract.Equals(contract) && d.Cycle.Equals(cycle)
                        && d.RealDateTime < startTime).Take(takeNumber).ToList();
                }
                else
                {
                    // 数据库中获取数据
                }
                if (resultDatas == null)
                {
                    Log("预加载数据错误", true);
                    FinishedLoadData(contract, cycle, null, true);
                    return false;
                }
                var preDatas = resultDatas.OrderByDescending(d => d.RealDateTime).Take(10);
                string key = string.Format("{0}_{1}", cycle, contract);
                if (!preBarsDict.ContainsKey(key))
                {
                    preBarsDict.Add(key, new List<BarData>());
                }
                preBarsDict[key].AddRange(preDatas.OrderBy(d => d.RealDateTime));
                FinishedLoadData(contract, cycle, resultDatas, LoadDataCount == 0);
                
                return true;
            }
            catch (Exception ex)
            {
                Log("加载数据错误", ex);
                return false;
            }
        }

        public virtual void FinishedLoadData(string contract, string cycle, List<Model.BarData> barDatas, bool isLast)
        {

        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public bool InitSetting(string setting)
        {
            try
            {
                var values = setting.Split(',');
                Setting = new Settings()
                {
                    StrategyName = report.StrategyName,
                    Contracts = values[0].Split('|'),
                    Cycles = values[1].Split('|'),
                    InitMoney = double.Parse(values[2]),
                    MaxLongVolumeAllowed = int.Parse(values[2]),     //最大允许多头
                    MaxShortVolumeAllowed = int.Parse(values[3])     //最大允许空头
                };
                report.InitRight = report.MaxRight = Setting.InitMoney;
                SetMinCycle(Setting.Cycles);
                ContractService contractService = new ContractService();
                contractService.OnRtnErrorEvent += Log;
                contractService.OnRtnLogEvent += Log;
                var contractList = contractService.GetContracts(Setting.Contracts);
                foreach(var cInfo in contractList)
                {
                    contractDict.Add(cInfo.Contract, cInfo);
                }
                return true;
            }
            catch (Exception ex)
            {
                Log("初始化配置失败", ex);
                return false;
            }
        }

        /// <summary>
        /// 初始化策略
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public virtual bool InitStrategy(object sender)
        {
            return true;
        }

        /// <summary>
        /// 启动策略
        /// </summary>
        /// <returns></returns>
        public bool StartStrategy()
        {
            try
            {
                foreach (var sc in Setting.Contracts)
                {
                    positionDict.Add(sc, new Positions()
                    {
                        StrategyId = report.StrategyId,
                        UserId = report.UserId,
                        Contract = sc
                    });
                    preBarsDict.Add(sc, new List<BarData>());
                }
                Thread threadBarTest = new Thread(BeginBarTest);
                threadBarTest.Start();
                return true;
            }
            catch(Exception ex)
            {
                Log("StartStrategy", ex);
                return false;
            }
        }

        /// <summary>
        /// 买入
        /// </summary>
        /// <param name="contract">合约</param>
        /// <param name="number">数量</param>
        /// <returns></returns>
        public string Buy(string contract, int number)
        {
            var clientOrderId = Guid.NewGuid().GetHashCode().ToString();
            pendingOrderList.Add(string.Format("{0},{1},{2},{3},{4}", clientOrderId, contract, "", 1, number));
            return clientOrderId;
        }

        /// <summary>
        /// 卖出
        /// </summary>
        /// <param name="contract">合约</param>
        /// <param name="number">数量</param>
        /// <returns></returns>
        public string Sell(string contract, int number)
        {
            var clientOrderId = Guid.NewGuid().GetHashCode().ToString();
            pendingOrderList.Add(string.Format("{0},{1},{2},{3},{4}", clientOrderId, contract, "", 2, number));
            return clientOrderId;
        }

        /// <summary>
        /// 撤销订单
        /// </summary>
        /// <param name="clientOrderId"></param>
        /// <returns></returns>
        public bool Cancel(string clientOrderId)
        {
            return true;
        }

        public virtual void RtnTickData(TickData tickData)
        {

        }

        public virtual void RtnBarData(BarData barData, bool isNewBar)
        {

        }

        public virtual void RtnOrder(Orders order)
        {

        }

        public virtual void RtnTrade(Trades trade)
        {

        }

        public virtual void RtnMessage(Messages msg)
        {

        }
        /// <summary>
        /// 停止策略
        /// </summary>
        public void StopStrategy()
        {
            contractDict.Clear();
            pendingOrderList.Clear();
            positionDict.Clear();
            testBarDataList.Clear();
            dailyMoneyList.Clear();
            totalCsvDataList.Clear();
            strategyRspTradeList.Clear();
            strategyRspOrderList.Clear();
            preBarsDict.Clear();
            curBarDict.Clear();
        }

        #endregion
    }
}
