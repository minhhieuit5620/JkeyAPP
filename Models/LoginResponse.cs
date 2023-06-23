using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKeyApp.Models
{
    public class LoginResponse
    {

        public string errorCode { get; set; }
        public string errorDesc { get; set; }
        public DataLogin data { get; set; }    
    }
}
