using Godot;

namespace MultiTankWar
{
    public partial class BaseHuman : CharacterBody2D
    {
        //是否正在移动
        internal bool isMoving = false;
        //移动目标点
        private Vector2 targetPosition;
        //移动速度
        public float speed = 100f;

        Marker2D marker;
        PackedScene bulletTscn;

        //是否正在攻击
        internal bool isAttacking = false;
        internal float attackTime = float.MinValue;
        //描述
        public string desc = "";

        private Vector2 moveDirection;
        private float projectileRotation;

        //移动到某处
        public void MoveTo(Vector2 pos)
        {
            targetPosition = pos;
            isMoving = true;
            //每次设定目标的时候更新一下子弹的方向
          //  moveDirection = (targetPosition - marker.GlobalPosition).Normalized();
        }

        //移动Update
        public void MoveUpdate(double delta)
        {
            if (!isMoving)
            {
                return;
            }
            //this.Rotation = targetPosition.AngleToPoint(Position);
            Vector2 directionVector = targetPosition - Position;
            this.Position += directionVector.Normalized() * speed * (float)delta;
            LookAt(targetPosition);
            if (directionVector.Length() < 5)
            {
                isMoving = false;
            }

        }


        //攻击动作
        public void Attack()
        {
            isAttacking = true;
        }

        //攻击Update
        private void AttackUpdate()
        {
           
            if (!isAttacking) return;
            var newBullet = bulletTscn.Instantiate<Projectile>();
            //newBullet.Scale = new Vector2(0.2f, 0.2f);
            //var newBullet = new Projectile();
            newBullet.Position = marker.GlobalPosition;
            //newBullet.TargetPostion = targetPosition;
          ////这样也不行,在移动过程中,当炮筒的移动超过目标点后,再点攻击,方向就被刷成反向了
          if (isMoving) {
          
              moveDirection = (targetPosition - marker.GlobalPosition).Normalized();
          }

           // GD.Print($"当前坦克状态移动状态{isMoving}当前子弹方向{moveDirection}");
            newBullet.Direction = moveDirection;
            newBullet.Rotation = moveDirection.Angle();
            //  newBullet.LookAt(moveDirection);
            GetTree().Root.AddChild(newBullet);
            // newBullet.Rotation = Rotation;
            isAttacking = false;
        }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            bulletTscn = ResourceLoader.Load<PackedScene>("Projectile.tscn");
            marker = GetNode<Marker2D>("Sprite2D2/Sprite2D/Sprite2D3/Sprite2D4/Marker2D");
        }
        public override void _PhysicsProcess(double delta)
        {
            MoveUpdate(delta);

            AttackUpdate();
            MoveAndSlide();
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {

        }

        public void Die()
        {
            QueueFree();
        }


    }
}
