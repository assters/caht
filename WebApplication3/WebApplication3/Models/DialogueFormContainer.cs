using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.Models;
using WebApplication3.SecurityUtilities;

namespace WebApplication3.Models
{
    public class DialogueFormContainer
    {
        public string ownerName { get; set; }
        public string partnerName { get; set; }
        public IEnumerable<string> Users { get; set; }
        public List<Message> Story { get; set; }
}
}
