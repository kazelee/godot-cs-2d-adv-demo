namespace Adventure.State.Boar; 

public partial class BoarDyingState : BoarState {
    public override void PhysicsUpdate(float delta) {
        Boar.Move(delta, 0f);
    }
}