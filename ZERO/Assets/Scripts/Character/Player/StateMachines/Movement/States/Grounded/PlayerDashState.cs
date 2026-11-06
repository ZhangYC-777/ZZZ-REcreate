using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerGroundedState
{
    public PlayerDashState(MovementStateMachines movementStateMachines) : base(movementStateMachines)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        MovementStateMachine.ReusableData.SpeedModifier = MovementStateMachine.PlayerMoving.Data?.playerGroundedData?.playerDashData?.speedModifier ?? 2f;
        //声明一个方法去处理从静止状态到冲刺状态的过渡
        AddForceToPlayer();
    }
    public void AddForceToPlayer()
    {
        if(MovementStateMachine.ReusableData.MovementInput != Vector2.zero)
        {
            return;
        }
        //获得玩家的方向
        Vector3 PlayerDirection = MovementStateMachine.PlayerMoving.transform.forward;
        PlayerDirection.y = 0f;
        //给玩家添加一个向后冲刺的力
        MovementStateMachine.PlayerMoving.playerRigidbody.velocity = -PlayerDirection * GetSpeed();

    }
}
