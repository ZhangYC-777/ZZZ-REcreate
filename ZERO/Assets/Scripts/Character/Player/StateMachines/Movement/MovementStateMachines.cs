using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateMachines : StateMachine
{
   public PlayerIdlingState idlingState { get;  }
   public PlayerRunningState runningState { get;  }
   public MovementStateMachines()
   {
       idlingState = new PlayerIdlingState();
       runningState = new PlayerRunningState();
   }
}
