using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.Models;
using WebApplication3.SecurityUtilities;

namespace WebApplication3
{
    public static class InitialUserData
    {
        public static void Initialize(DataBaseContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User
                    {
                        Name = "123",
                        Password = Hash.GetMd5Hash("123")
                    },
                    new User
                    {
                        Name = "222",
                        Password = Hash.GetMd5Hash("222")
                    },
                    new User
                    {
                        Name = "4",
                        Password = Hash.GetMd5Hash("4")
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
