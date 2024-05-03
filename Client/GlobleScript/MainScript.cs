using Godot;
using MultiTankWar;
using System;
using System.Collections.Generic;

public partial class MainScript : Node
{
    //人物模型预设
    public PackedScene humanpfb;
    public PackedScene myHumanPfb;
    //人物列表
    public ControlHunman myHuman;
    public Dictionary<string, BaseHuman> otherHumans = new Dictionary<string, BaseHuman>();

    public override void _Ready()
    {
   //  //网络模块
   //  NetManager.AddListener("Enter", OnEnter);
   //  NetManager.AddListener("List", OnList);
   //  NetManager.AddListener("Move", OnMove);
   //  NetManager.AddListener("Leave", OnLeave);
   //  NetManager.AddListener("Attack", OnAttack);
   //  NetManager.AddListener("Die", OnDie);
   //  GD.Print("ready to connect");
   //  NetManager.Connect("127.0.0.1", 8888);
   //  //添加一个角色
   //  humanpfb = ResourceLoader.Load<PackedScene>("res://AsyncPlayer.tscn");
   //  myHumanPfb = ResourceLoader.Load<PackedScene>("res://Player.tscn");
   //
   //  myHuman = (ControlHunman)CreatePlayer(true);
   //
   //  otherHumans.Add(NetManager.GetDesc(), myHuman);
   //  //发送协议
   //  Vector2 pos = myHuman.Position;
   //  float eul = myHuman.Rotation;
   //  string sendStr = "Enter|";
   //  sendStr += NetManager.GetDesc() + ",";
   //  sendStr += pos.X + ",";
   //  sendStr += pos.Y + ",";
   //  sendStr += "0,";
   //  sendStr += eul + ",";
   //  NetManager.Send(sendStr);
   //  NetManager.Send("List|");
    }

    public override void _Process(double delta)
    {
        NetManager.Update();
    }

    void OnEnter(string msgArgs)
    {
        GD.Print("OnEnter " + msgArgs);
        //解析参数
        string[] split = msgArgs.Split(',');
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        float eulY = float.Parse(split[4]);
        //是自己
        if (desc == NetManager.GetDesc())
            return;
        //添加一个角色
        BaseHuman h = CreatePlayer(false,x,y);
        h.desc = desc;
        otherHumans.Add(desc, h);
    }

    BaseHuman CreatePlayer(bool isMain = false,float x=0f,float y=0f)
    {
        PackedScene packedScene;
        if (isMain)
        {
            packedScene = myHumanPfb;
        }
        else
        {
            packedScene = humanpfb;

        }
        BaseHuman myHuman = packedScene.Instantiate<BaseHuman>();
        float x1 = x==0f?Random.Shared.Next(0, 800):x;
        float y1 =y==0f? Random.Shared.Next(0, 600):y;
        myHuman.Position = new Vector2(x1, y1);
       
      // myHuman.Position = new Vector2(100, 100);
        myHuman.desc = NetManager.GetDesc();
        GetTree().Root.CallDeferred(Node2D.MethodName.AddChild,myHuman);
        return myHuman;
    }


    void OnList(string msgArgs)
    {
        GD.Print("OnList " + msgArgs);
        //解析参数
        string[] split = msgArgs.Split(',');
        int count = (split.Length - 1) / 6;
        for (int i = 0; i < count; i++)
        {
            string desc = split[i * 6 + 0];
            float x = float.Parse(split[i * 6 + 1]);
            float y = float.Parse(split[i * 6 + 2]);
            float z = float.Parse(split[i * 6 + 3]);
            float eulY = float.Parse(split[i * 6 + 4]);
            int hp = int.Parse(split[i * 6 + 5]);
            //是自己
            if (desc == NetManager.GetDesc())
                continue;
            //添加一个角色
            BaseHuman h = CreatePlayer(false,x,y);
            h.desc = desc;
            otherHumans.Add(desc, h);
        }
    }

    void OnMove(string msgArgs)
    {
        GD.Print("OnMove " + msgArgs);
        //解析参数
        string[] split = msgArgs.Split(',');
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        //移动
        if (!otherHumans.ContainsKey(desc))
            return;
        BaseHuman h = otherHumans[desc];
        Vector2 targetPos = new Vector2(x, y);
        h.MoveTo(targetPos);

    }

    void OnLeave(string msgArgs)
    {
        GD.Print("OnLeave " + msgArgs);
        //解析参数
        string[] split = msgArgs.Split(',');
        string desc = split[0];
        //删除
        if (!otherHumans.ContainsKey(desc))
            return;
        BaseHuman h = otherHumans[desc];
        h.QueueFree();
        otherHumans.Remove(desc);
    }

    void OnAttack(string msgArgs)
    {
        GD.Print("OnAttack " + msgArgs);
        //解析参数
        string[] split = msgArgs.Split(',');
        string desc = split[0];
        float eulY = float.Parse(split[1]);
        //攻击动作
        if (!otherHumans.ContainsKey(desc))
            return;
        BaseHuman h = otherHumans[desc];
        h.Attack();
    }

    void OnDie(string msgArgs)
    {
        GD.Print("OnAttack " + msgArgs);
        //解析参数
        string[] split = msgArgs.Split(',');
        string attDesc = split[0];
        //自己死了
        if (attDesc == myHuman.desc)
        {
            myHuman.Die();
            GD.Print("Game Over");
            return;
        }
        //死了
        if (!otherHumans.ContainsKey(attDesc))
            return;
        AsyncHuman h = (AsyncHuman)otherHumans[attDesc];
        h.QueueFree();

    }
 
  
}
