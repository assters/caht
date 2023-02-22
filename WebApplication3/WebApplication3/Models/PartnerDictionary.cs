using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.Models;
using WebApplication3.SecurityUtilities;

namespace WebApplication3.Models
{
    public class PartnerDictionary
    {
        public int Id { get; set; }
        public string PartnerName { get; set; }
        public List<Message> Messages { get; set; }
    }
}
