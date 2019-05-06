using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPI.CSharp.Model
{
    public class Contracts
    {
        public string Contract { get; set; }

        public string Category { get; set; }

        public double PriceTick { get; set; }

        public double VolumeMultiple { get; set; }

        public double Fee { get; set; }

        public double Margin { get; set; }
    }
}
