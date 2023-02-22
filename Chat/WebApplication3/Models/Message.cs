using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string MessageText { get; set; }
        public DateTime TimeStamp { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}
