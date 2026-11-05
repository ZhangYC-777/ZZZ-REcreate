using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerIdlingState : PlayerGroundedState
{
    public PlayerIdlingState(MovementStateMachines movementStateMachines) : base(movementStateMachines)
    {
        
    }
    
    //重写运动状态的进入函数
    public override void Enter()
    {
        base.Enter();
        MovementStateMachine.ReusableData.SpeedModifier = 0f;
        //调用重置玩家速度的方法
        ResetPlayerSpeed();
    }
    //重写update函数
    public override void Update()
    {
        base.Update();
        //检测玩家输入 有输入执行移动状态改变逻辑
        if(MovementStateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            return;
        }
        //状态改变函数
        OnMove(); 
    }
    

    
}
