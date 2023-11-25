using Godot;

namespace Adventure; 

public partial class Enemy : CharacterBody2D {
    public int Left = -1;
    public int Right = 1;
    
    public float Gravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");
    public float Acceleration = 2000f;
    
    public float MaxSpeed = 180f;

    [Export] public int Direction;
    
    public Node2D Graphics;
    public AnimationPlayer AnimationPlayer;
    public Node StateMachine;

    public override void _Ready() {
        Graphics = GetNode<Node2D>("Graphics");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        StateMachine = GetNode<Node>("StateMachine");
        
        SetDirection(Direction);
    }

    public void Move(float delta, float speed) {
        var velocity = Velocity;
        velocity.X = Mathf.MoveToward(velocity.X, speed * Direction, Acceleration * delta);
        velocity.Y += Gravity * delta;
        Velocity = velocity;

        MoveAndSlide();
    }

    public void SetDirection(int dir) {
        Direction = dir;
        Graphics.Scale = new Vector2(-Direction, 1);
    }

    public void Die() {
        QueueFree();
    }
}