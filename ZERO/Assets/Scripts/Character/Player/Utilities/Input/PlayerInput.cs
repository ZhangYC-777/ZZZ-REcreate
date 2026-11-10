using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
   public PlayerInputMap playerInputMap {get; private set;}
   public PlayerInputMap.PlayerActions playerActions {get; private set;}
   void Awake()
    {
        playerInputMap = new PlayerInputMap();
        playerActions = playerInputMap.Player;
    }
    void OnEnable()
    {
        playerInputMap.Enable();
    }
    void OnDisable()
    {
        // 组件关闭时取消等待，避免协程稍后重新启用输入。
        StopAllCoroutines();
        playerInputMap.Disable();
    }

    public void DisableActionFor(InputAction action, float seconds)
    {
        StartCoroutine(DisableAction(action, seconds));
    }

    private IEnumerator DisableAction(InputAction action, float seconds)
    {
        // 立即禁用指定的输入动作。
        action.Disable();

        // 只暂停这段流程，等待期间游戏仍然正常更新。
        yield return new WaitForSeconds(seconds);

        // 等待结束后，从这里继续执行并恢复输入。
        action.Enable();
    }
}
