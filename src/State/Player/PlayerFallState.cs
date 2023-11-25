namespace Adventure.State.Player; 

public partial class PlayerFallState : PlayerState {
    public override void Enter() {
        base.Enter();
        
        if (Player.LeaveFromOnFloorState) {
            Player.CoyoteTimer.Start();
        }

        if (Player.LeaveFromWall) {
            Player.WallJumpTimer.Start();
        }
    }

    public override void Update(float delta) {
        base.Update(delta);
        if (Returned) {
            Returned = false;
            return;
        }
        
        if (Player.IsOnFloor()) {
            StateMachine.TransitionTo(IsStill ? "Landing" : "Running");
            return;
        }

        if (Player.CanWallSlide()) {
            StateMachine.TransitionTo("WallSliding");
        }
    }

    public override void PhysicsUpdate(float delta) {
        base.PhysicsUpdate(delta);
        Player.Move(delta, Player.Gravity, Player.AirAcceleration);
    }
}