namespace Adventure.State.Player; 

public partial class PlayerDyingState : PlayerState {
    public override void Enter() {
        base.Enter();
        Player.InvincibleTimer.Stop();
    }

    public override void PhysicsUpdate(float delta) {
        base.PhysicsUpdate(delta);
        Player.Stand(delta, Player.Gravity, Player.FloorAcceleration);
    }
}