using Godot;
using MultiTankWar;

public partial class ControlHunman : BaseHuman
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
    }



    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton inputEventMouse)
        {
            if (inputEventMouse.ButtonIndex == MouseButton.Right && inputEventMouse.Pressed)
            {
                Vector2 mouse = GetGlobalMousePosition();


                //发送协议
                string sendStr = "Move|";
                sendStr += NetManager.GetDesc() + ",";
                sendStr += mouse.X + ",";
                sendStr += mouse.Y + ",";
                sendStr += "0,";
                sendStr += "0,";
                NetManager.Send(sendStr);

                // base.MoveTo(mouse);

            }


        }
        if (@event is InputEventKey inputEvent)
        {
            if (inputEvent.Keycode == Key.X && inputEvent.Pressed)
            {
                string sendStr = "Attack|";
                sendStr += NetManager.GetDesc() + ",";
                sendStr += "0,";
                NetManager.Send(sendStr);

                // base.Attack();
            }

        }

        base._Input(@event);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        // Input.IsKeyPressed(MouseButtonMask.Left.);
        //
        //
        // Vector2 direction = Input.GetVector("left", "right", "up", "down");
        // if (direction.Length() != 0)
        // {
        //     base.MoveTo(direction);
        //     //发送协议
        //     //string sendStr = "Move|";
        //     //sendStr += NetManager.GetDesc() + ",";
        //     //sendStr += hit.point.x + ",";
        //     //sendStr += hit.point.y + ",";
        //     //sendStr += hit.point.z + ",";
        //     //NetManager.Send(sendStr);
        // }
        // if (Input.IsActionPressed("attack"))
        // {
        //     base.Attack();
        //     //发送协议
        // }

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
        //
        //   //移动
        //   if (Input.GetMouseButtonDown(0))
        //   {
        //       Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //       RaycastHit hit;
        //       Physics.Raycast(ray, out hit);
        //       if (hit.collider.tag == "Terrain")
        //       {
        //           MoveTo(hit.point);
        //           //发送协议
        //           string sendStr = "Move|";
        //           sendStr += NetManager.GetDesc() + ",";
        //           sendStr += hit.point.x + ",";
        //           sendStr += hit.point.y + ",";
        //           sendStr += hit.point.z + ",";
        //           NetManager.Send(sendStr);
        //
        //       }
        //   }
        //   //攻击
        //   if (Input.GetMouseButtonDown(1))
        //   {
        //       if (isAttacking) return;
        //       if (isMoving) return;
        //
        //       Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //       RaycastHit hit;
        //       Physics.Raycast(ray, out hit);
        //       if (hit.collider.tag != "Terrain") return;
        //       transform.LookAt(hit.point);
        //       Attack();
        //       //发送协议
        //       string sendStr = "Attack|";
        //       sendStr += NetManager.GetDesc() + ",";
        //       sendStr += transform.eulerAngles.y + ",";
        //       NetManager.Send(sendStr);
        //       //攻击判定
        //       Vector3 lineEnd = transform.position + 0.5f * Vector3.up;
        //       Vector3 lineStart = lineEnd + 20 * transform.forward;
        //       if (Physics.Linecast(lineStart, lineEnd, out hit))
        //       {
        //           GameObject hitObj = hit.collider.gameObject;
        //           if (hitObj == gameObject)
        //               return;
        //           SyncHuman h = hitObj.GetComponent<SyncHuman>();
        //           if (h == null)
        //               return;
        //           sendStr = "Hit|";
        //           sendStr += NetManager.GetDesc() + ",";
        //           sendStr += h.desc + ",";
        //           NetManager.Send(sendStr);
        //       }
        //
        //   }

    }
}
