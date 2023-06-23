using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKeyApp.Models
{
    public enum ErrorCode
    {
        /// <summary>
        /// successfully
        /// </summary>
        Success = 0,

        /// <summary>
        /// some error from user, ex: field empty,error syntax
        /// </summary>
        UserInvalid = 1,

        /// <summary>
        /// system have exception
        /// </summary>
        Exception = 2,

        /// <summary>
        /// Invalid OTP verify
        /// </summary>
        InvalidOTP = 3,
    }
}
