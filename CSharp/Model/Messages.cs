using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPI.CSharp.Model
{
    public class Messages
    {
        public int Id { get; set; }

        public string OrderId { get; set; }

        public string MsgType { get; set; }

        public string MsgContent { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
