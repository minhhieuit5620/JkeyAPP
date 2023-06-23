using JKeyApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKeyApp.Service
{
    public interface ILoginService
    {
        Task<LoginResponse> Authenticate(LoginRequest loginRequest);
        Task<ConfigResponse> ConfigAppSys();
    }
}
