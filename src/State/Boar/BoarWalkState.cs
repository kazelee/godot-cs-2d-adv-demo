using Godot;

namespace Adventure.State.Boar; 

public partial class BoarWalkState : BoarState {
    public override void Enter() {
        base.Enter();
        if (!Boar.FloorChecker.IsColliding()) {
            Boar.SetDirection(Boar.Direction * -1);
        }
        Boar.FloorChecker.ForceRaycastUpdate();
    }

    public override void Update(float delta) {
        base.Update(delta);
        if (Returned) {
            Returned = false;
            return;
        }
        if (Boar.CanSeePlayer()) {
            StateMachine.TransitionTo("Run");
            return;
        }

        if (Boar.WallChecker.IsColliding() || !Boar.FloorChecker.IsColliding()) {
            StateMachine.TransitionTo("Idle");
        }
    }

    public override void PhysicsUpdate(float delta) {
        Boar.Move(delta, Boar.MaxSpeed / 3f);
    }
}