using Godot;
using System.Net.Sockets;

public partial class Test01 : Control
{
    [Export]
    TextEdit _textEdit;

    [Export]
    public RichTextLabel _richTextLabel;

    Socket socket;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void Connect()
    {
        socket = new Socket(AddressFamily.InterNetwork,
        SocketType.Stream, ProtocolType.Tcp);
        //Connect
        socket.Connect("127.0.0.1", 8888);

    }

    public void Send()
    {
        //Send
        string sendStr = _textEdit.Text;
        byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);
        socket.Send(sendBytes);
        //Recv
        byte[] readBuff = new byte[1024];
        int count = socket.Receive(readBuff);
        string recvStr = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
        _richTextLabel.Text = recvStr;
        //Close
       // socket.Close();
    }
}
