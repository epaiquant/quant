using EPI.CSharp.Model;
using EPI.CSharp.Model.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids.Indicators
{
    public class BaseIndicator
    {
        private int _count;
        private int _maxCacheCount = 10;
        protected List<BarData> barDatas;
        protected Dictionary<string, List<double>> valueDict;
        protected Dictionary<string, IndicatorGraph> graphDict;
        public BaseIndicator(List<BarData> bars)
        {
            barDatas = new List<BarData>();
            if (bars != null)
                barDatas.AddRange(bars);
            _count = barDatas.Count;
            valueDict = new Dictionary<string, List<double>>();
            graphDict = new Dictionary<string, IndicatorGraph>();
        }

        protected virtual void Caculate()
        {
            if (IsSimpleMode)
            {
                if (barDatas.Count > MaxCacheCount)
                {
                    barDatas.RemoveRange(0, barDatas.Count - MaxCacheCount);
                }
                graphDict.Clear();
                GC.Collect();
            }
        }

        #region 公共属性
        /// <summary>
        /// 指标名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 指标描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 是否在主图上显示
        /// </summary>
        public bool IsShowInMain { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// 精简模式
        /// </summary>
        public bool IsSimpleMode { get; set; }
        /// <summary>
        /// 数据字典
        /// </summary>
        public Dictionary<string, List<double>> ValueDict { get { return valueDict; } }
        /// <summary>
        /// 图形字典
        /// </summary>
        public Dictionary<string, IndicatorGraph> GraphDict { get { return graphDict; } }
        /// <summary>
        /// 数量
        /// </summary>
        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
            }
        }
        /// <summary>
        /// 缓存Bar数量
        /// </summary>
        public int MaxCacheCount
        {
            get { return _maxCacheCount; }
            set
            {
                _maxCacheCount = value;
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 添加参考线
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="color"></param>
        /// <param name="lineStyle"></param>
        public void AddGuidLine(string name, double value, Color color, EnumLineStyle lineStyle = EnumLineStyle.DotLine)
        {
            if (!graphDict.ContainsKey(name))
            {
                var graph = new IndicatorGraph() { Name = name, LineStyle = lineStyle };
                graph.AddValue(value, color);
                graphDict.Add(name, graph);
            }
        }

        /// <summary>
        /// 删除参考线
        /// </summary>
        /// <param name="name"></param>
        public void DelGuidLine(string name)
        {
            graphDict.Remove(name);
        }

        /// <summary>
        /// 根据索引获取Bar数据(0->最新一根，1->前一根,以此推类)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public BarData GetBarData(int index = 0)
        {
            if (index < Count)
                return barDatas[barDatas.Count - index - 1];
            else
                return null;
        }
        /// <summary>
        /// 获取最新缓存Bar数据
        /// </summary>
        /// <returns></returns>
        public List<BarData> GetBarDatas()
        {
            return barDatas;
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        /// <param name="datas"></param>
        public void BindData(List<BarData> datas)
        {
            barDatas.Clear();
            barDatas.AddRange(datas);
            Caculate();
        }

        /// <summary>
        /// 添加Bar数据至最后
        /// </summary>
        /// <param name="bar"></param>
        public virtual void AddBarData(BarData bar)
        {
            barDatas.Add(new BarData(bar));
            if (IsSimpleMode && barDatas.Count > MaxCacheCount)
            {
                barDatas.RemoveAt(0);
            }
            _count++;
        }
        /// <summary>
        /// 更新Bar数据
        /// </summary>
        /// <param name="bar"></param>
        /// <returns></returns>
        public virtual void UpdateBarData(BarData bar)
        {
            barDatas[barDatas.Count - 1] = new BarData(bar);
        }
        /// <summary>
        /// 插入Bar数据
        /// </summary>
        /// <param name="index"></param>
        /// <param name="bar"></param>
        public void InsertBar(int index, BarData bar)
        {
            if (index < barDatas.Count)
            {
                barDatas.Insert(index, new BarData(bar));
            }
            else
            {
                barDatas.Add(bar);
            }
            if (barDatas.Count > MaxCacheCount)
            {
                barDatas.RemoveAt(0);
            }
            _count++;
            Caculate();
        }
        /// <summary>
        /// 批量添加Bars
        /// </summary>
        public void AddBars(List<BarData> bars)
        {
            barDatas.AddRange(bars);
            _count += bars.Count;
            Caculate();
        }
        /// <summary>
        /// 批量插入Bars
        /// </summary>
        /// <param name="index"></param>
        /// <param name="bars"></param>
        public void InsertBars(int index, List<BarData> bars)
        {
            if (index < barDatas.Count)
            {
                if (bars[bars.Count - 1].RealDateTime == barDatas[index].RealDateTime)
                {
                    barDatas.RemoveAt(index);//删除第一根实时接收Bar数据
                    _count--;
                }
                _count += bars.Count;
                barDatas.InsertRange(index, bars);
                Caculate();
            }
        }

        /// <summary>
        /// 获取最新一组数据
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, double> GetLastValues()
        {
            Dictionary<string, double> resultDict = new Dictionary<string, double>();
            foreach (var vd in valueDict)
            {
                if (Count != 0)
                    resultDict.Add(vd.Key, vd.Value[Count - 1]);
                else
                    resultDict.Add(vd.Key, JPR.NaN);
            }
            return resultDict;
        }
        
        #endregion
    }
}
