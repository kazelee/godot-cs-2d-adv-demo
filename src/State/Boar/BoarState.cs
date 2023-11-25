using System;
using Godot;

namespace Adventure.State.Boar; 

public partial class BoarState : State {
    public Adventure.Boar Boar;
    
    [Export] protected String AnimationName;

    protected bool Returned = false; // Update: get next state

    public override void Enter() {
        Boar.AnimationPlayer.Play(AnimationName);
    }
    
    public override void Update(float delta) {
        if (Boar.Stats.Health == 0) {
            StateMachine.TransitionTo("Dying");
            Returned = true;
            return;
        }

        if (Boar.PendingDamage != null && this is not BoarHurtState) {
            StateMachine.TransitionTo("Hurt");
            Returned = true;
        }
    }
}