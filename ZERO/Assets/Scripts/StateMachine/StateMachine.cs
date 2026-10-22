public abstract class StateMachine
{
   protected IState currentState;
   //需要一个方法去改变状态
   public void ChangeState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }
    public void HandleInput()
    {
        currentState?.HandleInput();
    }
    public void Update()
    {
        currentState?.Update();
    }
    public void PhysicsUpdate()
    {
        currentState?.PhysicsUpdate();
    }
}
