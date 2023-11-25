using Godot;

namespace Adventure.State.Boar; 

public partial class BoarHurtState : BoarState {
    public override void Enter() {
        base.Enter();
        if (Boar.PendingDamage == null) {
            return;
        }
        Boar.Stats.Health -= Boar.PendingDamage.Amount;
        var dir = Boar.PendingDamage.Source.GlobalPosition.DirectionTo(Boar.GlobalPosition);
        Boar.Velocity = dir * Boar.KnockBackAmount;
        Boar.SetDirection(dir.X > 0 ? Boar.Left : Boar.Right);
        Boar.PendingDamage = null;
    }

    public override void Update(float delta) {
        base.Update(delta);
        if (Returned) {
            Returned = false;
            return;
        }
        
        if (Boar.PendingDamage != null) {
            Enter();
            return;
        }

        if (!Boar.AnimationPlayer.IsPlaying()) {
            StateMachine.TransitionTo("Run");
        }
        
    }


    public override void PhysicsUpdate(float delta) {
        Boar.Move(delta, 0f);
    }
}