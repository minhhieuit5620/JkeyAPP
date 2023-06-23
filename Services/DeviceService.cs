using JKeyApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace JKeyApp.Services
{
    public class DeviceService : IDeviceService
    {
        /// <summary>
        /// get all authen type from device user
        /// </summary>
        /// <param name="getAuthenTypeRequest"></param>
        /// <returns></returns>
        public async Task<DeviceUserResponse> GetAllAuthenTypeInDevice(GetAuthenTypeRequest getAuthenTypeRequest)
        {
            try
            {

                using (var client = new HttpClient())
                {
                    string token = await SecureStorage.GetAsync("TOKEN");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    string getAuthenTypeRequestStr = JsonConvert.SerializeObject(getAuthenTypeRequest);
                    var response = await client.PostAsync(ConfigApp.Config.baseURL + "/device/getauthentypeindevice",
                        new StringContent(getAuthenTypeRequestStr, Encoding.UTF8, "application/json"));
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<DeviceUserResponse>(json);
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {

                return new DeviceUserResponse
                {
                    errorCode = "9999",
                    errorDesc = ex.Message,
                };
            }
        }

     

        /// <summary>
        /// register device user 
        /// </summary>
        /// <param name="deviceUser"></param>
        /// <returns></returns>
        public async Task<DeviceResponse> RegisterDevice(DeviceUser deviceUser)
        {
            try
            {

                using (var client = new HttpClient())
                {
                    string token = await SecureStorage.GetAsync("TOKEN");                  

                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    string deviceUserStr = JsonConvert.SerializeObject(deviceUser);
                    var response = await client.PostAsync(ConfigApp.Config.baseURL+"/device/registerdevice",
                        new StringContent(deviceUserStr, Encoding.UTF8, "application/json"));
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<DeviceResponse>(json);
                    }
                    else
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<DeviceResponse>(json);
                    }
                }
            }
            catch (Exception ex)
            {

                return new DeviceResponse
                {
                    errorCode = "9999",
                    errorDesc= ex.Message,
                  
                    
                };
            }
        }

        /// <summary>
        /// unregister device user 
        /// when user registed user can unregis device 
        /// </summary>
        /// <param name="deviceUser"></param>
        /// <returns></returns>
        public async Task<DeviceResponse> UnRegisterDevice(DeviceUser deviceUser)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string token = await SecureStorage.GetAsync("TOKEN");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    string deviceUserStr = JsonConvert.SerializeObject(deviceUser);
                    var response = await client.PostAsync(ConfigApp.Config.baseURL+"/device/unregisterdevice",
                        new StringContent(deviceUserStr, Encoding.UTF8, "application/json"));
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<DeviceResponse>(json);
                    }
                    else { return null; }
                }
            }
            catch (Exception ex)
            {


                return new DeviceResponse
                {
                    errorCode = "9999",
                    errorDesc = ex.Message,


                };
            }
            
           
        }


        /// <summary>
        /// send OTP to email at time register
        /// when user register, system will send an email have OTP code
        /// </summary>
        /// <param name="deviceUser"></param>
        /// <returns></returns>
        public async Task<DeviceResponse> OTPFirstRegist(GenerateOTPModel deviceUser)
        {
            try
            {

                using (var client = new HttpClient())
                {
                    string token = await SecureStorage.GetAsync("TOKEN");

                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    string getAuthenTypeRequestStr = JsonConvert.SerializeObject(deviceUser);
                    var response = await client.PostAsync(ConfigApp.Config.baseURL + "/generateotp/generatefirstotp",
                        new StringContent(getAuthenTypeRequestStr, Encoding.UTF8, "application/json"));
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<DeviceResponse>(json);
                    }
                    else
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<DeviceResponse>(json);
                    }
                }
            }
            catch (Exception ex)
            {

                return new DeviceResponse
                {
                    errorCode = "9999",
                    errorDesc = ex.Message,
                };
            }
        }

        /// <summary>
        /// Verify OTP first register device user 
        /// Verify OTP system sent to email customer to verify email
        /// </summary>
        /// <param name="deviceUser"></param>
        /// <returns></returns>
        public async Task<DeviceResponse> VerifyOTPFirstRegist(AuthenModel deviceUser)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string token = await SecureStorage.GetAsync("TOKEN");

                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    string deviceUserStr = JsonConvert.SerializeObject(deviceUser);
                    var response = await client.PostAsync(ConfigApp.Config.baseURL + "/generateotp/verifyotpfirst",
                        new StringContent(deviceUserStr, Encoding.UTF8, "application/json"));
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<DeviceResponse>(json);
                    }
                    else
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<DeviceResponse>(json);
                    }
                }
            }
            catch (Exception ex)
            {

                return new DeviceResponse
                {
                    errorCode = "9999",
                    errorDesc = ex.Message,
                };
            }
        }
    
        public async Task<DeviceResponse> CheckScanQR(DeviceUser deviceUser)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string token = await SecureStorage.GetAsync("TOKEN");

                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    string deviceUserStr = JsonConvert.SerializeObject(deviceUser);
                    var response = await client.PostAsync(ConfigApp.Config.baseURL + "/device/CheckScanQR",
                        new StringContent(deviceUserStr, Encoding.UTF8, "application/json"));
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var json = await response.Content.ReadAsStringAsync();

                       // var a= JsonConvert.DeserializeObject<DeviceResponse>(json);
                        return JsonConvert.DeserializeObject<DeviceResponse>(json);
                    }
                    else
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<DeviceResponse>(json);
                    }
                }
            }
            catch (Exception ex)
            {

                return new DeviceResponse
                {
                    errorCode = "9999",
                    errorDesc = ex.Message,
                };
            }
        }

        public async Task<DeviceResponse> VerifyOTPAdvanced(AuthenModel authenModel)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string token = await SecureStorage.GetAsync("TOKEN");

                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    string deviceUserStr = JsonConvert.SerializeObject(authenModel);
                    var response = await client.PostAsync(ConfigApp.Config.baseURL + "/generateotp/verifyotp",
                        new StringContent(deviceUserStr, Encoding.UTF8, "application/json"));
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var json = await response.Content.ReadAsStringAsync();

                        // var a= JsonConvert.DeserializeObject<DeviceResponse>(json);
                        return JsonConvert.DeserializeObject<DeviceResponse>(json);
                    }
                    else
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<DeviceResponse>(json);
                    }
                }
            }
            catch (Exception ex)
            {

                return new DeviceResponse
                {
                    errorCode = "9999",
                    errorDesc = ex.Message,
                };
            }
        }
    }
}
