using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Services.Smsc
{
    public abstract class AbstractSmsCenter
    {
        public AbstractSmsCenter(string login, string psw, ICollection<string> phones)
        {
            Login = login;
            Password = psw;
            Phones = phones;
        }

        abstract public void InitLogging(); 
        abstract public void InitSaving(); 
        abstract public void Logging();  
        abstract public void Saving();  

        public string Login { get; }
        public string Password { get; }
        public ICollection<string> Phones { get; }
    }
}
