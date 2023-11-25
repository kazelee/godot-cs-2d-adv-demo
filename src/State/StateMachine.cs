using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Adventure.State;

public partial class StateMachine : Node {
    [Export] public NodePath InitialState;
    
    private Dictionary<string, State> _states;
    private State _currentState;

    public float StateTime = 0f; // instead of timer
    
    public override void _Ready() {
        
        _states = new Dictionary<string, State>();
        foreach (var node in GetChildren()) {
            if (node is State s) {
                _states[node.Name] = s;
                s.StateMachine = this;
                s.Ready();
                // s.Exit(); // reset
            }
        }
        
        _currentState = GetNode<State>(InitialState);
        _currentState.Enter();
    }

    public override void _Process(double delta) {
        _currentState.Update((float)delta);
    }

    public override void _PhysicsProcess(double delta) {
        _currentState.PhysicsUpdate((float)delta);
        StateTime += (float)delta; // add in physics process
    }

    public override void _UnhandledInput(InputEvent @event) {
        _currentState.HandleInput(@event);
    }

    public void TransitionTo(string key) {
        if (!_states.ContainsKey(key) || _states[key] == _currentState) {
            return;
        }
        // log state changes
        GD.Print($"[{Engine.GetPhysicsFrames()}] {_currentState.Name} => {_states[key].Name}");
        
        _currentState.Exit();
        _currentState = _states[key];
        _currentState.Enter();

        StateTime = 0f; // reset in transition state
    }
}
