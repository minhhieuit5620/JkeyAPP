using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKeyApp.Models
{
    public class GenerateOTPModel
    {
        public string UserId { get; set; }
        public string DeviceId { get; set; }
        public string TenantId { get; set; }
        public string AuthenType { get; set; }
        public object AdditionalData { get; set; }
    }
}
