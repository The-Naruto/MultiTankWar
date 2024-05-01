using GameServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    public class ServerCache
    {
       public Dictionary<Socket, ClientState> CurrentClients = [];
    }
}
