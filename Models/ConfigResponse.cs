using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKeyApp.Models
{
    public class ConfigResponse
    {
        public string errorCode { get; set; }
        public string errorDesc { get; set; }
        public ObservableCollection<SysAuthendevice> data { get; set; }
    }
}
