namespace Adventure.State.Player; 

public partial class PlayerAttack3State : PlayerState {
    public override void Enter() {
        base.Enter();
        Player.IsComboRequested = false;
    }

    public override void Update(float delta) {
        base.Update(delta);
        if (!Player.AnimationPlayer.IsPlaying()) {
            StateMachine.TransitionTo("Idle");
        }
    }

    public override void PhysicsUpdate(float delta) {
        base.PhysicsUpdate(delta);
        Player.Stand(delta, Player.Gravity, Player.FloorAcceleration);
    }
}