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
        MovementStateMachine.ReusableData.SpeedModifier = 1f;
    }
}
