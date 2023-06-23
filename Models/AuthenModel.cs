using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKeyApp.Models
{
    public class AuthenModel
    {
        public Guid? UserId { get; set; }

        public string DeviceId { get; set; }

        public string TenantId { get; set; }

        public string OtpCode { get; set; }

        public string AuthenType { get; set; }

        public AdditionalData? AdditionalData { get; set; }
    }
}
