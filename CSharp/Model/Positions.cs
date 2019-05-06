using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPI.CSharp.Model
{
    public class Positions
    {
        public Positions Clone()
        {
            return (Positions)this.MemberwiseClone();
        }
        public int Id { get; set; }

        public int UserId { get; set; }

        public string StrategyId { get; set; }

        public string Contract { get; set; }

        public double LongPrice { get; set; }

        public int LongVolume { get; set; }

        public double ShortPrice { get; set; }

        public int ShortVolume { get; set; }
        public double LongMarketValue { get; set; }
        public double ShortMarketValue { get; set; }

        public double UsedMargin { get; set; }

        public double Fee { get; set; }

        public double SlippageFee { get; set; }
        public double LeftProfit { get; set; }

        public string LongUnitMarketValues { get; set; }

        public string ShortUnitMarketValues { get; set; }

        public int NetVolume { get { return LongVolume - ShortVolume; } }
    }
}
