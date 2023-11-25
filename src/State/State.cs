using Godot;
using System;

namespace Adventure.State;

public partial class State : Node {
    public StateMachine StateMachine;

    public virtual void Enter() {
    }
    public virtual void Exit() {
    }

    public new virtual void Ready() {
    }
    public virtual void Update(float delta) {
    }
    public virtual void PhysicsUpdate(float delta) {
    }
    public virtual void HandleInput(InputEvent @event) {
    }
}
