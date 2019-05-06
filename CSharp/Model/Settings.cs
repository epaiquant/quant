using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPI.CSharp.Model
{
    public class Settings
    {
        public string StrategyName { get; set; }

        public double InitMoney { get; set; }

        public int MaxLongVolumeAllowed { get; set; }

        public int MaxShortVolumeAllowed { get; set; }

        public string[] Contracts { get; set; }

        public string[] Cycles { get; set; }

    }
}
