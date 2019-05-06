using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPI.CSharp.Model
{
    public class TickData
    {
        public string Contract { get; set; }

        public double LastPrice { get; set; }

        public double AskPrice1 { get; set; }

        public int AskVolume1 { get; set; }

        public double BidPrice { get; set; }

        public int BidVolume { get; set; }

        public int Volume { get; set; }

        public double Amount { get; set; }

        public double OpenInst { get; set; }

        public DateTime RealTime { get; set; }

        public string TradeDay { get; set; }
    }
}
