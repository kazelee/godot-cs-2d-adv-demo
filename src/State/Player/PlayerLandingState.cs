namespace Adventure.State.Player; 

public partial class PlayerLandingState : PlayerState {
    public override void Update(float delta) {
        base.Update(delta);
        if (Returned) {
            Returned = false;
            return;
        }

        if (!IsStill) {
            StateMachine.TransitionTo("Running");
        }
        
        if (!Player.AnimationPlayer.IsPlaying()) {
            StateMachine.TransitionTo("Idle");
        }
    }

    public override void PhysicsUpdate(float delta) {
        base.PhysicsUpdate(delta);
        Player.Stand(delta, Player.Gravity, Player.FloorAcceleration);
    }
}