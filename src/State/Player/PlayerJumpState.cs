using Godot;

namespace Adventure.State.Player; 

public partial class PlayerJumpState : PlayerState {
    public override void Enter() {
        base.Enter();
        Player.IsFirstTick = true;
        Player.Velocity = new Vector2(Player.Velocity.X, Player.JumpVelocity);
        Player.CoyoteTimer.Stop();
        Player.JumpRequestTimer.Stop();
    }

    public override void Update(float delta) {
        base.Update(delta);
        if (Returned) {
            Returned = false;
            return;
        }
        if (Player.Velocity.Y >= 0) {
            StateMachine.TransitionTo("Fall");
        }
    }
    
    public override void PhysicsUpdate(float delta) {
        base.PhysicsUpdate(delta);
        // GD.Print(Player.IsFirstTick);
        Player.Move(delta, Player.IsFirstTick ? 0f : Player.Gravity, Player.AirAcceleration);
        Player.IsFirstTick = false;
    }
}