using EPI.CSharp.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPI.CSharp.Model;
using Asteroids.Indicators;

namespace EPI.CSharp.Tests
{
    public class StrategyTest : BaseFastTest
    {
        KDJ kdj;

        public StrategyTest(int userId, string strategyId) : base(userId, strategyId)
        {

        }

        public override bool InitStrategy(object sender)
        {
            LoadDataCount = 1;
            return LoadBarDatas(Setting.Contracts[0], Setting.Cycles[0], 100, false);
        }

        public override void FinishedLoadData(string contract, string cycle, List<BarData> barDatas, bool isLast)
        {
            if (contract== Setting.Contracts[0] && cycle == Setting.Cycles[0])
            {
                kdj = new KDJ(barDatas, 9, 3, 3);
            }
            if (isLast)
            {
                StartStrategy();
            }
        }

        public override void RtnBarData(BarData barData, bool isNewBar)
        {
            kdj.AddBarData(barData);
            var postion = GetPosition(barData.Contract);
            bool isUp = CrossKdjUp(kdj);
            bool isDown = CrossKdjDown(kdj);
            if (postion.NetVolume<=0 && isUp)
            {
                Buy(barData.Contract, 1);
            }
            else if (postion.NetVolume>=0&& isDown)
                    {
                Sell(barData.Contract, 1);
            }
        }


        /// <summary>
        /// KDJ金叉
        /// </summary>
        /// <returns></returns>
        bool CrossKdjUp(KDJ kdj)
        {
            var preDValue = kdj.GetDValue(kdj.Count - 2);
            var preJValue = kdj.GetJValue(kdj.Count - 2);
            var dValue = kdj.GetDValue(kdj.Count - 1);
            var jValue = kdj.GetJValue(kdj.Count - 1);
            if (!JPR.IsNaN(preDValue) && !JPR.IsNaN(preJValue) && !JPR.IsNaN(dValue) && !JPR.IsNaN(jValue))
            {
                return preJValue < preDValue && jValue > dValue;
            }
            return false;
        }
        /// <summary>
        /// KDJ死叉
        /// </summary>
        /// <returns></returns>
        bool CrossKdjDown(KDJ kdj)
        {
            var preDValue = kdj.GetDValue(kdj.Count - 2);
            var preJValue = kdj.GetJValue(kdj.Count - 2);
            var dValue = kdj.GetDValue(kdj.Count - 1);
            var jValue = kdj.GetJValue(kdj.Count - 1);
            if (!JPR.IsNaN(preDValue) && !JPR.IsNaN(preJValue) && !JPR.IsNaN(dValue) && !JPR.IsNaN(jValue))
            {
                return preJValue > preDValue && jValue < dValue;
            }
            return false;
        }
    }
}
