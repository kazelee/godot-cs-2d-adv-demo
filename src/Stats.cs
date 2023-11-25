using System.ComponentModel;
using Godot;

namespace Adventure; 

public partial class Stats : Node {
    [Signal]
    public delegate void HealthChangedEventHandler();
    
    [Export] public int MaxHealth = 3;

    private int _health;
    public int Health {
        get => _health;
        set {
            value = Mathf.Clamp(value, 0, MaxHealth);
            if (Health == value) {
                return;
            }
            _health = value;
            EmitSignal(SignalName.HealthChanged);
        }
    }

    public override void _Ready() {
        Health = MaxHealth;
    }
}