using GameServer.Entities;
using GameServer.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;

namespace GameServer.Servers
{
    internal class MyServer03 : IHostedService
    {
        ILogger<MyServer03> _logger;
        Socket _listenfd;
        //客户端Socket及状态信息
        readonly Dictionary<Socket, ClientState> clients;
        readonly ClientEventHandler clientEventHandler;
        readonly MsgHandler msgHandler;
        public MyServer03(ILogger<MyServer03> logger, ServerCache serverCache, ClientEventHandler clientEventHandler, MsgHandler msgHandler)
        {
            _logger = logger;
            clients = serverCache.CurrentClients;
            this.clientEventHandler = clientEventHandler;
            this.msgHandler = msgHandler;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Bind
            IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEp = new IPEndPoint(ipAdr, 8888);
            _listenfd.Bind(ipEp);
            //Listen
            _listenfd.Listen(0);
            _logger.LogDebug("[服务器]启动成功");
            while (!cancellationToken.IsCancellationRequested)
            {
                //检查listenfd
                if (_listenfd.Poll(0, SelectMode.SelectRead))
                {
                    ReadListenfd(_listenfd);
                }
                //检查clientfd
                foreach (ClientState s in clients.Values)
                {
                    Socket clientfd = s.socket;
                    if (clientfd.Poll(0, SelectMode.SelectRead))
                    {
                        if (!ReadClientfd(clientfd))
                        {
                            break;
                        }
                    }
                }
                //防止cpu占用过高
                await Task.Delay(10);


            }
            return;
        }

        //读取Clientfd
        public bool ReadClientfd(Socket clientfd)
        {
            ClientState state = clients[clientfd];
            //接收
            int count = 0;
            try
            {
                count = clientfd.Receive(state.readBuff);
            }
            catch (SocketException ex)
            {
                clientEventHandler.OnDisconnect(state);
                clientfd.Close();
                clients.Remove(clientfd);
                _logger.LogDebug("Receive SocketException " + ex.ToString());
                return false;
            }
            //客户端关闭
            if (count <= 0)
            {
                clientEventHandler.OnDisconnect(state);
                clientfd.Close();
                clients.Remove(clientfd);
                _logger.LogDebug("Socket Close");
                return false;
            }
            //消息处理
            string recvStr =
                    System.Text.Encoding.Default.GetString(state.readBuff, 0, count);
            string[] split = recvStr.Split('|');
            _logger.LogDebug("Recv " + recvStr);
            //本地测试一直沾包,所以调整解包方式

            int idx = 0;
            while (idx < split.Length)
            {
                string msgName = split[idx];
                string msgArgs = string.Empty;
                if (idx+1 < split.Length-1)
                { 
                    msgArgs = split[idx + 1];
                }
                idx += 2;
                string funName = "Msg" + msgName;
                msgHandler.DoHandler(funName, state, msgArgs);
            }



            return true;
        }

        private void ReadListenfd(Socket listenfd)
        {
            _logger.LogDebug("Accept");
            Socket clientfd = listenfd.Accept();
            ClientState state = new ClientState();
            state.socket = clientfd;
            clients.Add(clientfd, state);

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _listenfd.Close();
            _listenfd.Dispose();
            return Task.CompletedTask;
        }
    }
}
