using Microsoft.Extensions.Hosting; 
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GameServer.Entities;

namespace GameServer.Servers
{
    internal class MyServer02 : IHostedService
    {
        ILogger<MyServer02> _logger;
        Socket _listenfd;
        //客户端Socket及状态信息
        readonly Dictionary<Socket, ClientState> _clients = [];
        public MyServer02(ILogger<MyServer02> logger  )
        {
            _logger = logger;
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
                foreach (ClientState s in _clients.Values)
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

        private bool ReadClientfd(Socket clientfd)
        {
            try
            {

          
            ClientState state = _clients[clientfd];
            int count = clientfd.Receive(state.readBuff);
            //客户端关闭
            if (count == 0)
            {
                clientfd.Close();
                _clients.Remove(clientfd);
                _logger.LogDebug("Socket Close");
                return false;
            }
            //广播
            string recvStr = System.Text.Encoding.Default.GetString(state.readBuff, 0, count);
            _logger.LogDebug("Receive " + recvStr);
            string sendStr = clientfd.RemoteEndPoint.ToString() + ":" + recvStr;
            byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);
            foreach (ClientState cs in _clients.Values)
            {
                cs.socket.Send(sendBytes);
            }
            }
            catch (Exception e)
            {
                _clients.Remove(clientfd);
                _logger.LogError("recv error:"+e.Message);
            }
            return true;
        }

        private void ReadListenfd(Socket listenfd)
        {
            _logger.LogDebug("Accept");
            Socket clientfd = listenfd.Accept();
            ClientState state = new ClientState();
            state.socket = clientfd;
            _clients.Add(clientfd, state);
           
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _listenfd.Close();
            _listenfd.Dispose();
            return Task.CompletedTask;
        }
    }
}
