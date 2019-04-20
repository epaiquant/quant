using EPI.CSharp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids.Functions
{
    public class AverageE : BaseFunction
    {
        private int _length;
        private double factor;
        private double preLastValue;
        private double lastValue;

        public AverageE()
        {
            Name = "AverageE";
            Description = "指数求平均";
            lastValue = JPR.NaN;
        }

        public void SetParameters(int length)
        {
            _length = length;
            factor = 2d / (length + 1);
            lastValue = JPR.NaN;
        }

        public double Caculate(double data)
        {
            double result;
            if (lastValue == JPR.NaN)
            {
                preLastValue = lastValue = data;
                result = lastValue;
            }
            else
            {
                preLastValue = lastValue;
                lastValue = lastValue + factor * (data - lastValue);
                result = lastValue;
            }
            return result;
        }

        public void ResetValue()
        {
            lastValue = preLastValue;
        }

        public int Length
        {
            get { return _length; }
        }
    }
}
