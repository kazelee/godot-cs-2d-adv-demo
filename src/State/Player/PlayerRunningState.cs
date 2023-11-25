using Godot;

namespace Adventure.State.Player; 

public partial class PlayerRunningState : PlayerState {
    public override void Update(float delta) {
        base.Update(delta);
        if (Returned) {
            Returned = false;
            return;
        }
        // if (!Player.IsOnFloor()) {
        //     StateMachine.TransitionTo("Fall");
        // }
        if (Input.IsActionJustPressed("attack")) {
            StateMachine.TransitionTo("Attack1");
            return;
        }
        if (IsStill) {
            StateMachine.TransitionTo("Idle");
        }
    }

    public override void PhysicsUpdate(float delta) {
        base.PhysicsUpdate(delta);
        Player.Move(delta, Player.Gravity, Player.FloorAcceleration);
    }
}