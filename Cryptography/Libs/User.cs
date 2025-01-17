using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptography.Libs
{
    public class User
    {
        public string Name { get; set; }    
        public string Salt { get; set; }
        public string SaltedHashedPassword { get; set; }

        public string[]? Roles { get; set; }

        public User(string name, string salt, string saltedHashedPassword, string[]? roles) 
        { 
            Name = name;
            Salt = salt;
            SaltedHashedPassword = saltedHashedPassword;
            Roles = roles;
        }
    }
}
