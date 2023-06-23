using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKeyApp.Services
{
    public enum OtpHashMode
    {
        /// <summary>
        /// Sha1 is used as the HMAC hashing algorithm
        /// </summary>
        Sha1,
        /// <summary>
        /// Sha256 is used as the HMAC hashing algorithm
        /// </summary>
        Sha256,
        /// <summary>
        /// Sha512 is used as the HMAC hashing algorithm
        /// </summary>
        Sha512
    }
}
