using Godot;

namespace Adventure;

[GlobalClass]
public partial class Hitbox : Area2D {
    [Signal]
    public delegate void HitEventHandler(Hurtbox hurtbox);

    public override void _Ready() {
        AreaEntered += OnAreaEntered; // parameter should be Area2D
    }

    public void OnAreaEntered(Area2D area) {
        Hurtbox hurtbox = area as Hurtbox;
        EmitSignal(SignalName.Hit, hurtbox);
        hurtbox?.EmitSignal(Hurtbox.SignalName.Hurt, this);
    }
}