using Godot;

namespace Adventure.State.Player; 

public partial class PlayerWallSlidingState : PlayerState {
    public override void Enter() {
        base.Enter();
        Player.WallJumpTimer.Stop(); // other state to WallSliding
    }

    public override void Update(float delta) {
        base.Update(delta);
        if (Returned) {
            Returned = false;
            return;
        }
        // OR: if (Player.JumpRequestTimer.TimeLeft > 0 && Player.IsFirstTick)
        if (Player.JumpRequestTimer.TimeLeft > 0 && StateMachine.StateTime < 0.1) {
            StateMachine.TransitionTo("WallJump");
        }
        
        if (Player.IsOnFloor()) {
            StateMachine.TransitionTo("Idle");
            return;
        }
        
        if (!Player.IsOnWall()) {
            StateMachine.TransitionTo("Fall");
        }
    }

    public override void PhysicsUpdate(float delta) {
        base.PhysicsUpdate(delta);
        Player.Move(delta, Player.Gravity / 3f, Player.AirAcceleration);
        Player.Graphics.Scale = new Vector2(Player.GetWallNormal().X, 1);
    }
}