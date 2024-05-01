using GameServer.Entities;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;

namespace GameServer.Services
{
    public class MsgHandler
    {
        private readonly ILogger<MsgHandler> _logger;

        //客户端Socket及状态信息
        readonly Dictionary<Socket, ClientState> clients;
        readonly ClientEventHandler clientEventHandler;

        readonly Dictionary<string, Action<ClientState, string>> actions = [];
        public MsgHandler(ILogger<MsgHandler> logger, ServerCache cache, ClientEventHandler clientEventHandler)
        {
            _logger = logger;
            this.clients = cache.CurrentClients;
            this.clientEventHandler = clientEventHandler;
            actions.Add("MsgEnter", MsgEnter);
            actions.Add("MsgList", MsgList);
            actions.Add("MsgMove", MsgMove);
            actions.Add("MsgAttack", MsgAttack);
            actions.Add("MsgHit", MsgHit);
        }

        public void DoHandler(string handlerName, ClientState clientState, string msg)
        {
            if (actions.TryGetValue(handlerName, out Action<ClientState, string> action))
            {
                ////报错平台不支持,只能用同步的方式 2024年5月1日14:07:07
                //action.BeginInvoke(clientState, msg, AsyncComplete,action);
                action(clientState, msg);
            }

        }

        private void AsyncComplete(IAsyncResult asyncResult)
        {
            Action<ClientState, string>? action = asyncResult.AsyncState as Action<ClientState, string>;
            action?.EndInvoke(asyncResult);
        }


        public void MsgEnter(ClientState c, string msgArgs)
        {
            //解析参数
            string[] split = msgArgs.Split(',');
            string desc = split[0];
            float x = float.Parse(split[1]);
            float y = float.Parse(split[2]);
            float z = float.Parse(split[3]);
            float eulY = float.Parse(split[4]);
            //赋值
            c.hp = 100;
            c.x = x;
            c.y = y;
            c.z = z;
            c.eulY = eulY;
            //广播
            string sendStr = "Enter|" + msgArgs;
            foreach (ClientState cs in clients.Values)
            {
                clientEventHandler.Send(cs, sendStr);
            }
        }

        public void MsgList(ClientState c, string msgArgs)
        {
            string sendStr = "List|";
            foreach (ClientState cs in clients.Values)
            {
                sendStr += cs.socket.RemoteEndPoint.ToString() + ",";
                sendStr += cs.x.ToString() + ",";
                sendStr += cs.y.ToString() + ",";
                sendStr += cs.z.ToString() + ",";
                sendStr += cs.eulY.ToString() + ",";
                sendStr += cs.hp.ToString() + ",";
            }
            clientEventHandler.Send(c, sendStr);
        }

        public void MsgMove(ClientState c, string msgArgs)
        {
            //解析参数
            string[] split = msgArgs.Split(',');
            string desc = split[0];
            float x = float.Parse(split[1]);
            float y = float.Parse(split[2]);
            float z = float.Parse(split[3]);
            //赋值
            c.x = x;
            c.y = y;
            c.z = z;
            //广播
            string sendStr = "Move|" + msgArgs;
            foreach (ClientState cs in clients.Values)
            {
                clientEventHandler.Send(cs, sendStr);
            }
        }

        public void MsgAttack(ClientState c, string msgArgs)
        {
            //广播
            string sendStr = "Attack|" + msgArgs;
            foreach (ClientState cs in clients.Values)
            {
                clientEventHandler.Send(cs, sendStr);
            }
        }

        public void MsgHit(ClientState c, string msgArgs)
        {
            //解析参数
            string[] split = msgArgs.Split(',');
            string hitDesc = split[0];
            //被攻击
            ClientState hitCS = null;
            foreach (ClientState cs in clients.Values)
            {
                if (cs.socket.RemoteEndPoint.ToString() == hitDesc)
                    hitCS = cs;
            }
            if (hitCS == null)
                return;

            string sendStr = "Die|" + hitDesc;
            foreach (ClientState cs in clients.Values)
            {
                clientEventHandler.Send(cs, sendStr);
            }

        }
    }


}
