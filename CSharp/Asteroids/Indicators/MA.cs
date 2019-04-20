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
    public class MA : BaseIndicator
    {
        private EnumBarStruct barStruct;
        private int _length;
        private AverageS averageS;

        public MA(List<BarData> barDatas, EnumBarStruct objBarStruct = EnumBarStruct.Close,
            int length = 10, bool isSimpleMode = true, bool isShowInMain = true, string tag = "1")
            : base(barDatas)
        {
            Tag = tag;
            _length = length;
            barStruct = objBarStruct;
            IsSimpleMode = isSimpleMode;
            string paramTag = string.Format("({0},{1})", barStruct.ToString(), _length);
            Name = string.Format("MA{0}", paramTag);
            Description = "移动平均";
            valueDict.Add("MA", new List<double>());
            if (!IsSimpleMode)
            {
                graphDict.Add("MA", new IndicatorGraph() { Name = "MA", Tag = paramTag, LineStyle = EnumLineStyle.SolidLine });
            }
            averageS = new AverageS();
            IsShowInMain = isShowInMain;
            Caculate();
        }

        public void SetParameters(EnumBarStruct objBarStruct, int length)
        {
            if (objBarStruct != barStruct || length != _length)
            {
                string paramTag = string.Format("({0},{1})", objBarStruct.ToString(), length);
                Name = string.Format("MA{0}", paramTag);
                if (!IsSimpleMode)
                {
                    graphDict["MA"].Tag = paramTag;
                }
                barStruct = objBarStruct;
                _length = length;
                Caculate();
            }
        }

        protected override void Caculate()
        {
            valueDict["MA"].Clear();
            if (!IsSimpleMode)
            {
                graphDict["MA"].Clear();
            }
            averageS.SetParameters(_length);
            if (barDatas != null && Count != 0)
            {
                for (int i = 0; i < Count; i++)
                {
                    GenerateSMA(i);
                }
            }
            base.Caculate();
        }

        private void GenerateSMA(int i)
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
            double value = averageS.AddValue(GetData(curData));
            valueDict["MA"].Add(value);
            if (!IsSimpleMode)
            {
                graphDict["MA"].AddValue(value, Color.Yellow);
            }
        }

        public override void UpdateBarData(BarData bar)
        {
            base.UpdateBarData(bar);
            valueDict["MA"].RemoveAt(Count - 1);
            if (!IsSimpleMode)
            {
                graphDict["MA"].RemoveLast();
            }
            averageS.RemoveLast();
            GenerateSMA(Count - 1);


        }

        public override void AddBarData(BarData bar)
        {
            base.AddBarData(bar);
            GenerateSMA(Count - 1);
        }

        private double GetData(BarData bar)
        {
            double data;
            switch (barStruct)
            {
                case EnumBarStruct.Open:
                    data = bar.Open;
                    break;
                case EnumBarStruct.High:
                    data = bar.High;
                    break;
                case EnumBarStruct.Low:
                    data = bar.Low;
                    break;
                case EnumBarStruct.Close:
                    data = bar.Close;
                    break;
                case EnumBarStruct.Volume:
                    data = bar.Volume;
                    break;
                case EnumBarStruct.OpenInterest:
                    data = bar.OpenInterest;
                    break;
                case EnumBarStruct.Amount:
                    data = bar.Amount;
                    break;
                default:
                    data = 0;
                    break;
            }
            return data;
        }

        public List<double> GetValues()
        {
            return valueDict["MA"];
        }

        public double GetValue(int index)
        {
            if (index >= 0 && index < Count)
                return valueDict["MA"][index];
            else
                return JPR.NaN;
        }

        public double GetLast()
        {
            if (Count != 0)
                return valueDict["MA"][Count - 1];
            else
                return JPR.NaN;
        }

        public int Length
        {
            get { return _length; }
        }

        public int DataType
        {
            get { return barStruct.GetHashCode(); }
        }
    }
}
