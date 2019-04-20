using EPI.CSharp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids.Functions
{
    public class AverageS : BaseFunction
    {
        private int _length;
        private List<double> tmpList;

        public AverageS()
        {
            Name = "AverageS";
            Description = "简单求平均";
            tmpList = new List<double>();
        }

        public void SetParameters(int length)
        {
            _length = length;
            tmpList.Clear();
        }

        public void Clear()
        {
            tmpList.Clear();
        }

        public double AddValue(double data)
        {
            double result;
            if (tmpList.Count > 0 && tmpList.Count == _length)
            { tmpList.RemoveAt(0); }
            else if (tmpList.Count > _length)
            { throw new Exception("计算数量超出范围"); }
            tmpList.Add(data);
            if (tmpList.Count == _length)
            { result = tmpList.Average(); }
            else
            { result = JPR.NaN; }
            return result;
        }

        public void RemoveLast()
        {
            tmpList.RemoveAt(tmpList.Count - 1);
        }

        public int Length
        {
            get { return _length; }
        }

        public double REF(int refidx)
        {
            if (refidx >= 0 && refidx < _length && refidx < tmpList.Count)
            {
                return tmpList[tmpList.Count - refidx - 1];
            }
            else
                return JPR.NaN;
        }
    }
}
