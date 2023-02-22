using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.Models;
using WebApplication3.SecurityUtilities;

namespace WebApplication3.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string partnerName { get; set; }

        // Словарь: партнер чата и сообщения с ним
        public List<PartnerDictionary> PartnersDictionary { get; set; }
    }
}
