using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateMachines : StateMachine
{
    public PlayerMoving PlayerMoving { get; }
    public PlayerReusableData ReusableData { get; }
   public PlayerIdlingState idlingState { get;  }
   public PlayerRunningState runningState { get;  }
   public PlayerDashState dashingState { get;  }
   public MovementStateMachines(PlayerMoving playerMoving)
   {
        PlayerMoving = playerMoving;
        ReusableData = new PlayerReusableData();
       idlingState = new PlayerIdlingState(this);
       runningState = new PlayerRunningState(this);
       dashingState = new PlayerDashState(this);
   }
}
