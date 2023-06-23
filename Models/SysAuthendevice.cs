using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKeyApp.Models
{
    public  class SysAuthendevice
    {
        public string Authentype { get; set; } = null!;

        public string Authenname { get; set; } = null!;

        public string? Description { get; set; }

        public int Lenght { get; set; }

        public int Failnumber { get; set; }

        public int Interval { get; set; }

        public string Status { get; set; } = null!;

        public DateTime? Datecreate { get; set; }

        public string? Usercreate { get; set; }

        public DateTime? Datemodify { get; set; }

        public string? Usermodify { get; set; }
    }
}
