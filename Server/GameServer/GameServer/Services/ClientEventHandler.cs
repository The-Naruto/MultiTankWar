using GameServer.Entities;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using System.Text;

namespace GameServer.Services
{
    public class ClientEventHandler
    {
        private readonly ILogger<ClientEventHandler> _logger;

        //客户端Socket及状态信息
        readonly Dictionary<Socket, ClientState> _clients;
        public ClientEventHandler(ILogger<ClientEventHandler> logger, ServerCache serverCache)
        {
            _logger = logger;
            _clients = serverCache.CurrentClients;
        }

        public void OnDisconnect(ClientState c)
        {
            string desc = c.socket.RemoteEndPoint.ToString();
            string sendStr = "Leave|" + desc + ",";
            foreach (ClientState cs in _clients.Values)
            {
                Send(cs, sendStr);
            }
        }

        //发送
        public void Send(ClientState cs, string sendStr)
        {
            try
            {
                byte[] sendBytes = Encoding.Default.GetBytes(sendStr);
                cs.socket.Send(sendBytes);
            }
            catch (SocketException ex)
            {
                _clients.Remove(cs.socket);
                _logger.LogError("Send msg error:" + ex.Message);
            }
        }
    }
}
