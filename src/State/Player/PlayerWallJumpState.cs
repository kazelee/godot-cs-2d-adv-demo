using Godot;

namespace Adventure.State.Player; 

public partial class PlayerWallJumpState : PlayerState {
    public override void Enter() {
        base.Enter();
        Player.IsFirstTick = true;
        var velocity = Player.WallJumpVelocity;
        velocity.X *= Player.GetWallNormal().X;
        Player.Velocity = velocity;
        
        Player.WallJumpTimer.Stop(); // add Wall Jump Timer
        Player.JumpRequestTimer.Stop();
    }

    public override void Update(float delta) {
        base.Update(delta);
        if (Player.CanWallSlide() && Player.IsFirstTick) {
            StateMachine.TransitionTo("WallSliding");
            return;
        }

        if (Player.Velocity.Y >= 0) {
            StateMachine.TransitionTo("Fall");
        }
    }

    public override void PhysicsUpdate(float delta) {
        base.PhysicsUpdate(delta);
        if (StateMachine.StateTime < 0.1) {
            Player.Stand(delta, Player.IsFirstTick ? 0f : Player.Gravity, Player.AirAcceleration);
            Player.Graphics.Scale = new Vector2(Player.GetWallNormal().X, 1);
        }
        else {
            Player.Move(delta, Player.Gravity, Player.AirAcceleration);
        }

        Player.IsFirstTick = false;
    }
}