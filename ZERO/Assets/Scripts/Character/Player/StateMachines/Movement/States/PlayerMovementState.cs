using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementState : IState
{
    //声明玩家的输入
    public Vector2 movementInput;
    //声明玩家的基本速度
    public float baseSpeed = 5f;
    //声明归一化标准
    public float speedModifier = 1f;
    protected MovementStateMachines MovementStateMachines;
    public PlayerMovementState(MovementStateMachines movementStateMachines)
    {
        MovementStateMachines = movementStateMachines;
    }
    public virtual void Enter()
    {
        Debug.Log("Enter " + this.GetType().Name);
    }

    public virtual void Exit()
    {
        
    }

    public virtual void HandleInput()
    {
        //获取玩家的输入
        GetMovementInput();
    }

    public virtual void PhysicsUpdate()
    {
        //执行玩家移动逻辑
        Move();
    }

    public virtual void Update()
    {
        
    }
    public void GetMovementInput()
    {
        movementInput = MovementStateMachines.PlayerMoving.playerInput.playerActions.Moving.ReadValue<Vector2>();
    }
    public void Move()
    {
        if(movementInput == Vector2.zero)
        {
            return;
        }
        //获取移动方向
        Vector3 moveDirection = GetMoveDirection();
        float moveSpeed = GetSpeed();
    }
    public Vector3 GetMoveDirection()
    {
        //获取玩家的输入方向
        return new Vector3(movementInput.x, 0f, movementInput.y);
    }
    public float GetSpeed()
    {
        //计算移动速度
        return baseSpeed * speedModifier;
    }
}
