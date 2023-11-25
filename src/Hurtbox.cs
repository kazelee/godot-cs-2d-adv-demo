using Godot;

namespace Adventure; 

[GlobalClass]
public partial class Hurtbox : Area2D {
    [Signal]
    public delegate void HurtEventHandler(Hitbox hitbox);
}