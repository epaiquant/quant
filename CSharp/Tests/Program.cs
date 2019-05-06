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
            //ZMQTest zmqTest = new ZMQTest();
            //zmqTest.StartPubServer();
            //zmqTest.StartSubClient();

            //zmqTest.StartPullServer();
            //zmqTest.StartPushClient();

            //zmqTest.StartRouterServer();
            //zmqTest.StartDealerWorker("Worker1");

            //zmqTest.StartDealerWorker("Worker2");

            //zmqTest.StartDealerClient();

            //while (true)
            //{
            //    var msg = Console.ReadLine();
            //    if (msg == "Q")
            //    {
            //        zmqTest.Stop();
            //        Console.WriteLine("程序已结束.");
            //        Console.Read();
            //        break;
            //    }

            //}

            StrategyTest strategyTest = new StrategyTest(100002, "f57e3cb8-0920-4086-8075-8cbc7887ec72");
            strategyTest.OnRtnLogEvent += (msg, isError) =>
            {
                Console.WriteLine(msg);
            };
            strategyTest.OnRtnErrorEvent += (title, msg) =>
            {
                Console.WriteLine("{0}:{1}",title,msg);
            };
            strategyTest.IsSavedToCsv = true;
            if (strategyTest.InitSetting("rb000,1M,100000,50,50"))
            {
                if (strategyTest.CreateMockBarDatas("1M_rb000", "2019-04-01", "2019-05-01", "rb000_1M.csv", null))
                {
                    strategyTest.InitStrategy("9,3,3");
                }

            }
            Console.Read();

        }
    }
}
