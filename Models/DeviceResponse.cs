using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKeyApp.Models
{
    public class DeviceResponse
    {
        public string errorCode { get; set; }
        public string errorDesc { get; set; }
        public DeviceUser data { get; set; }
    }
}
