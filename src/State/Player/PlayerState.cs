using System;
using Godot;

namespace Adventure.State.Player; 

public partial class PlayerState : State {
    public Adventure.Player Player;

    [Export] protected bool IsOnFloorState = false;
    [Export] protected String AnimationName;
    
    protected bool IsStill = true; // not move by default
    protected bool Returned = false; // Update: get next state
    
    public override void Enter() {
        // 游戏开始的第一帧 Player 还没有准备好
        if (Player == null) {
            return;
        }
        
        if (!Player.LeaveFromOnFloorState && IsOnFloorState) {
            Player.CoyoteTimer.Stop();
        }

        if (!Player.LeaveFromWall && this is PlayerWallSlidingState s) {
            Player.WallJumpTimer.Stop();
        }
        
        Player.AnimationPlayer.Play(AnimationName);
    }

    public override void PhysicsUpdate(float delta) {
        // if (Player == null) {
        //     return;
        // }
    }

    public override void Exit() {
        // if (Player == null) {
        //     return;
        // }
        
        Player.LeaveFromOnFloorState = IsOnFloorState;
        Player.LeaveFromWall = this is PlayerWallSlidingState;
    }

    public override void Update(float delta) {
        // if (Player == null) {
        //     return;
        // }

        var canJump = Player.IsOnFloor() || Player.CoyoteTimer.TimeLeft > 0 || Player.WallJumpTimer.TimeLeft > 0;
        var shouldJump = canJump && Player.JumpRequestTimer.TimeLeft > 0;
        if (shouldJump) {
            StateMachine.TransitionTo("Jump");
            Returned = true;
            return;
        }

        if (IsOnFloorState && !Player.IsOnFloor()) {
            StateMachine.TransitionTo("Fall");
            Returned = true;
            return;
        }
        
        var direction = Input.GetAxis("move_left", "move_right");
        IsStill = Mathf.IsZeroApprox(direction) && Mathf.IsZeroApprox(Player.Velocity.X);
    }
    
}