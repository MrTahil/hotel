using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hmz_rt.Models.Dtos
{
    public static class TokenStorage
    {
        public static string AuthToken { get; set; }
        public static string RefreshToken { get; set; }
        public static string Role { get; set; }
        public static string Username { get; set; }

        public static int UserId { get; set; }


        public static void ClearTokens()
        {
            AuthToken = null;
            RefreshToken = null;
            Username = null;
            Role = null;
            UserId = 0;
        }
    }
}
