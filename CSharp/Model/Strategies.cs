using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPI.CSharp.Model
{
    public class Strategies
    {
        public string StrategyId { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string StrategyParams { get; set; }

        public string StrategySettings { get; set; }

        public string StrategyWorker { get; set; }

        public string RealAccount { get; set; }

        public bool IsAuto { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
