using Godot;
using MultiTankWar;

public partial class Projectile : Node2D
{
    //移动速度
    float speed = 500f;

    Vector2 _direction;

    public Vector2 Direction
    {
        set
        {
            _direction = value;

        }
        get => _direction;
    }


    //  private Vector2 _targetPostion;
    //  public Vector2 TargetPostion
    //  {
    //      set
    //      {
    //          _targetPostion = value;
    //          LookAt(Vector2.Zero);
    //      }
    //      get => _targetPostion;
    //  }




    System.Threading.Timer timer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        timer = new System.Threading.Timer(Timer_Timeout, null, 5000, 1);
    }

    private void Timer_Timeout(object state)
    {
        timer.Dispose();
        QueueFree();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        // GD.Print(Direction+" "+ Position);
        this.Position += Direction * speed * (float)delta;

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }


    public void BodyEnter(Node2D node2D)
    {
        if (node2D is BaseHuman human)
        {
            string sendStr = "Hit|" + human.desc + ",0";
            NetManager.Send(sendStr);
            /// 不能自己说谁死就让谁死,让服务器广播谁死
           // human.Die();
            Timer_Timeout(null);
        }
    }

}
