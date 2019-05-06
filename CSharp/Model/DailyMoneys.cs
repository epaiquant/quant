using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPI.CSharp.Model
{
    public class DailyMoneys
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string StrategyId { get; set; }

        public double Right { get; set; }

        public double Profit { get; set; }

        public double BaseYield { get; set; }

        public double CompoundYield { get; set; }

        public double NetValue { get; set; }

        public double Fee { get; set; }

        public int NetVolume { get; set; }

        public double UsedMargin { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
