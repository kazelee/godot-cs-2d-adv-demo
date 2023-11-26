using Adventure.State.Player;
using Godot;
using Vector2 = Godot.Vector2;

namespace Adventure; 

public partial class Player : CharacterBody2D
{
    public float Gravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");
    public float AirAcceleration = 800.0f; // RunSpeed / 0.2f
    public float FloorAcceleration = 1600.0f; // RunSpeed / 0.1f
    
    public float RunSpeed = 160.0f;
    public float JumpVelocity = -360.0f;
    public Vector2 WallJumpVelocity = new Vector2(380, -280);
    public float KnockBackAmount = 512.0f;

    public bool LeaveFromOnFloorState = false; // from in FloorState
    public bool IsFirstTick = false;

    public bool LeaveFromWall = false; // used for WallJumpTimer

    [Export] public bool CanCombo = false;
    public bool IsComboRequested = false;

    // public int AttackAmount = 2;

    public Damage PendingDamage;
    
    public Node2D Graphics;
    public AnimationPlayer AnimationPlayer;
    public Timer CoyoteTimer;
    public Timer JumpRequestTimer;
    public Timer WallJumpTimer;
    public Timer InvincibleTimer;
    public Node StateMachine;
    public Stats Stats;
    public RayCast2D HandChecker;
    public RayCast2D FootChecker;
    public Hurtbox Hurtbox;
    public Hitbox Hitbox;
	
    public override void _Ready() {
        Graphics = GetNode<Node2D>("Graphics");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        CoyoteTimer = GetNode<Timer>("CoyoteTimer");
        JumpRequestTimer = GetNode<Timer>("JumpRequestTimer");
        WallJumpTimer = GetNode<Timer>("WallJumpTimer");
        InvincibleTimer = GetNode<Timer>("InvincibleTimer");
        HandChecker = GetNode<RayCast2D>("Graphics/HandChecker");
        FootChecker = GetNode<RayCast2D>("Graphics/FootChecker");
        Stats = GetNode<Stats>("Stats");
        StateMachine = GetNode<Node>("StateMachine");
        Hurtbox = GetNode<Hurtbox>("Hurtbox");
        Hitbox = GetNode<Hitbox>("Hitbox");
        foreach (PlayerState state in StateMachine.GetChildren()) { // make sure state is PlayerState
            state.Player = this;
        }

        Hurtbox.Hurt += Hurtbox_OnHurt;
    }

    public void Move(float delta, float gravity, float acceleration) {
        var direction = Input.GetAxis("move_left", "move_right");
        Velocity = new Vector2(
            Mathf.MoveToward(Velocity.X, direction * RunSpeed, acceleration * delta),
            Velocity.Y + gravity * delta
        );
        
        if (!Mathf.IsZeroApprox(direction)) {
            Graphics.Scale = new Vector2(direction < 0 ? -1 : 1, 1);
        }
        
        MoveAndSlide();
    }

    public void Stand(float delta, float gravity, float acceleration) {
        var velocity = Velocity;
        velocity.X = Mathf.MoveToward(velocity.X, 0f, acceleration * delta);
        velocity.Y += gravity * delta;
        Velocity = velocity;

        MoveAndSlide();
    }

    public void Die() {
        GetTree().ReloadCurrentScene();
    }
    
    public bool CanWallSlide() {
        return IsOnWall() && HandChecker.IsColliding() && FootChecker.IsColliding();
    }

    public void Hurtbox_OnHurt(Hitbox hitbox) {
        if (InvincibleTimer.TimeLeft > 0) {
            return;
        }

        PendingDamage = new Damage();
        PendingDamage.Amount = 1;
        PendingDamage.Source = hitbox.Owner as Node2D;
    }
    
    public override void _UnhandledInput(InputEvent @event) {
        base._UnhandledInput(@event);
        if (@event.IsActionPressed("jump")) {
            JumpRequestTimer.Start();
        }

        if (@event.IsActionReleased("jump")) {
            JumpRequestTimer.Stop();
            if (Velocity.Y < JumpVelocity / 2f) {
                Velocity = new Vector2(Velocity.X, JumpVelocity / 2f);
            }
        }

        if (Input.IsActionJustPressed("attack") && CanCombo) {
            IsComboRequested = true;
        }
    }
}