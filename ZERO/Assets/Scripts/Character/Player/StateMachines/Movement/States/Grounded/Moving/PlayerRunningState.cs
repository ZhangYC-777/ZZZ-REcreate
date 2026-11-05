using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunningState : PlayerGroundedState
{
    public PlayerRunningState(MovementStateMachines movementStateMachines) : base(movementStateMachines)
    {
        
    }
    //重写运动状态的进入函数
    public override void Enter()
    {
        base.Enter();
        MovementStateMachine.ReusableData.SpeedModifier =
            MovementStateMachine.PlayerMoving.Data?.playerGroundedData?.playerRunnData?.speedModifier ?? 1f;
    }
    //重写基类的监听逻辑 按钮取消按键时暂时改变状态为漫步
    


}
