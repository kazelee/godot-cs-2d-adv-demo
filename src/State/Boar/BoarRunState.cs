using Godot;

namespace Adventure.State.Boar; 

public partial class BoarRunState : BoarState {
    public override void Update(float delta) {
        base.Update(delta);
        if (Returned) {
            Returned = false;
            return;
        }
        if (!Boar.CanSeePlayer() && Boar.CalmDownTimer.IsStopped()) {
            StateMachine.TransitionTo("Walk");
        }
    }

    public override void PhysicsUpdate(float delta) {
        base.PhysicsUpdate(delta);
        if (Boar.WallChecker.IsColliding() || !Boar.FloorChecker.IsColliding()) {
            Boar.SetDirection(Boar.Direction * -1);
        }
        Boar.Move(delta, Boar.MaxSpeed);
        if (Boar.CanSeePlayer()) {
            Boar.CalmDownTimer.Start();
        }
    }
}