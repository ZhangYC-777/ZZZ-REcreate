public interface IState
{
    public void Enter(); //进入时的操作
    public void Exit(); //离开时的操作
    public void HandleInput(); //输入操作
    public void Update(); //非物理逻辑
    public void PhysicsUpdate(); //物理帧逻辑
    
} 
