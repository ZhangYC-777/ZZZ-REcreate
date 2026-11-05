using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementState : IState
{
    protected MovementStateMachines MovementStateMachine;
    public PlayerMovementState(MovementStateMachines movementStateMachines)
    {
        MovementStateMachine = movementStateMachines;
        Initialize();
    }
    //初始化转身所用时间
    public void Initialize()
    {
        MovementStateMachine.ReusableData.TimeToReachTargetRotation =
            MovementStateMachine.PlayerMoving.Data?.playerGroundedData?.playerRotationData?.targetRotationReachTime
            ?? new Vector3(0f, 0.14f, 0f);
    }
    public virtual void Enter()
    {
        Debug.Log("Enter " + this.GetType().Name);
        //添加玩家按键输入的监听
        AddInputActionsCallbacks();

    }

    public virtual void Exit()
    {
        //移除玩家按键输入的监听
        RemoveInputActionsCallbacks();
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
        MovementStateMachine.ReusableData.MovementInput = MovementStateMachine.PlayerMoving.playerInput.playerActions.Moving.ReadValue<Vector2>();
    }
    public void Move()
    {
        if(MovementStateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            return;
        }
        //获取移动方向
        Vector3 moveDirection = GetMoveDirection();
        //得到目标旋转角度
        float targetRotationYAngle = RotatePlayer(moveDirection);
        //将角度转换成方向
        Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle);
        float moveSpeed = GetSpeed();
        Vector3 currentVelocity = GetPlayerHorizontalSpeed();
        //实际移动
        MovementStateMachine.PlayerMoving.playerRigidbody.AddForce(targetRotationDirection * moveSpeed - currentVelocity, ForceMode.VelocityChange);
    }
    public Vector3 GetTargetRotationDirection(float targetRotationYAngle)
    {
        return Quaternion.Euler(0f, targetRotationYAngle, 0f) * Vector3.forward;
    }
    public float RotatePlayer(Vector3 Direction)
    {
        //获取最终输入方向的角度
        float DirectionAngle = UpdateTargetRotation(Direction);
        //旋转玩家朝向目标方向
        RotateTowardsTargetRatation();
        return DirectionAngle;
    }
    //实时获取玩家的输入方向角度，并将其存储在CurrentTargetRotation中
    public float UpdateTargetRotation(Vector3 Direction, bool shouldConsiderCameraRotation = true)
    {
        float DirectionAngle = GetDirectionAngle(Direction);
        //加上相机的偏转角度
        if(shouldConsiderCameraRotation)
        {
            DirectionAngle = AddCameraRotationToAngle(DirectionAngle);
        }
        if(DirectionAngle != MovementStateMachine.ReusableData.CurrentTargetRotation.y)
        {
            UpdateTargetRotationData(DirectionAngle);
        }
       
        return DirectionAngle;
    }
    public void UpdateTargetRotationData(float DirectionAngle)
    {
        MovementStateMachine.ReusableData.CurrentTargetRotation.y = DirectionAngle;
        //角度更新后 旋转时间需要重置
        MovementStateMachine.ReusableData.DampedTargetRotationPassedTime.y = 0f;
    }
    //根据输入方向计算角度
    public float GetDirectionAngle(Vector3 Direction)
    {
        float DirectionAngle = Mathf.Atan2(Direction.x, Direction.z) * Mathf.Rad2Deg;
        if(DirectionAngle < 0f)
        {
            DirectionAngle += 360f;
        }
        return DirectionAngle;
    }
    //在输入角度上加入相机的偏转
    public float AddCameraRotationToAngle(float DirectionAngle)
    {
        DirectionAngle += MovementStateMachine.PlayerMoving.cameraTransform.transform.eulerAngles.y;
        if(DirectionAngle > 360f)
        {
            DirectionAngle -= 360f;
        }
        return DirectionAngle;
    }
    public Vector3 GetMoveDirection()
    {
        //获取玩家的输入方向
        return new Vector3(MovementStateMachine.ReusableData.MovementInput.x, 0f, MovementStateMachine.ReusableData.MovementInput.y);
    }
    public float GetSpeed()
    {
        //计算移动速度
        return (MovementStateMachine.PlayerMoving.Data?.playerGroundedData?.baseSpeed ?? 3f)
            * MovementStateMachine.ReusableData.SpeedModifier;
    }
    //声明一个方法去控制速度
    public Vector3 GetPlayerHorizontalSpeed()
    {
        Vector3 horizontalVelocity = MovementStateMachine.PlayerMoving.playerRigidbody.velocity;
        horizontalVelocity.y = 0f;
        return horizontalVelocity;
        
    }
    //旋转玩家朝向目标方向
    public void RotateTowardsTargetRatation()
    {
        float currentYAngle = MovementStateMachine.PlayerMoving.playerRigidbody.rotation.eulerAngles.y;
        if(currentYAngle == MovementStateMachine.ReusableData.CurrentTargetRotation.y)
        {
            return;
        }
        float smoothDampYAngle = Mathf.SmoothDampAngle(
            currentYAngle,
            MovementStateMachine.ReusableData.CurrentTargetRotation.y,
            ref MovementStateMachine.ReusableData.DampedTargetRotationCurrentVelocity.y,
            MovementStateMachine.ReusableData.TimeToReachTargetRotation.y - MovementStateMachine.ReusableData.DampedTargetRotationPassedTime.y);
        MovementStateMachine.ReusableData.DampedTargetRotationPassedTime.y += Time.deltaTime;
        Quaternion targetRotation = Quaternion.Euler(0f, smoothDampYAngle, 0f);
        MovementStateMachine.PlayerMoving.playerRigidbody.MoveRotation(targetRotation);
    }
    //声明一个方法让玩家的速度为0
    public void ResetPlayerSpeed()
    {
        MovementStateMachine.PlayerMoving.playerRigidbody.velocity = Vector3.zero;
    }
    //声明两个函数添加和移除玩家按键输入的监听
    public virtual void AddInputActionsCallbacks()
    {
        
    }
    public  virtual void RemoveInputActionsCallbacks()
    {
        
    }
}
