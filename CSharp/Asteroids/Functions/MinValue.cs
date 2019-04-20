using EPI.CSharp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids.Functions
{
    public class MinValue : BaseFunction
    {
        private int _length;
        private List<double> tmpList;

        public MinValue()
        {
            Name = "MinValue";
            Description = "求最低值";
            tmpList = new List<double>();
        }

        public void SetParameters(int length)
        {
            _length = length;
            tmpList.Clear();
        }

        public double AddValue(double data)
        {
            double result;
            if (tmpList.Count == _length)
            { tmpList.RemoveAt(0); }
            else if (tmpList.Count > _length)
            { throw new Exception("计算数量超出范围"); }
            tmpList.Add(data);
            if (tmpList.Count == _length)
            { result = tmpList.Min(); }
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
    }
}
