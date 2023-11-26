using Godot;

namespace Adventure; 

public partial class StatusPanel : HBoxContainer {
    [Export] public Stats Stats;
    public TextureProgressBar HealthBar;
    public TextureProgressBar EasedHealthBar;

    public override void _Ready() {
        HealthBar = GetNode<TextureProgressBar>("HeathBar");
        EasedHealthBar = GetNode<TextureProgressBar>("HeathBar/EasedHealthBar");
        Stats.HealthChanged += UpdateHealth;
        UpdateHealth();
    }

    public void UpdateHealth() {
        var percentage = Stats.Health / (float)Stats.MaxHealth;
        HealthBar.Value = percentage;
        CreateTween().TweenProperty(EasedHealthBar, "value", percentage, 0.3);
    }
}