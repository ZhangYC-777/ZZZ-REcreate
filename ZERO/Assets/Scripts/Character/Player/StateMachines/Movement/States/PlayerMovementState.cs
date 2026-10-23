using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementState : IState
{
    public virtual void Enter()
    {
        Debug.Log("Enter " + this.GetType().Name);
    }

    public virtual void Exit()
    {
        
    }

    public virtual void HandleInput()
    {
        
    }

    public virtual void PhysicsUpdate()
    {
        
    }

    public virtual void Update()
    {
        
    }
}
