using Adventure.State.Boar;
using Godot;

namespace Adventure; 

public partial class Boar : Enemy {
    public float KnockBackAmount = 512f;
    // public int AttackAmount = 1;

    public Damage PendingDamage; 
    
    public RayCast2D WallChecker;
    public RayCast2D FloorChecker;
    public RayCast2D PlayerChecker;
    public Timer CalmDownTimer;
    public Hitbox Hitbox;
    public Hurtbox Hurtbox;
    public Stats Stats;
    
    public override void _Ready() {
        base._Ready();
        WallChecker = GetNode<RayCast2D>("Graphics/WallChecker");
        FloorChecker = GetNode<RayCast2D>("Graphics/FloorChecker");
        PlayerChecker = GetNode<RayCast2D>("Graphics/PlayerChecker");
        CalmDownTimer = GetNode<Timer>("CalmDownTimer");
        Hitbox = GetNode<Hitbox>("Hitbox");
        Hurtbox = GetNode<Hurtbox>("Hurtbox");
        Stats = GetNode<Stats>("Stats");
        Hurtbox.Hurt += Hurtbox_OnHurt;
        
        foreach (BoarState state in StateMachine.GetChildren()) { // make sure state is BoarState
            state.Boar = this;
        }
    }

    public bool CanSeePlayer() {
        if (!PlayerChecker.IsColliding()) {
            return false;
        }
        return PlayerChecker.GetCollider() is Player;
    }
    
    public void Hurtbox_OnHurt(Hitbox hitbox) {
        PendingDamage = new Damage();
        PendingDamage.Amount = 1;
        PendingDamage.Source = hitbox.Owner as Node2D;
    }
}