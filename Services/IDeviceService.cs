using JKeyApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKeyApp.Services
{
    public interface IDeviceService
    {
        Task<DeviceUserResponse> GetAllAuthenTypeInDevice(GetAuthenTypeRequest getAuthenTypeRequest);
        Task<DeviceResponse> RegisterDevice(DeviceUser deviceUser);
        Task<DeviceResponse> CheckScanQR(DeviceUser deviceUser);
        Task<DeviceResponse> UnRegisterDevice(DeviceUser deviceUser);
        Task<DeviceResponse> OTPFirstRegist(GenerateOTPModel deviceUser);
        Task<DeviceResponse> VerifyOTPFirstRegist(AuthenModel authenModel);
        Task<DeviceResponse> VerifyOTPAdvanced(AuthenModel authenModel);
    }
}
