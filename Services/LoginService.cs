using JKeyApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JKeyApp.Service
{
    public class LoginService : ILoginService
    {
        public async Task<LoginResponse> Authenticate(LoginRequest loginRequest)
        {
            using(var client=new HttpClient())
            {
                string loginRequestStr= JsonConvert.SerializeObject(loginRequest);
                var response = await client.PostAsync(ConfigApp.Config.baseURL + "/login/loginsystem", 
                    new StringContent(loginRequestStr, Encoding.UTF8, "application/json"));
                if (response.StatusCode== System.Net.HttpStatusCode.OK)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<LoginResponse>(json);
                }
                else { var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<LoginResponse>(json); }
            }
        }

        public async Task<ConfigResponse> ConfigAppSys()
        {
            using (var client = new HttpClient())
            {
               // string loginRequestStr = JsonConvert.SerializeObject(loginRequest);
                var response = await client.PostAsync(ConfigApp.Config.baseURL + "/device/getConfigApp",
                    new StringContent( "application/json"));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ConfigResponse>(json);
                }
                else
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ConfigResponse>(json);
                }
            }
        }
    }
}
