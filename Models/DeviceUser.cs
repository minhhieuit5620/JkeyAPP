using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKeyApp.Models
{
    public class DeviceUser
    {
        public Guid CustomerId { get; set; }

        public string DeviceId { get; set; } = null!;

        public string TenantId { get; set; } = null!;

        public string Status { get; set; }

        public string AuthenType { get; set; } = null!;

        public DateTime? IssueDate { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public DateTime? ActivedDate { get; set; }

        public string? ActiveCode { get; set; }

        public Guid? UserCreated { get; set; }

        public Guid? UserDeleted { get; set; }

        public Guid? UserApproved { get; set; }

        public string? Referent1 { get; set; }

        public string? Referent2 { get; set; }

        public string? Referent3 { get; set; }

        public string? Email { get; set; }
        public string? Phone { get; set; }

        public string? UniqueQr { get; set; }
    }

}
