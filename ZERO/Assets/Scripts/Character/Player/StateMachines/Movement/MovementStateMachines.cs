using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateMachines : StateMachine
{
    public PlayerMoving PlayerMoving { get; }
   public PlayerIdlingState idlingState { get;  }
   public PlayerRunningState runningState { get;  }
   public MovementStateMachines(PlayerMoving playerMoving)
   {
        PlayerMoving = playerMoving;
       idlingState = new PlayerIdlingState(this);
       runningState = new PlayerRunningState(this);
   }
}
