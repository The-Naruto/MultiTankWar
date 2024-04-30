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

namespace GameServer.Servers
{
    internal class MyServer01 : IHostedService
    {
        ILogger<MyServer01> _logger;
        Socket _listenfd;
        public MyServer01(ILogger<MyServer01> logger  )
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _listenfd = new Socket(AddressFamily.InterNetwork,      SocketType.Stream, ProtocolType.Tcp);
            //Bind
            IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEp = new IPEndPoint(ipAdr, 8888);
            _listenfd.Bind(ipEp);
            //Listen
            _listenfd.Listen(0);
            _logger.LogDebug("[服务器]启动成功");
            while (!cancellationToken.IsCancellationRequested)
            {
                //Accept
                Socket connfd = _listenfd.Accept();
                _logger.LogDebug("[服务器]Accept");
                //Receive
                byte[] readBuff = new byte[1024];
                int count = connfd.Receive(readBuff);
                string readStr = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
                _logger.LogDebug("[服务器接收]" + readStr);
                //Send
                string sendStr = System.DateTime.Now.ToString()+":"+ readStr;
                byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);
                connfd.Send(sendBytes);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _listenfd.Close();
            _listenfd.Dispose();
            return Task.CompletedTask;
        }
    }
}
