namespace Adventure.State.Player; 

public partial class PlayerHurtState : PlayerState {
    public override void Enter() {
        base.Enter();
        if (Player.PendingDamage == null) {
            return;
        }
        Player.Stats.Health -= Player.PendingDamage.Amount;
        var dir = Player.PendingDamage.Source.GlobalPosition.DirectionTo(Player.GlobalPosition);
        Player.Velocity = dir * Player.KnockBackAmount;
        Player.PendingDamage = null;
        Player.InvincibleTimer.Start();
    }

    public override void Update(float delta) {
        base.Update(delta);
        if (Returned) {
            Returned = false;
            return;
        }

        if (Player.PendingDamage != null) {
            Enter();
            return;
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