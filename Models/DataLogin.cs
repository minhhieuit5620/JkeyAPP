using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKeyApp.Models
{
    public class DataLogin
    {
        public string accessToken { get; set; }
        public DateTime expiresIn { get; set; }
        public string tokenType { get; set; }

    }
}
