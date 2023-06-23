using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKeyApp.Models
{
    public class GetAuthenTypeRequest
    {
        public Guid customerId { get; set; }

        public string deviceId { get; set; }

        public string tenantId = "A";
    }
}
