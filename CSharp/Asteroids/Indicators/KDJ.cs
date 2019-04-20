using Asteroids.Functions;
using EPI.CSharp.Model;
using EPI.CSharp.Model.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids.Indicators
{
    public class KDJ : BaseIndicator
    {
        private int _length;
        private int _ma1;
        private int _ma2;
        private MaxValue HH;
        private double HighestH;
        private MinValue LL;
        private double LowestL;
        private double Rsv;
        private double preRsv;
        private double preK;
        private double thisK;
        private List<double> KList;
        private double preD;
        private double thisD;
        private List<double> DList;
        private double thisJ;
        private bool init;
        private bool initK;

        public KDJ(List<BarData> bars, int length = 9, int ma1 = 3, int ma2 = 3, bool isSimpleMode = true, bool isShowInMain = false, string tag = "1")
            : base(bars)
        {
            Tag = tag;
            IsSimpleMode = isSimpleMode;
            _length = length;
            _ma1 = ma1;
            _ma2 = ma2;
            string paramTag = string.Format("({0},{1},{2})", _length, _ma1, _ma2);
            Name = string.Format("KDJ{0}", paramTag);
            Description = "随机指标";
            valueDict.Add("K", new List<double>());
            valueDict.Add("D", new List<double>());
            valueDict.Add("J", new List<double>());
            if (!IsSimpleMode)
            {
                graphDict.Add("K", new IndicatorGraph() { Name = "K", Tag = paramTag, LineStyle = EnumLineStyle.SolidLine });
                graphDict.Add("D", new IndicatorGraph() { Name = "D", Tag = paramTag, LineStyle = EnumLineStyle.SolidLine });
                graphDict.Add("J", new IndicatorGraph() { Name = "J", Tag = paramTag, LineStyle = EnumLineStyle.SolidLine });
            }

            HH = new MaxValue();
            LL = new MinValue();
            KList = new List<double>();
            DList = new List<double>();
            IsShowInMain = isShowInMain;
            Caculate();
        }

        public void SetParameters(int length, int ma1, int ma2)
        {
            if (length != _length || ma1 != _ma1 || ma2 != _ma2)
            {
                string paramTag = string.Format("({0},{1},{2})", length, ma1, ma2);
                Name = string.Format("KDJ{0}", paramTag);
                if (!IsSimpleMode)
                {
                    graphDict["K"].Tag = paramTag;
                    graphDict["D"].Tag = paramTag;
                    graphDict["J"].Tag = paramTag;
                }
                _length = length;
                _ma1 = ma1;
                _ma2 = ma2;
                Caculate();
            }
        }

        protected override void Caculate()
        {
            valueDict["K"].Clear();
            valueDict["D"].Clear();
            valueDict["J"].Clear();
            if (!IsSimpleMode)
            {
                graphDict["K"].Clear();
                graphDict["D"].Clear();
                graphDict["J"].Clear();
            }
            preRsv = 0.5;
            HH.SetParameters(_length);
            LL.SetParameters(_length);
            preK = JPR.NaN;
            thisK = JPR.NaN;
            KList.Clear();
            preD = JPR.NaN;
            thisD = JPR.NaN;
            DList.Clear();
            thisJ = JPR.NaN;
            init = false;
            initK = false;
            if (barDatas != null && Count != 0)
            {
                for (int i = 0; i < Count; i++)
                {
                    GenerateKDJ(i);
                }
            }
            base.Caculate();
        }

        private void GenerateKDJ(int i)
        {
            BarData curData = null;
            if (i >= barDatas.Count)
            {
                curData = GetBarData(0);
            }
            else
            {
                curData = barDatas[i];
            }
            HighestH = HH.AddValue(curData.High);
            LowestL = LL.AddValue(curData.Low);
            if (HighestH - LowestL == 0)
            {
                Rsv = preRsv;
                valueDict["K"].Add(JPR.NaN);
                valueDict["D"].Add(JPR.NaN);
                valueDict["J"].Add(JPR.NaN);
                if (!IsSimpleMode)
                {
                    graphDict["K"].AddValue(JPR.NaN, Color.White);
                    graphDict["D"].AddValue(JPR.NaN, Color.Yellow);
                    graphDict["J"].AddValue(JPR.NaN, Color.Magenta);
                }
            }
            else
            {
                Rsv = 100 * (curData.Close - LowestL) / (HighestH - LowestL);
                if (!init)
                {
                    if (thisK != JPR.NaN)
                    {
                        preK = thisK;
                        thisK = preK * (_ma1 - 1) / _ma1 + Rsv * 1 / _ma1;
                        if (DList.Count < _ma2)
                        { DList.Add(thisK); }
                    }
                    if (!initK)
                    {
                        if (KList.Count < _ma1)
                        { KList.Add(Rsv); }
                        if (KList.Count == _ma1)
                        {
                            initK = true;
                            thisK = KList.Average();
                            DList.Add(thisK);
                        }
                    }
                    if (DList.Count > 0 && DList.Count == _ma2)
                    {
                        thisD = DList.Average();
                        init = true;
                    }
                    Rsv = JPR.NaN;
                    valueDict["K"].Add(JPR.NaN);
                    valueDict["D"].Add(JPR.NaN);
                    valueDict["J"].Add(JPR.NaN);
                    if (!IsSimpleMode)
                    {
                        graphDict["K"].AddValue(JPR.NaN, Color.White);
                        graphDict["D"].AddValue(JPR.NaN, Color.Yellow);
                        graphDict["J"].AddValue(JPR.NaN, Color.Magenta);
                    }
                }
                else
                {
                    preK = thisK;
                    thisK = preK * (_ma1 - 1) / _ma1 + Rsv * 1 / _ma1;
                    preD = thisD;
                    thisD = preD * (_ma2 - 1) / _ma2 + thisK * 1 / _ma2;
                    thisJ = 3 * thisK - 2 * thisD;
                    valueDict["K"].Add(thisK);
                    valueDict["D"].Add(thisD);
                    valueDict["J"].Add(thisJ);
                    if (!IsSimpleMode)
                    {
                        graphDict["K"].AddValue(thisK, Color.White);
                        graphDict["D"].AddValue(thisD, Color.Yellow);
                        graphDict["J"].AddValue(thisJ, Color.Magenta);
                    }
                }
            }
            preRsv = Rsv;
        }

        public override void UpdateBarData(BarData bar)
        {
            base.UpdateBarData(bar);
            if (valueDict["K"].Count == Count)
            {
                valueDict["K"].RemoveAt(Count - 1);
                if (!IsSimpleMode)
                {
                    graphDict["K"].RemoveLast();
                }
            }
            if (valueDict["D"].Count == Count)
            {
                valueDict["D"].RemoveAt(Count - 1);
                if (!IsSimpleMode)
                {
                    graphDict["D"].RemoveLast();
                }
            }
            if (valueDict["J"].Count == Count)
            {
                valueDict["J"].RemoveAt(Count - 1);
                if (!IsSimpleMode)
                {
                    graphDict["J"].RemoveLast();
                }
            }
            if (KList.Count > 0)
            {
                KList.RemoveAt(KList.Count - 1);
            }
            if (DList.Count > 0)
            {
                DList.RemoveAt(DList.Count - 1);
            }
            HH.RemoveLast();
            LL.RemoveLast();
            thisK = preK;
            if (init)
            { thisD = preD; }

            GenerateKDJ(Count - 1);


        }

        public override void AddBarData(BarData bar)
        {
            base.AddBarData(bar);
            GenerateKDJ(Count - 1);


        }

        public List<double> GetKValues()
        {
            return valueDict["K"];
        }
        public List<double> GetDValues()
        {
            return valueDict["D"];
        }
        public List<double> GetJValues()
        {
            return valueDict["J"];
        }

        public double GetKValue(int index)
        {
            if (index >= 0 && index < Count)
                return valueDict["K"][index];
            else
                return JPR.NaN;
        }
        public double GetDValue(int index)
        {
            if (index >= 0 && index < Count)
                return valueDict["D"][index];
            else
                return JPR.NaN;
        }
        public double GetJValue(int index)
        {
            if (index >= 0 && index < Count)
                return valueDict["J"][index];
            else
                return JPR.NaN;
        }

        public double GetLastK()
        {
            if (Count != 0)
                return valueDict["K"][Count - 1];
            else
                return JPR.NaN;
        }
        public double GetLastD()
        {
            if (Count != 0)
                return valueDict["D"][Count - 1];
            else
                return JPR.NaN;
        }
        public double GetLastJ()
        {
            if (Count != 0)
                return valueDict["J"][Count - 1];
            else
                return JPR.NaN;
        }

        public int Length
        {
            get { return _length; }
        }
        public double Ma1
        {
            get { return _ma1; }
        }
        public double Ma2
        {
            get { return _ma2; }
        }
    }
}
