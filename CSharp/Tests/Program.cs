using EPI.CSharp.Model;
using EPI.CSharp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPI.CSharp.Tests
{
    class Program
    {
        public static Users UserInfo { get; set; }

        static void Main(string[] args)
        {
            UserService userService = new UserService();
            UserInfo = userService.Login("jie", "123456");

            var user = userService.GetUserInfo(UserInfo.UserId, UserInfo.Token);
        }
    }
}
