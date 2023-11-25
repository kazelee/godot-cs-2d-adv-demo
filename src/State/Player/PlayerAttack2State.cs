namespace Adventure.State.Player; 

public partial class PlayerAttack2State : PlayerState {
    public override void Enter() {
        base.Enter();
        Player.IsComboRequested = false;
    }

    public override void Update(float delta) {
        base.Update(delta);
        if (!Player.AnimationPlayer.IsPlaying()) {
            StateMachine.TransitionTo(Player.IsComboRequested ? "Attack3" : "Idle");
        }
    }

    public override void PhysicsUpdate(float delta) {
        base.PhysicsUpdate(delta);
        Player.Stand(delta, Player.Gravity, Player.FloorAcceleration);
    }
}