using Godot;

namespace Adventure.State.Boar; 

public partial class BoarIdleState : BoarState {
    private bool _isBoarReady = false;
    
    public override void Enter() {
        if (Boar == null) {
            // _isBoarReady = false;
            return;
        }
        base.Enter();
        if (Boar.WallChecker.IsColliding()) {
            Boar.SetDirection(Boar.Direction * -1);
        } 
    }

    public override void Update(float delta) {
        if (!_isBoarReady) {
            if (Boar == null) {
                return;
            }

            _isBoarReady = true;
            Enter();
        }
        base.Update(delta);
        if (Returned) {
            Returned = false;
            return;
        }
        if (Boar.CanSeePlayer()) {
            StateMachine.TransitionTo("Run");
            return;
        }

        if (StateMachine.StateTime > 2) {
            StateMachine.TransitionTo("Walk");
        }
    }

    public override void PhysicsUpdate(float delta) {
        Boar.Move(delta, 0f);
    }
}