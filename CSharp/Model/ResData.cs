using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPI.CSharp.Model
{
    public class ResData
    {
        public bool status { get; set; }
        public int code { get; set; }
        public string msg { get; set; }

        public object result { get; set; }
    }
}
