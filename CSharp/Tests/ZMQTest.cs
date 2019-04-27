using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EPI.CSharp.Tests
{
    public class ZMQTest
    {
        bool isClosing = false;
        /// <summary>
        /// 启动发布服务器
        /// </summary>
        public void StartPubServer()
        {
            Task task = new Task(() =>
            {
                // 创建发布服务的Socket
                using (var server = new ZeroMQ.ZSocket(ZeroMQ.ZSocketType.PUB))
                {
                    server.Bind("tcp://127.0.0.1:1001");
                    while (true)
                    {
                        if (isClosing)
                        {
                            using (var quit = new ZeroMQ.ZFrame("Quit"))
                            {
                                server.SendFrame(quit);
                            }
                            break;
                        }
                        using (var head = new ZeroMQ.ZFrame("Time"))
                        {
                            server.SendFrameMore(head);
                        }
                        using (var body = new ZeroMQ.ZFrame(DateTime.Now.TimeOfDay.ToString()))
                        {
                            server.SendFrame(body);
                        }
                        Thread.Sleep(2000);
                    }
                    Console.WriteLine("PubServer is Closed.");
                }
            });
            task.Start();
        }

        /// <summary>
        /// 启动订阅客户端
        /// </summary>
        public void StartSubClient()
        {
            Task task = new Task(() =>
            {
                // 创建发布服务的Socket
                using (var client = new ZeroMQ.ZSocket(ZeroMQ.ZSocketType.SUB))
                {
                    client.Connect("tcp://127.0.0.1:1001");
                    client.Subscribe("Quit");
                    client.Subscribe("Time"); // 订阅内容标识
                    while (true)
                    {
                        if (isClosing)
                        {
                            break;
                        }
                        using (var msg = client.ReceiveMessage())
                        {
                            if (msg.Count == 1)
                            {
                                break;
                            }
                            if (msg.Count == 2)
                            {
                                var title = msg[0].ReadString();    // ReadString只能调用一次，多次调用会取不到数据
                                var content = msg[1].ReadString();  // 如需多次使用该消息，像这样存放到临时变量中
                                Console.WriteLine("{0}->{1}", title, content);
                            }
                        }
                    }
                    Console.WriteLine("SubClient is Closed.");
                }

            });
            task.Start();
        }

        /// <summary>
        /// 启动消息拉取服务器
        /// </summary>
        public void StartPullServer()
        {
            Task task = new Task(() =>
            {
                // 创建发布服务的Socket
                using (var server = new ZeroMQ.ZSocket(ZeroMQ.ZSocketType.PULL))
                {
                    server.Bind("tcp://127.0.0.1:1002");
                    while (true)
                    {
                        if (isClosing)
                        {
                            break;
                        }
                        using (var msg = server.ReceiveMessage())
                        {
                            var content = msg[0].ReadString();
                            if (content == "Quit")
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("{0}", content);
                            }
                        }
                    }
                    Console.WriteLine("PullServer is Closed.");
                }
            });
            task.Start();
        }

        /// <summary>
        /// 启动消息推送客户端
        /// </summary>
        public void StartPushClient()
        {
            Task task = new Task(() =>
            {
                // 创建推送消息的Socket
                using (var client = new ZeroMQ.ZSocket(ZeroMQ.ZSocketType.PUSH))
                {
                    client.Connect("tcp://127.0.0.1:1002");
                    while (true)
                    {
                        if (isClosing)
                        {
                            using (var content = new ZeroMQ.ZFrame("Quit"))
                            {
                                client.SendFrame(content);
                            }
                            break;
                        }
                        using (var content = new ZeroMQ.ZFrame("Push->"+DateTime.Now.TimeOfDay.ToString()))
                        {
                            client.SendFrame(content);
                        }
                        Thread.Sleep(2000);
                    }
                    Console.WriteLine("PushClient is Closed.");
                }

            });
            task.Start();
        }

        /// <summary>
        /// 启动路由服务器
        /// </summary>
        public void StartRouterServer()
        {
            var front = new ZeroMQ.ZSocket(ZeroMQ.ZSocketType.ROUTER);
            var backend = new ZeroMQ.ZSocket(ZeroMQ.ZSocketType.ROUTER);

            Task taskFront = new Task(() =>
            {
                int i = 0;
                front.Bind("tcp://127.0.0.1:1003");
                while (true)
                {
                    if (isClosing)
                    {
                        break;
                    }
                    using (var msg = front.ReceiveMessage())
                    {
                        var id = msg[0].ReadString();
                        Console.WriteLine("Received From Client:{0}", id);
                        var content = "Do Work->" + msg[1].ReadString();
                        var replyMsg = "No Worker";
                        if (workerList.Count > 0)
                        {
                            if (i > workerList.Count - 1)// 负载均衡工人
                            {
                                i = 0;
                            }
                            using (var head = new ZeroMQ.ZFrame(workerList[i]))
                            {
                                backend.SendFrameMore(head);
                            }
                            using (var body = new ZeroMQ.ZFrame(content))
                            {
                                backend.SendFrame(body);
                            }
                            replyMsg = "Beging Working...";
                            i++;
                        }
                        using (var reply = new ZeroMQ.ZFrame(replyMsg))
                        {
                            front.SendFrame(reply);
                        }
                    }
                }
                front.Close();
                Console.WriteLine("FrontServer is Closed.");
            });
            Task taskBacend = new Task(() =>
            {
                backend.Bind("tcp://127.0.0.1:1004");
                while (true)
                {
                    if (isClosing)
                    {
                        break;
                    }
                    // 接收工人上线消息
                    using (var msg = backend.ReceiveMessage())
                    {
                        var worker = msg[0].ReadString();
                        var info = msg[1].ReadString();
                        if (info == "Quit")
                        {
                            workerList.Remove(worker);
                        }
                        if (!workerList.Contains(worker))
                        {
                            workerList.Add(worker);
                        }
                        Console.WriteLine("{0} is standby, Msg:{1}", worker, info);
                    }
                }
                backend.Close();
                Console.WriteLine("BackendServer is Closed.");
            });
            taskFront.Start();
            taskBacend.Start();
        }

        List<string> workerList = new List<string>();

        /// <summary>
        /// 启动订阅客户端
        /// </summary>
        public void StartDealerClient()
        {
            Task task = new Task(() =>
            {
                // 创建推送消息的Socket
                using (var client = new ZeroMQ.ZSocket(ZeroMQ.ZSocketType.DEALER))
                {
                    client.Identity = Encoding.UTF8.GetBytes("Client1");
                    client.Connect("tcp://127.0.0.1:1003");
                    while (true)
                    {
                        if (isClosing)
                        {
                            using (var content = new ZeroMQ.ZFrame("Quit"))
                            {
                                client.SendFrame(content);
                            }
                            break;
                        }
                        using (var content = new ZeroMQ.ZFrame("Task " + DateTime.Now.TimeOfDay.ToString()))
                        {
                            client.SendFrame(content);
                        }
                        Thread.Sleep(10000);
                    }
                    Console.WriteLine("DealerClient is Closed.");
                }

            });
            task.Start();
        }

        /// <summary>
        /// 启动订阅客户端
        /// </summary>
        public void StartDealerWorker(string id)
        {
            Task task = new Task(() =>
            {
                using (var worker = new ZeroMQ.ZSocket(ZeroMQ.ZSocketType.DEALER))
                {
                    worker.Identity = Encoding.UTF8.GetBytes(id);
                    worker.Connect("tcp://127.0.0.1:1004");
                    using (var content = new ZeroMQ.ZFrame("hello"))
                    {
                        worker.SendFrame(content);
                    }
                    while (true)
                    {
                        if (isClosing)
                        {
                            using (var quit = new ZeroMQ.ZFrame("Quit"))
                            {
                                worker.SendFrame(quit);
                            }
                            break;
                        }
                        using (var msg = worker.ReceiveMessage())
                        {
                            var content = msg[0].ReadString();
                            Console.WriteLine("Begin Working->" + content);
                            using (var info = new ZeroMQ.ZFrame("i am working..."))
                            {
                                worker.SendFrame(info);
                            }
                        }
                    }
                    Console.WriteLine("BackendClient is Closed.");
                }

            });
            task.Start();
        }

        public void Stop()
        {
            isClosing = true;
        }
    }

}
