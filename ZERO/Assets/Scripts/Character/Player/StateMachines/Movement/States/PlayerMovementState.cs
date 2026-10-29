using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementState : IState
{
    //声明玩家的输入
    public Vector2 movementInput;
    //声明玩家的基本速度
    public float baseSpeed = 3f;
    //声明归一化标准
    public float speedModifier = 1f;
    protected MovementStateMachines MovementStateMachine;
    public PlayerMovementState(MovementStateMachines movementStateMachines)
    {
        MovementStateMachine = movementStateMachines;
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
        movementInput = MovementStateMachine.PlayerMoving.playerInput.playerActions.Moving.ReadValue<Vector2>();
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
        Vector3 currentVelocity = GetPlayerHorizontalSpeed();
        MovementStateMachine.PlayerMoving.playerRigidbody.AddForce(moveDirection * moveSpeed - currentVelocity, ForceMode.VelocityChange);
    }
    public void RotatePlayer(Vector3 Direction)
    {
        float DirectionAngle = Mathf.Atan2(Direction.x, Direction.z) * Mathf.Rad2Deg;
        if(DirectionAngle < 0f)
        {
            DirectionAngle += 360f;
        }
        DirectionAngle += MovementStateMachine.PlayerMoving.cameraTransform.transform.eulerAngles.y;
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
    //声明一个方法去控制速度
    public Vector3 GetPlayerHorizontalSpeed()
    {
        Vector3 horizontalVelocity = MovementStateMachine.PlayerMoving.playerRigidbody.velocity;
        horizontalVelocity.y = 0f;
        return horizontalVelocity;
        
    }
}
