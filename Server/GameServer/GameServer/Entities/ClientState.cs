﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    internal class ClientState
    {
        public Socket socket;
        public byte[] readBuff = new byte[1024];
    }
}