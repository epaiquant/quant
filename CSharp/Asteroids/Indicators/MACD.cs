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
    public class MACD : BaseIndicator
    {
        public enum EnumValueType
        {
            MACDValue = 0,
            AvgMACD = 1,
            MACDDiff = 2,
        }

        private int _fastLength;
        private int _slowLength;
        private AverageE fastEMA;
        private AverageE slowEMA;
        private double _macdValue;
        private int _macdLength;
        private AverageE macdEMA;
        private double _macdAvg;
        private double _macdDiff;

        public MACD(List<BarData> barDatas, int fastLength = 12, int slowLength = 26, int macdLength = 9, bool isSimpleMode = true, bool isShowInMain = false, string tag = "1")
            : base(barDatas)
        {
            Tag = tag;
            _fastLength = fastLength;
            _slowLength = slowLength;
            _macdLength = macdLength;
            IsSimpleMode = isSimpleMode;
            string paramTag = string.Format("({0},{1},{2})", _fastLength, _slowLength, _macdLength);
            Name = string.Format("MACD{0}", paramTag);
            Description = "指数平滑异同平均线";
            valueDict.Add(EnumValueType.MACDValue.ToString(), new List<double>());
            valueDict.Add(EnumValueType.AvgMACD.ToString(), new List<double>());
            valueDict.Add(EnumValueType.MACDDiff.ToString(), new List<double>());
            if (!IsSimpleMode)
            {
                graphDict.Add(EnumValueType.MACDValue.ToString(), new IndicatorGraph() { Name = EnumValueType.MACDValue.ToString(), Tag = paramTag, LineStyle = EnumLineStyle.SolidLine });
                graphDict.Add(EnumValueType.AvgMACD.ToString(), new IndicatorGraph() { Name = EnumValueType.AvgMACD.ToString(), Tag = paramTag, LineStyle = EnumLineStyle.SolidLine });
                graphDict.Add(EnumValueType.MACDDiff.ToString(), new IndicatorGraph() { Name = EnumValueType.MACDDiff.ToString(), Tag = paramTag, LineStyle = EnumLineStyle.Steam });
            }
            fastEMA = new AverageE();
            slowEMA = new AverageE();
            macdEMA = new AverageE();
            IsShowInMain = isShowInMain;
            Caculate();
        }

        public void SetParameters(int fastLength, int slowLength, int macdLength)
        {
            if (fastLength != _fastLength || slowLength != _slowLength || macdLength != _macdLength)
            {
                string paramTag = string.Format("({0},{1},{2})", fastLength, slowLength, macdLength);
                Name = string.Format("MACD{0}", paramTag);
                if (!IsSimpleMode)
                {
                    graphDict[EnumValueType.MACDValue.ToString()].Tag = paramTag;
                    graphDict[EnumValueType.AvgMACD.ToString()].Tag = paramTag;
                    graphDict[EnumValueType.MACDDiff.ToString()].Tag = paramTag;
                }
                _fastLength = fastLength;
                _slowLength = slowLength;
                _macdLength = macdLength;
                Caculate();
            }
        }

        protected override void Caculate()
        {
            valueDict[EnumValueType.MACDValue.ToString()].Clear();
            valueDict[EnumValueType.AvgMACD.ToString()].Clear();
            valueDict[EnumValueType.MACDDiff.ToString()].Clear();
            if (!IsSimpleMode)
            {
                graphDict[EnumValueType.MACDValue.ToString()].Clear();
                graphDict[EnumValueType.AvgMACD.ToString()].Clear();
                graphDict[EnumValueType.MACDDiff.ToString()].Clear();
            }
            fastEMA.SetParameters(_fastLength);
            slowEMA.SetParameters(_slowLength);
            macdEMA.SetParameters(_macdLength);
            if (barDatas != null && Count != 0)
            {
                for (int i = 0; i < Count; i++)
                {
                    GenerateMACD(i);
                }
            }
            base.Caculate();
        }

        private void GenerateMACD(int i)
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
            _macdValue = fastEMA.Caculate(curData.Close) - slowEMA.Caculate(curData.Close);
            _macdAvg = macdEMA.Caculate(_macdValue);
            valueDict[EnumValueType.MACDValue.ToString()].Add(_macdValue);
            valueDict[EnumValueType.AvgMACD.ToString()].Add(_macdAvg);
            if (!IsSimpleMode)
            {
                graphDict[EnumValueType.MACDValue.ToString()].AddValue(_macdValue, Color.White);
                graphDict[EnumValueType.AvgMACD.ToString()].AddValue(_macdAvg, Color.Yellow);
            }
            _macdDiff = (_macdValue - _macdAvg) * 2;
            if (_macdDiff >= 0)
            {
                valueDict[EnumValueType.MACDDiff.ToString()].Add(_macdDiff);
                if (!IsSimpleMode)
                {
                    graphDict[EnumValueType.MACDDiff.ToString()].AddValue(_macdDiff, Color.Red);
                }
            }
            else if (_macdDiff < 0)
            {
                valueDict[EnumValueType.MACDDiff.ToString()].Add(_macdDiff);
                if (!IsSimpleMode)
                {
                    graphDict[EnumValueType.MACDDiff.ToString()].AddValue(_macdDiff, Color.Cyan);
                }
            }
        }

        public override void UpdateBarData(BarData bar)
        {
            base.UpdateBarData(bar);
            valueDict[EnumValueType.MACDValue.ToString()].RemoveAt(Count - 1);
            valueDict[EnumValueType.AvgMACD.ToString()].RemoveAt(Count - 1);
            valueDict[EnumValueType.MACDDiff.ToString()].RemoveAt(Count - 1);
            if (!IsSimpleMode)
            {
                graphDict[EnumValueType.MACDValue.ToString()].RemoveLast();
                graphDict[EnumValueType.AvgMACD.ToString()].RemoveLast();
                graphDict[EnumValueType.MACDDiff.ToString()].RemoveLast();
            }
            fastEMA.ResetValue();
            slowEMA.ResetValue();
            macdEMA.ResetValue();
            GenerateMACD(Count - 1);


        }

        public override void AddBarData(BarData bar)
        {
            base.AddBarData(bar);
            GenerateMACD(Count - 1);


        }

        public List<double> GetValues(EnumValueType valueType)
        {
            return valueDict[valueType.ToString()];
        }

        public double GetMACDValue(int index)
        {
            if (index >= 0 && index < Count)
                return valueDict[EnumValueType.MACDValue.ToString()][index];
            else
                return JPR.NaN;
        }

        public double GetAvgValue(int index)
        {
            if (index >= 0 && index < Count)
                return valueDict[EnumValueType.AvgMACD.ToString()][index];
            else
                return JPR.NaN;
        }

        public double GetDiffValue(int index)
        {
            if (index >= 0 && index < Count)
                return valueDict[EnumValueType.MACDDiff.ToString()][index];
            else
                return JPR.NaN;
        }

        public double GetLast(EnumValueType valueType)
        {
            if (Count != 0)
                return valueDict[valueType.ToString()][Count - 1];
            else
                return JPR.NaN;
        }

        public int FastLength
        {
            get { return _fastLength; }
        }
        public int SlowLength
        {
            get { return _slowLength; }
        }
        public int MacdLength
        {
            get { return _macdLength; }
        }
    }
}
