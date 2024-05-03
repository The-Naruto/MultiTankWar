using Godot;
using MultiTankWar.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

public partial class Chapter4 : Control
{
    [Export]
    TextEdit _textEdit;
    [Export]
    public RichTextLabel _richTextLabel;

    Socket socket;
    //接收缓冲区
    ByteArray readBuff = new ByteArray();
    //发送缓冲区
    Queue<ByteArray> writeQueue = new Queue<ByteArray>();
    bool isSending = false;
    //显示文字
    string recvStr = "";

    //checkRead列表
    List<Socket> checkRead = new List<Socket>();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!string.IsNullOrEmpty(recvStr))
        {
            _richTextLabel.AppendText(recvStr);
            recvStr=string.Empty;
        }
        // if (socket == null)
        // {
        //     return;
        // }
        // //填充checkRead列表
        // checkRead.Clear();
        // checkRead.Add(socket);
        // //select
        // Socket.Select(checkRead, null, null, 1000);
        // //check
        // foreach (Socket s in checkRead)
        // {
        //     byte[] readBuff = new byte[1024];
        //     int count = socket.Receive(readBuff);
        //     string recvStr = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
        //     _richTextLabel.Text = recvStr;
        // }

    }

    public void Connect()
    {
        //Socket
        socket = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
        //为了精简代码：使用同步Connect
        //             不考虑抛出异常
        socket.Connect("127.0.0.1", 8888);

        socket.BeginReceive(readBuff.bytes, readBuff.writeIdx,
                            readBuff.remain, 0, ReceiveCallback, socket);
    }
    //public void ConnectCallback(IAsyncResult ar)
    //{
    //    try
    //    {
    //        Socket socket = (Socket)ar.AsyncState;
    //        socket.EndConnect(ar);
    //        GD.Print("Socket Connect Succ ");
    //        socket.BeginReceive(readBuff, 0, 1024, 0,
    //            ReceiveCallback, socket);
    //    }
    //    catch (SocketException ex)
    //    {
    //        GD.Print("Socket Connect fail" + ex.ToString());
    //    }
    //}
    //Receive回调
    public void ReceiveCallback(IAsyncResult ar)
    {
        try
        {

            Socket socket = (Socket)ar.AsyncState;
            //获取接收数据长度
            int count = socket.EndReceive(ar);
            readBuff.writeIdx += count;
            //处理二进制消息
            OnReceiveData();
            //继续接收数据
            if (readBuff.remain < 8)
            {
                readBuff.MoveBytes();
                readBuff.ReSize(readBuff.length * 2);
            }
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx,
                readBuff.remain, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            GD.Print("Socket Receive fail" + ex.ToString());
        }
    }
    public void OnReceiveData()
    {
        GD.Print("[Recv 1] length  =" + readBuff.length);
        GD.Print("[Recv 2] readbuff=" + readBuff.ToString());
        if (readBuff.length <= 2)
            return;
        //消息长度
        int readIdx = readBuff.readIdx;
        byte[] bytes = readBuff.bytes;
        // Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
        Int16 bodyLength = readBuff.ReadInt16();
        if (readBuff.length < bodyLength)
            return;
        //内部已经加了
        //readBuff.readIdx += 2;
        GD.Print("[Recv 3] bodyLength=" + bodyLength);
        //消息体
        byte[] stringByte = new byte[bodyLength];
        readBuff.Read(stringByte, 0, bodyLength);
        string s = System.Text.Encoding.UTF8.GetString(stringByte);

        GD.Print("[Recv 4] s=" + s);
        ;
        GD.Print("[Recv 5] readbuff=" + s.ToString());

        //处理消息
        recvStr = s;

        //继续读取消息
        if (readBuff.length > 2)
        {
            OnReceiveData();
        }
    }

    public void Send()
    {
        string sendStr = _textEdit.Text;
        //组装协议
        byte[] bodyBytes = System.Text.Encoding.Default.GetBytes(sendStr);
        Int16 len = (Int16)bodyBytes.Length;
        byte[] lenBytes = BitConverter.GetBytes(len);
        //大小端编码
        if (!BitConverter.IsLittleEndian)
        {
            GD.Print("[Send] Reverse lenBytes");
            lenBytes = lenBytes.Reverse().ToArray();
        }
        //拼接字节
        byte[] sendBytes = lenBytes.Concat(bodyBytes).ToArray();
        ByteArray ba = new ByteArray(sendBytes);
        lock (writeQueue)
        {
            writeQueue.Enqueue(ba);
        }
        //send
        //socket = new Socket(AddressFamily.InterNetwork,
        //    SocketType.Stream, ProtocolType.Tcp);
        if (!isSending)
        {
            socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
        }
        GD.Print("[Send] " + BitConverter.ToString(sendBytes));
    }
    //Send回调
    public void SendCallback(IAsyncResult ar)
    {
        try
        {
            isSending =true;
            //获取state
            Socket socket = (Socket)ar.AsyncState;
            //EndSend的处理
            int count = 0;
            count = socket.EndSend(ar);

            GD.Print("Socket Send succ " + count);
            ByteArray ba = new ByteArray();
            lock (writeQueue)
            {
                if (writeQueue.Count > 0)
                {
                    ba = writeQueue.Dequeue();
                }
                else
                {
                    isSending = false;
                    return;
                }
                // ba = writeQueue.First();
            }


            if (ba != null)
            {
                socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallback, socket);
            }
            else
            {
                isSending = false;
            }
        }
        catch (SocketException ex)
        {
            GD.Print("Socket Send fail" + ex.ToString());
        }

    }
}
