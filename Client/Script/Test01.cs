using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using static System.Net.Mime.MediaTypeNames;

public partial class Test01 : Control
{
    [Export]
    TextEdit _textEdit;
    [Export]
    public RichTextLabel _richTextLabel;

    Socket socket;
    byte[] readBuff = new byte[1024];
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
        socket = new Socket(AddressFamily.InterNetwork,
        SocketType.Stream, ProtocolType.Tcp);
        //Connect
        //socket.Connect("127.0.0.1", 8888);
        
        socket.BeginConnect("127.0.0.1", 8888, ConnectCallback, socket);
    }
    public void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            GD.Print("Socket Connect Succ ");
            socket.BeginReceive(readBuff, 0, 1024, 0,
                ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            GD.Print("Socket Connect fail" + ex.ToString());
        }
    }
    //Receive回调
    public void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);
            string s = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
            s += "\r\n";
            _richTextLabel.CallDeferred(RichTextLabel.MethodName.AppendText, s); 
            socket.BeginReceive(readBuff, 0, 1024, 0,
                ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            GD.Print("Socket Receive fail" + ex.ToString());
        }
    }

    public void Send()
    {
        //Send
        string sendStr = _textEdit.Text;
        byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);
        socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
    }
    //Send回调
    public void SendCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndSend(ar);
             GD.Print("Socket Send succ " + count);
        }
        catch (SocketException ex)
        {
            GD.Print("Socket Send fail" + ex.ToString());
        }

    }
}
