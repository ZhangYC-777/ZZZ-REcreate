using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//对有关地面移动状态切换的逻辑进行通用处理
public class PlayerGroundedState : PlayerMovementState
{
    public PlayerGroundedState(MovementStateMachines movementStateMachines) : base(movementStateMachines)
    {
        
    }
    public override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();
        MovementStateMachine.PlayerMoving.playerInput.playerActions.Moving.canceled += OnMoveCanceled;
    }
    public override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();
        MovementStateMachine.PlayerMoving.playerInput.playerActions.Moving.canceled -= OnMoveCanceled;
    }
    //声明监听事件改变状态
    public virtual void OnMoveCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        MovementStateMachine.ChangeState(MovementStateMachine.idlingState);
    }
    public virtual void OnMove()
    {
        MovementStateMachine.ChangeState(MovementStateMachine.runningState);
    }
    //声明监听事件进入冲刺状态
    public virtual void OnDash()
    {
        MovementStateMachine.ChangeState(MovementStateMachine.dashingState);
    }

}
