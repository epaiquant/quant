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
            // 测试获取Web数据
            //UserService userService = new UserService();
            //UserInfo = userService.Login("jie", "123456");
            //var user = userService.GetUserInfo(UserInfo.UserId, UserInfo.Token);

            // 测试ZeroMQ
            ZMQTest zmqTest = new ZMQTest();
            zmqTest.StartPubServer();
            zmqTest.StartSubClient();

            zmqTest.StartPullServer();
            zmqTest.StartPushClient();

            zmqTest.StartRouterServer();
            zmqTest.StartDealerWorker("Worker1");
            
            zmqTest.StartDealerWorker("Worker2");
            
            zmqTest.StartDealerClient();

            while (true)
            {
                var msg = Console.ReadLine();
                if (msg == "Q")
                {
                    zmqTest.Stop();
                    Console.WriteLine("程序已结束.");
                    Console.Read();
                    break;
                }
                
            }

        }
    }
}
